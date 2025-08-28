using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SisConAxs.Session;
using SisConAxs.Services;
using SisConAxs_DM.Repository;
using SisConAxs_DM.DTO;
using SisConAxs_DM.DTO.Response;
using SisConAxs.Integration;

namespace SisConAxs.Controllers
{
    public class UsersController : ApiController
    {
        public IQueryable<AccessUserDTO> GetUsers(HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);

            var query = new AccessUsersRepository().GetAccessUsers();
            //if (query.Count() == 0)
            //{
            //    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
            //    message.Content = new StringContent("No se encontró ningún registro");
            //    throw new HttpResponseException(message);
            //}
            return query;
        }

        [Route("api/UserPaginate")]
        [HttpGet]
        public PaginationResponse<AccessUserDTO> GetUserPaginate([FromUri]AccessUserFilter filter, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            var query = new AccessUsersRepository().GetAccessUsersPaginate(filter);
            return query;
        }

        [HttpGet]
        [Route("api/Users/AD/")]
        public IQueryable<AccessUserDTO> GetUsersAD([FromUri]AccessUserDTO filter, HttpRequestMessage msg) //GetUsersAD([FromUri]FilterUsersAD filter, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            IQueryable<AccessUserDTO> query = null;
            try
            {
                query = new AccessUsersRepository().GetAccessUsersAD(filter, session.SessionUser); //GetAccessUsersAD(filter, session.SessionUser);
            }
            catch (Exception ex)
            {
                LogManager.Error("Error al listar a los usuarios.", ex);
            }
            if (query == null || query.Count() == 0)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent("No se encontró ningún registro");
                throw new HttpResponseException(message);
            }
            return query;
        }


        [HttpGet]
        [Route("api/Users/findUserAD/")]
        public IQueryable<AccessUserDTO> FindUserAD([FromUri]AccessUserDTO filter, HttpRequestMessage msg) //GetUsersAD([FromUri]FilterUsersAD filter, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            //Si no tiene letra ponerle una D
            filter.UserCode = filter.UserCode.ToUpper();
            //string primerCaracter=filter.UserCode.Substring(0, 1);

            //if (!primerCaracter.Equals("D") && !primerCaracter.Equals("E"))
            //{
            //    filter.UserCode = "D" + filter.UserCode;
            //}
            IQueryable<AccessUserDTO> query = GetUsersAD(filter, msg);

            //comprobar que solo exista 1 resultado
            if (query == null || query.Count() == 0)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent("No se encontró el registro.");
                throw new HttpResponseException(message);
            }
            if (query == null || query.Count() > 1)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent("Se encontraron multiples registros.");
                throw new HttpResponseException(message);
            }

            return query;
        }

        [Route("api/Users/{id}")]
        public AccessUserDTO Get(int id, HttpRequestMessage msg)
        {
            //List<int> approvedProfiles = new List<int>();
            //approvedProfiles.Add((int)SITBaseApiController.ROL.Admin);
            SessionInfo session = SessionManager.ValidateSession(msg); //, approvedProfiles);
            try
            {
                return new AccessUsersRepository().GetAccessUserById(id);
            }
            catch (Exception ex)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                message.Content = new StringContent(ex.Message);
                throw new HttpResponseException(message);
            }
        }

        // POST api/<controller>
        public AccessUserDTO Post([FromBody]AccessUserDTO user, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.SYSADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);            
            try
            {
                var accessUser = new AccessUsersRepository().SaveAccessUser(user, session.SessionUser);
                new IntegrationResourceIcarusAccess().SyncResource(accessUser);
                return accessUser;
            }
            catch (Exception ex)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                message.Content = new StringContent(ex.Message);
                throw new HttpResponseException(message);
            }
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        //[Route("api/Users/{id}")]
        //public void Delete(int id, HttpRequestMessage msg)
        //{
        //    List<int> approvedProfiles = new List<int>();
        //    approvedProfiles.Add(UserRole.ADMIN);
        //    SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
        //    try
        //    {
        //        new AccessUsersRepository().DeleteAccessUser(id);
        //    }
        //    catch (Exception ex)
        //    {
        //        HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        //        message.Content = new StringContent(ex.Message);
        //        throw new HttpResponseException(message);
        //    }
        //}

        [HttpPost]
        [Route("api/Users/SysAdmin")]
        public void SetSysAdmin([FromBody] AccessUserDTO user, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.SYSADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            new AccessUsersRepository().SetSysAdmin(user.UserInternalID, user.UserRoleSysAdmin > 0, session.SessionUser.UserInternalID);
        }

    }
}
