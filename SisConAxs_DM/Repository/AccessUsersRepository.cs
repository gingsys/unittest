using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using AutoMapper;
using SisConAxs_DM.Utils;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Linq.Expressions;
using SisConAxs_DM.DTO.Response;

namespace SisConAxs_DM.Repository
{
    public class FilterUsersAD
    {
        public string ADdomain { get; set; }
    }
    public class AccessUserFilter
    {
        public string UserInternalID { get; set; }
        public string UserLastName { get; set; }
        public string UserFirstName { get; set; }
        public string UserStatusDesc { get; set; }
        public int status { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
    public class CompanyADFilter
    {
        public string companyAD { get; set; }

    }


    public class AccessUsersRepository : AxsBaseRepository
    {

        public AccessUsersRepository()
        {
            dbSet = db.AccessUsers;
        }

        /// <summary>Llamada para obtener la lista de AccesTypes.</summary>
        /// <para>GET api/AccessTypes </para>
        /// <returns>Lista de AccesType</returns>
        public IQueryable<AccessUserDTO> GetAccessUsers()
        {
            var query = from au in db.AccessUsers
                        orderby
                          au.UserInternalID
                        select new AccessUserDTO()
                        {
                            UserID = au.UserID,
                            UserInternalID = au.UserInternalID,
                            UserLastName = au.UserLastName,
                            UserFirstName = au.UserFirstName,
                            UserPassword = au.UserPassword,
                            UserRole1 = au.UserRole1,
                            UserRole2 = au.UserRole2,
                            UserRole3 = au.UserRole3,
                            UserRole4 = au.UserRole4,
                            UserRole5 = au.UserRole5,
                            UserRole6 = au.UserRole6,
                            //UserRole7 = au.UserRole7,
                            UserRoleSysAdmin = au.UserRoleSysAdmin,
                            UserStatus = au.UserStatus
                        };
            return query;
        }

        /// <summary>Llamada para obtener la lista de AccesUsers Paginado.</summary>
        /// <para>GET api/AccessTypes </para>
        /// <returns>Lista de AccesType</returns>
        public PaginationResponse<AccessUserDTO> GetAccessUsersPaginate(AccessUserFilter filter)
        {
            var query = from au in db.AccessUsers
                        orderby
                          au.UserInternalID
                        select new AccessUserDTO()
                        {
                            UserID = au.UserID,
                            UserInternalID = au.UserInternalID,
                            UserLastName = au.UserLastName,
                            UserFirstName = au.UserFirstName,
                            UserPassword = au.UserPassword,
                            UserRole1 = au.UserRole1,
                            UserRole2 = au.UserRole2,
                            UserRole3 = au.UserRole3,
                            UserRole4 = au.UserRole4,
                            UserRole5 = au.UserRole5,
                            UserRole6 = au.UserRole6,
                            //UserRole7 = au.UserRole7,
                            UserRoleSysAdmin = au.UserRoleSysAdmin,
                            UserStatus = au.UserStatus,
                            UserStatusDesc = au.UserStatus == 0 ? "Inactivo" : "Activo"
                        };

            // table filters
            if (!String.IsNullOrWhiteSpace(filter.UserInternalID))
            {
                query = query.Where(x => x.UserInternalID.ToString().Contains(filter.UserInternalID));
            }
            if (!String.IsNullOrWhiteSpace(filter.UserLastName))
            {
                query = query.Where(x => x.UserLastName.ToString().Contains(filter.UserLastName));
            }
            if (!String.IsNullOrWhiteSpace(filter.UserFirstName))
            {
                query = query.Where(x => x.UserFirstName.ToString().Contains(filter.UserFirstName));
            }
            if (!String.IsNullOrWhiteSpace(filter.UserStatusDesc))
            {
                query = query.Where(x => x.UserStatusDesc.ToString().Equals(filter.UserStatusDesc));
            }

            // response
            PaginationResponse<AccessUserDTO> response = new PaginationResponse<AccessUserDTO>();
            int count = query.Count();
            if (count > 0)
            {
                response.TotalRows = count;
                if (filter.CurrentPage > 0 && filter.PageSize > 0)
                {
                    query = query.Skip((filter.CurrentPage - 1) * filter.PageSize).Take(filter.PageSize);
                }
                response.Rows = query.ToList();
            }
            return response;
        }

        public IQueryable<AccessUserDTO> GetAccessUsersAD(AccessUserDTO filter, AccessUsers sessionUser) //GetAccessUsersAD(FilterUsersAD filter, AccessUsers sessionUser)
        {
            try
            {
                string ADdomain = new SystemConfigRepository().GetADdomainConfig().NotifConfHost;
                string ADfilter = new SystemConfigRepository().GetADfilterConfig().NotifConfHost;
                if (filter != null)
                    ADfilter += filter.GetADFilter();

                GyM.Security.ActiveDirectory ad = new GyM.Security.ActiveDirectory();
                System.Data.DataTable dt = null;
                try
                {
                    dt = ad.GetUsers(ADdomain, GyM.Security.ActiveDirectory.Filter.Nombre, "*", null, null, ADfilter); // for CONCAR
                }
#pragma warning disable CS0168 // La variable 'ex' se ha declarado pero nunca se usa
                catch (Exception ex)
#pragma warning restore CS0168 // La variable 'ex' se ha declarado pero nunca se usa
                {
                    //LogManager.Error($"AD Error - 01 => [{filter.UserInternalID}] - {ex.Message}", ex);
                    dt = ad.GetUsers(ADdomain, GyM.Security.ActiveDirectory.Filter.Nombre, "*", sessionUser.UserInternalID, sessionUser.UserPassword, ADfilter);
                }
                var query = from users in dt.Select()
                            orderby
                              users["samAccountName"].ToString()
                            select new AccessUserDTO()
                            {
                                UserID = 0,
                                UserInternalID = users["samAccountName"].ToString(),
                                UserCode = users["postalCode"].ToString(),
                                UserLastName = users["sn"].ToString(),
                                UserFirstName = users["givenName"].ToString(),
                                UserEMail = users["mail"].ToString(),
                                UserDocNum = users["postalCode"].ToString(),
                                UserPhone1 = users["mobile"].ToString(),
                                UserCompany = users["company"].ToString(),
                                UserStatus = 1,
                                UserPrincipalName = users["userPrincipalName"].ToString()
                            };
                return SetUserValues(query);
            }
            catch (Exception ex)
            {
                throw new ModelException(ex.Message, ex);
            }
        }


        private IQueryable<AccessUserDTO> SetUserValues(IEnumerable<AccessUserDTO> query)
        {
            var CommonValueSet = db.CommonValueSets.FirstOrDefault(t => t.CommonValueSetName == "TIPO_DOCUMENTO");
            CommonValues DNI = CommonValueSet.CommonValues.FirstOrDefault(t => t.CommonValueName == "DNI");
            CommonValues PASAPORTE = CommonValueSet.CommonValues.FirstOrDefault(t => t.CommonValueName == "PASAPORTE");
            CommonValues CE = CommonValueSet.CommonValues.FirstOrDefault(t => t.CommonValueName == "C_E");

            CommonValues currentDocType;
            string DocTypeCode;
            var list = query.ToList();
            foreach (var dto in list)
            {
                DocTypeCode = !String.IsNullOrWhiteSpace(dto.UserDocNum) ? dto.UserDocNum.Substring(0, 1) : "";
                switch (DocTypeCode)
                {
                    case "D":
                        currentDocType = DNI;
                        break;
                    case "E":
                        currentDocType = CE;
                        break;
                    default:
                        currentDocType = null;
                        break;
                }

                if (currentDocType != null)
                {
                    dto.UserDocType = currentDocType.CommonValueID;
                    dto.UserDocTypeDisplay = currentDocType.CommonValueDisplay;
                    dto.UserDocNum = dto.UserDocNum.Substring(1, dto.UserDocNum.Length - 1);
                }
                else
                {
                    dto.UserDocType = 0;
                    dto.UserDocTypeDisplay = "";
                    dto.UserDocNum = "";
                }
            }

            return list.AsQueryable<AccessUserDTO>();
        }


        public IQueryable<AccessUserDTO> GetAccessUserQuery(Expression<Func<AccessUserDTO, bool>> whereExpr = null)
        {
            var query = from au in db.AccessUsers
                        select new AccessUserDTO()
                        {
                            UserID = au.UserID,
                            UserInternalID = au.UserInternalID,
                            UserLastName = au.UserLastName,
                            UserFirstName = au.UserFirstName,
                            UserPassword = au.UserPassword,
                            UserRole1 = au.UserRole1,
                            UserRole2 = au.UserRole2,
                            UserRole3 = au.UserRole3,
                            UserRole4 = au.UserRole4,
                            UserRole5 = au.UserRole5,
                            UserRole6 = au.UserRole6,
                            //UserRole7 = au.UserRole7,
                            UserRoleSysAdmin = au.UserRoleSysAdmin,
                            UserStatus = au.UserStatus,
                            UserCompanies = (
                                from company in db.Companies
                                join detail in au.UserCompanies on company.CompanyID equals detail.CompanyID
                                select new AccessUserCompanyDTO()
                                {
                                    CompanyID = company.CompanyID,
                                    CompanyName = company.CompanyName,
                                    CompanyDisplay = company.CompanyDisplay,
                                    CompanyActive = company.CompanyActive,
                                    UserID = detail.UserID,
                                    UserRole1 = detail.UserRole1,
                                    UserRole2 = detail.UserRole2,
                                    UserRole3 = detail.UserRole3,
                                    UserRole4 = detail.UserRole4,
                                    UserRole5 = detail.UserRole5,
                                    UserRole6 = detail.UserRole6,
                                    UserRole7 = detail.UserRole7
                                }
                            ).ToList()
                        };
            if (whereExpr != null)
            {
                query = query.Where(whereExpr);
            }
            return query;
        }

        public AccessUserDTO GetAccessUserById(int id)
        {
            return this.GetAccessUserQuery(u => u.UserID == id).FirstOrDefault();
        }

        public List<AccessUserDTO> UpdateLastAccessUsers(string aliasUser)
        {
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@USER_ALIAS", SqlDbType.VarChar) { Value = aliasUser }
            };
            var fechasLastAccess = this.Select<AccessUserDTO>("AXSCONTROL.SP_ACCESS_USERS_LAST_ACCESS", parameters);
            return fechasLastAccess;
        }

        public AccessUserDTO SaveAccessUser(AccessUserDTO dto, AccessUsers userEditor)
        {
            try
            {
                AccessUsers model = null;
                if (dto.UserID == 0)
                    model = db.AccessUsers.Create();  // create new from context
                else
                {
                    model = db.AccessUsers.FirstOrDefault(a => a.UserID == dto.UserID);  // get from context
                    dto.LastAccessUser = model.LastAccessUser;
                    if (userEditor.UserRoleSysAdmin <= 0)  // si no es SysAdmin no puede editar el estado
                    {
                        dto.UserStatus = model.UserStatus;
                    }
                }

                Mapper.Map<AccessUserDTO, AccessUsers>(dto, model);
                model.EditUser = userEditor.UserInternalID;
                //if (dto.LastAccessUser == null) // "1/01/0001 00:00:00")
                //{
                //    dto.LastAccessUser = DateTime.Now;
                //}
                //dto.LastAccessUser = userEditor.LastAccessUser;

                if (Validate(model, dto))
                {
                    PrepareDetail(model, dto);
                    if (SaveEntity(model.UserID == 0, model))
                    {
                        dto = AutoMapper.Mapper.Map<AccessUserDTO>(model);
                        return dto;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ModelException(ex.Message);
            }
            return null;
        }

        public AccessUserDTO SaveAccessUserFromAuthenticationData(AuthenticationDataDTO authData, AccessUsers userEditor)
        {
            return new AccessUsersRepository().SaveAccessUser(
                authData.ToAccessUserDTO(),         // Nuevo usuario
                new AccessUsers()                   // Usuario creador : Sistema (ICARUS).
                {
                    EditUser = "Sistema",
                    UserInternalID = "icarus"
                }
            );
        }

        private void PrepareDetail(AccessUsers model, AccessUserDTO dto)
        {
            var listToRemove = new List<AccessUserCompanies>();
            //model.LastAccessUser = dto.LastAccessUser;
            // remove detail
            if (dto.UserID > 0)
            {
                foreach (AccessUserCompanies item in model.UserCompanies)
                {
                    if (dto.UserCompanies.Any(y => y.CompanyID == item.CompanyID))
                        listToRemove.Add(item);
                }
                foreach (AccessUserCompanies item in listToRemove)
                {
                    db.AccessUserCompanies.Remove(item);
                    //model.UserCompanies.Remove(item);
                }
            }
            else
            {
                model.UserCompanies.Clear();
            }

            // add detail
            var listToAdd = dto.UserCompanies.Where(x => x.GetPermissions() > 0).Select(x =>
            {
                var item = AutoMapper.Mapper.Map<AccessUserCompanies>(x);
                item.EditUser = model.EditUser;
                return item;
            }).ToList();

            if(model.UserStatus == 0)
            {
                model.UserRoleSysAdmin = 0;
                return;
            }

            foreach (var item in listToAdd)
            {
                model.UserCompanies.Add(item);
            }
        }


        private bool Validate(AccessUsers model, AccessUserDTO dto)
        {
            dto.UserInternalID = dto.UserInternalID.ToUpper();
            dto.UserLastName = dto.UserLastName.ToUpper();
            dto.UserFirstName = dto.UserFirstName.ToUpper();

            //model.LastAccessUser = dto.LastAccessUser;

            // ID
            if (String.IsNullOrEmpty(dto.UserInternalID))
            {
                throw new ModelException("No ha ingresado un ID para este usuario.");
            }

            //// password
            //if (String.IsNullOrEmpty(model.UserPassword))
            //{
            //    throw new ModelException("No ha ingresado una clave para este usuario.");
            //}

            // repeat name
            int count = db.AccessUsers.Count(
                    t => t.UserID != dto.UserID
                         && t.UserInternalID == dto.UserInternalID
                );
            if (count > 0)
            {
                throw new ModelException("Ya existe un usuario registrado con el mismo ID.");
            }

            // sólo se puede tener el rol de aprobador en una sola empresa
            count = model.UserCompanies.Count(c => c.UserRole2 > 0);
            if (count > 1)
            {
                throw new ModelException("Sólo se puede tener el rol de aprobador en una sola empresa.");
            }
            return true;
        }

        public bool DeleteAccessUser(int id)
        {
            if (id == 0)
                throw new ModelException("No ha seleccionado ningún usuario");

            AccessUsers user = db.AccessUsers.FirstOrDefault(a => a.UserID == id);
            if (user != null)
            {
                var userIdParam = new System.Data.SqlClient.SqlParameter("@USER_INTERNAL_ID", user.UserInternalID);
                List<int> usedCount = db.Database.SqlQuery<int>("AXSCONTROL.SP_COUNT_RELATIONS_USERS @USER_INTERNAL_ID", userIdParam).ToList();

                if (usedCount.Count == 1 && usedCount.First() > 0)
                {
                    AccessUsers model = db.AccessUsers.FirstOrDefault(a => a.UserID == id);
                    model.UserStatus = 0;  // desactiva al usuario
                    SaveEntity(model.UserID == 0, model);
                    AccessUserDTO dto = new AccessUserDTO();
                    Mapper.Map<AccessUsers, AccessUserDTO>(model, dto);

                    return false;
                    //throw new ModelException("No se pudo eliminar el usuario, tiene dependencias, sólo se desactivó.");
                }
                else
                {
                    var userCompanies = (from uc in db.AccessUserCompanies
                                         where
                                            uc.UserID == id
                                         select uc
                                        ).ToList();
                    foreach (AccessUserCompanies item in userCompanies)
                    {
                        db.AccessUserCompanies.Remove(item);
                    }

                    return DeleteEntity(user);
                }
            }
            else
            {
                throw new ModelException("No se encontró el usuario");
            }
#pragma warning disable CS0162 // Se detectó código inaccesible
            return false;
#pragma warning restore CS0162 // Se detectó código inaccesible
        }

        public void UpdateStatusAndDeleteRoles(int userID, bool active, string editUser)
        {
            try
            {
                AccessUsers model = db.AccessUsers.FirstOrDefault(a => a.UserID == userID);
                var listToRemove = new List<AccessUserCompanies>();

                if (model == null)
                    throw new ModelException("Usuario no encontrado para el cambio de estado, ID" + userID);
                model.UserStatus = active ? 1 : 0;
                model.EditUser = editUser;

                foreach (AccessUserCompanies item in model.UserCompanies)
                    listToRemove.Add(item);

                foreach (AccessUserCompanies item in listToRemove)
                    db.AccessUserCompanies.Remove(item);
                
                SaveEntity(false, model);
            }
            catch (Exception ex)
            {
                throw new ModelException(ex.Message);
            }
        }
                
        public void SetSysAdmin(string userAlias, bool userStatus, string editUser)
        {
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@USER_ALIAS", SqlDbType.NVarChar) { Value = userAlias },
                new SqlParameter("@USER_SYSADMIN_STATUS", SqlDbType.Bit) { Value = userStatus },
                new SqlParameter("@EDIT_USER", SqlDbType.NVarChar) { Value = editUser }
            };
            this.ExecuteSP("AXSCONTROL.SET_USER_SYSADMIN", parameters);
        }
    }
}
