using System;
using System.Text;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security.Cryptography;
using System.Diagnostics;
using SisConAxs_DM.Models;
using SisConAxs_DM.Repository;
using SisConAxs_DM.DTO;
using SisConAxs_DM.Utils;
using SisConAxs.Integration;

namespace SisConAxs.Session
{
    public sealed class SessionManager
    {
        private static readonly SessionManager instance;

        public ConcurrentDictionary<string, SessionInfo> SessionList = new ConcurrentDictionary<string, SessionInfo>();
        public int expiredTime = 60;

        static SessionManager() { instance = new SessionManager(); }

        private SessionManager() { }

        public static SessionManager Instance
        {
            get { return instance; }
        }

        public static SessionInfo LogIn(AuthenticationDataDTO authData)
        {
            SisConAxsContext db = new SisConAxsContext();
            AccessUsersRepository userRepository = new AccessUsersRepository();
            ListTokensDebug();

            //if (data.empresa == null )
            //{
            //    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);  // Unauthorized
            //    message.Content = new StringContent("El usuario no tiene empresas asociada en el Azure AD");
            //    throw new HttpResponseException(message);
            //}

            var authProvider = AuthenticationProviderFactory.Get(authData.grant_type);
            var validate = authProvider.Validate(authData.username, authData.credentials);
            if (validate)
            {
                AccessUsers foundUser = null;
                foundUser = db.AccessUsers.FirstOrDefault(u => u.UserInternalID.Trim() == authData.userAlias);
                if (foundUser != null)
                {
                    if (foundUser.UserCompanies.Count == 0 && foundUser.UserRoleSysAdmin == 0)
                    {
                        HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        message.Content = new StringContent("El usuario no tiene empresas asociadas.");
                        throw new HttpResponseException(message);
                    }
                    if (foundUser.UserStatus == 0)
                    {
                        HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        message.Content = new StringContent("El usuario NO está activo.");
                        throw new HttpResponseException(message);
                    }

                    SessionInfo session = CreateSession(foundUser);
                    //new PeopleRepository(session.getSessionData()).SavePeopleFromAuthenticationData(authData, Constants.USER_SYSTEM_ICARUS);  // Creamos/Actualizamos la información de la persona
                    return session;
                }
                else
                {
                    //Si la empresa que viene del AD no está en Icarus, no registra al usuario
                    var company = new CompanyRepository().GetCompanyByAD(authData.empresa.Trim()).FirstOrDefault();
                    if (company == null)
                    {
                        HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        message.Content = new StringContent("El nombre de la empresa asociada a su usuario no se encuentra registrada en ICARUS, comuníquese con Mesa de ayuda");
                        throw new HttpResponseException(message);
                    }

                    // Creamos al Usuario
                    new AccessUsersRepository().SaveAccessUserFromAuthenticationData(
                        authData,                           // Nuevo usuario
                        new AccessUsers()                   // Usuario creador : Sistema (ICARUS)
                        {
                            EditUser = "Sistema",
                            UserInternalID = ConstantsDM.USER_SYSTEM_ICARUS
                        }
                    );

                    new PeopleRepository(ConstantsDM.USER_SYSTEM_ICARUS_SESSIONDATA).SavePeopleFromAuthenticationData(authData);   // Creamos/Actualizamos el Registro de Persona

                    
                    foundUser = db.AccessUsers.FirstOrDefault(u => u.UserInternalID.Trim() == authData.userAlias);

                    //Sincronizamos el recurso de aprobador                    
                    AccessUserDTO dto = new AccessUserDTO();
                    AutoMapper.Mapper.Map<AccessUsers, AccessUserDTO>(foundUser, dto);
                    new IntegrationResourceIcarusAccess().SyncResource(dto);

                    // Creamos la Sesion
                    SessionInfo session = CreateSession(foundUser);
                    return session;
                }
            }
            return null;
        }

        public static void LogOut(string token)
        {
            ListTokensDebug();
            SessionInfo tmpSession = null;
            CheckListExpired();
            SessionManager.Instance.SessionList.TryGetValue(token, out tmpSession);
            if (tmpSession != null)
                SessionManager.Instance.SessionList.TryRemove(token, out tmpSession);
        }

        //public static bool ValidateUserAD(string user, string pass)
        //{
        //    GyM.Security.Authentication authAD = new GyM.Security.Authentication();
        //    string domain = new SystemConfigRepository().GetADdomainConfig().NotifConfHost;

        //    AccessUserDTO filter = new AccessUserDTO();
        //    filter.UserInternalID = user;
        //    //var userAD = new AccessUsersRepository().GetAccessUsersAD(filter, null).FirstOrDefault();
        //    //if (userAD != null)
        //    //{
        //    //    //domain = userAD.UserPrincipalName.Split('@')[1];
        //    //}

        //    return authAD.IsAuthenticated(domain, user, pass);
        //}

        public static SessionInfo ValidateSession(HttpRequestMessage msg, List<int> approvedUserProfiles = null) //int[] roles = null)
        {
            bool success = false;
            string authToken = "";
            IEnumerable<string> values;
            msg.Headers.TryGetValues("X-Auth-Token", out values);
            if (values != null) authToken = values.First();
            SessionInfo session = SessionManager.GetSessionByToken(authToken);
            if (session == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);   //Unauthorized
                message.Content = new StringContent("No ha iniciado sesión");
                throw new HttpResponseException(message);
            }
            else if (approvedUserProfiles == null)  //roles == null)
            {
                //success = true;
                return session;
            }
            else if (approvedUserProfiles.Count > 0) //roles.Length > 0)
            {
                var sessionData = session.getSessionData();
                if (sessionData.HavePermission(approvedUserProfiles.ToArray()))
                    return session;
            }

            if (!success)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Forbidden);   //Unauthorized
                message.Content = new StringContent("No cuenta con privilegios suficientes para acceder a esta función");
                throw new HttpResponseException(message);
            }
            return session;
        }

        private static SessionInfo CreateSession(AccessUsers user)
        {
            AccessUsersRepository userRepository = new AccessUsersRepository();
            SessionInfo session = new SessionInfo(user);
            session.SetCompany();
            session.GenerateToken();
            SessionManager.Instance.SessionList.TryAdd(session.SessionToken, session);

            userRepository.UpdateLastAccessUsers(user.UserInternalID);  // ULTIMO ACCESO DENTRO DEL SISTEMA
            return session;
        }

        public static SessionInfo GetSessionByToken(string token)
        {
            ListTokensDebug();
            SessionInfo tmpSession = null;
            CheckListExpired();
            SessionManager.Instance.SessionList.TryGetValue(token, out tmpSession);
            if (tmpSession != null) tmpSession.SessionStart = DateTime.Now;
            return tmpSession;
        }

        private static void CheckListExpired()
        {
            SessionInfo tmpSession = null;
            foreach (KeyValuePair<string, SessionInfo> item in SessionManager.Instance.SessionList)
            {
                if (item.Value.SessionStart.AddMinutes(SessionManager.Instance.expiredTime) < DateTime.Now)
                {
                    SessionManager.Instance.SessionList.TryRemove(item.Key, out tmpSession);
                }
            }
        }

        private static void ListTokensDebug()
        {
            Debug.WriteLine("Lista de Tokens");
            foreach (KeyValuePair<string, SessionInfo> item in SessionManager.Instance.SessionList)
            {
                Debug.WriteLine(item.Key);
            }
        }
    }

}