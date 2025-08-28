using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using SisConAxs_DM.Repository;
using SisConAxs_DM.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs.Integration
{
    public class IntegrationResourceIcarusAccess: IIntegrationIcarusAccess
    {
        public const int ACTION_ID = 3;
        private static Dictionary<string, string> roleMapping = new Dictionary<string, string>()
        {
            {"Solicitante", "UserRole1"},
            {"Aprobador", "UserRole2"},
            {"Administrador", "UserRole3"},
            {"Crear Personas", "UserRole4"},
            {"Dar de baja", "UserRole5"},
            {"Ver solicitudes proyecto", "UserRole6"},
            {"Reportes", "UserRole7"}
        };
        private AccessUsersRepository AccessUsersRepository;
        private AccessResourcesRepository AccessResourcesRepository;
        private PeopleRepository PeopleRepository;
        private ResourcePeopleRepository ResourcePeopleRepository;        
        private const string USER_ID = "integration.icarus";
        private SessionData USER_SESSIONDATA = new SessionData()
        {
            User = new AccessUserDTO() { UserFirstName = "Intergración Icarus", UserLastName = "Intergración Icarus" },
            sessionUser = USER_ID,
            UserRole3 = 1,        // ADMIN
            UserRoleSysAdmin = 1  // SYSADMIN
        };
        public IntegrationResourceIcarusAccess()
        {
            AccessUsersRepository = new AccessUsersRepository();
            AccessResourcesRepository = new AccessResourcesRepository(USER_SESSIONDATA);
            PeopleRepository = new PeopleRepository(USER_SESSIONDATA);
            ResourcePeopleRepository = new ResourcePeopleRepository();
        }

        public string GetName()
        {
            return "ICARUS";
        }

        public void SyncAccess()
        {
            try
            {                
                var accessUsers = AccessUsersRepository.GetAccessUserQuery().ToList();
                var accessResources = AccessResourcesRepository.GetAccessResourceWithParametersByCategoryId(Constants.CATEGORY_SISTEMAS_INFORMACION_ALTAS_USUARIOS)
                    .Where(ar => ar.ResourceParameters.Count > 0)
                    .ToList();
                var peopleList = PeopleRepository.GetPeopleQuery(p => !string.IsNullOrEmpty(p.UserID)).ToList();

                foreach (var accessUser in accessUsers)
                {
                    var people = peopleList.Find(p => p.UserID.ToUpper() == accessUser.UserInternalID.ToUpper());
                    if (people == null) continue;
                    
                    if (accessUser.UserCompanies.Count == 0 && accessUser.UserStatus == 0)
                    {
                        var peopleResources = ResourcePeopleRepository.GetByPeopleID(people.PeopleID).Where(pr => pr.PresActive == 1).Select(pr => pr.ResourceID).ToList();
                        var resourcesToDelete = accessResources.Where(ar => peopleResources.Contains(ar.ResourceID)).ToList();

                        foreach (var resourceToDelete in resourcesToDelete)
                        {
                            try
                            {
                                var rp = PrepareSync(people, resourceToDelete, AccessRequestDTO.TYPE_BAJA);
                                ResourcePeopleRepository.SaveResourcePeople(rp, AccessRequestDTO.TYPE_BAJA);
                            }
                            catch (Exception ex)
                            {
                                LogManager.Error($"> Integration.IcarusAccess [SyncFromIcarus]>> Error de sincronización al dar de baja al recurso '{resourceToDelete.ResourceName}' para el destinatario '{people.PeopleFullname}'.", ex);
                            }
                        }
                        continue;
                    }

                    foreach (var userCompany in accessUser.UserCompanies)
                    {
                        var resources = accessResources.FindAll(r => r.ResourceCompany == userCompany.CompanyID).ToList();
                        foreach (var roleField in roleMapping)
                        {
                            int roleValue = (int)userCompany.GetType().GetProperty(roleField.Value).GetValue(userCompany);
                            var resource = resources.Find(r => r.ResourceParameters
                                                                    .Any(p => p.Value.Contains(roleField.Value)));
                            if (resource == null) continue;

                            try
                            {
                                var hasResource = ResourcePeopleRepository.GetByPeopleID(people.PeopleID)
                                                                            .Where(pr => pr.ResourceID == resource.ResourceID && pr.PresActive == 1)
                                                                            .FirstOrDefault();

                                if (roleValue == 1 && hasResource == null)
                                {
                                    var rp = PrepareSync(people,resource, AccessRequestDTO.TYPE_ALTA);
                                    ResourcePeopleRepository.SaveResourcePeople(rp, AccessRequestDTO.TYPE_ALTA);
                                }
                                else if (roleValue == 0 && hasResource != null)
                                {
                                    var rp = PrepareSync(people, resource, AccessRequestDTO.TYPE_BAJA);
                                    ResourcePeopleRepository.SaveResourcePeople(rp, AccessRequestDTO.TYPE_BAJA);
                                }
                            }
                            catch (Exception ex)
                            {
                                LogManager.Error($"> Integration.IcarusAccess [SyncFromIcarus]>> Error de sincronización al {(roleValue == 1 ? "grabar nuevo": "dar de baja al")} recurso '{resource.ResourceName}' para el destinatario '{people.PeopleFullname}'.", ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"> Integration.IcarusAccess [SyncFromIcarus]>> Error al actualizar los datos desde Icarus. {ex.Message}", ex);
            }
        }

        public void SyncResource(AccessUserDTO accessUser)
        {
            try
            {
                var accessResources = AccessResourcesRepository.GetAccessResourceWithParametersByCategoryId(Constants.CATEGORY_SISTEMAS_INFORMACION_ALTAS_USUARIOS)
                                                                .Where(ar => ar.ResourceParameters.Count > 0)
                                                                .ToList();

                var people = PeopleRepository.GetPeopleByUserId(accessUser.UserInternalID);
                if (people == null) return;

                if (accessUser.UserCompanies.Count > 0 && accessUser.UserStatus == 1)
                {
                    List<int> listCompanyAccess = accessUser.UserCompanies.Select(uc => uc.CompanyID).ToList();
                    var peopleResourcesToDelete = ResourcePeopleRepository.GetByPeopleID(people.PeopleID)
                                                                            .Where(pr => !listCompanyAccess.Contains(pr.PresCompany) && pr.PresActive == 1)
                                                                            .Select(pr => pr.ResourceID).ToList();
                    var resourcesToDelete = accessResources.FindAll(ar => peopleResourcesToDelete.Contains(ar.ResourceID));

                    foreach (var resourceToDelete in resourcesToDelete)
                    {
                        try
                        {
                            var rp = PrepareSync(people, resourceToDelete, AccessRequestDTO.TYPE_BAJA);
                            ResourcePeopleRepository.SaveResourcePeople(rp, AccessRequestDTO.TYPE_BAJA);
                        }
                        catch (Exception ex)
                        {
                            LogManager.Error($"> Integration.IcarusAccess [SyncFromIcarus]>> Error de sincronización al dar de baja al recurso '{resourceToDelete.ResourceName}' para el destinatario '{people.PeopleFullname}'.", ex);
                        }
                    }

                    foreach (var userCompany in accessUser.UserCompanies)
                    {
                        var resources = accessResources.FindAll(r => r.ResourceCompany == userCompany.CompanyID).ToList();
                        foreach (var roleField in roleMapping)
                        {
                            try
                            {
                                int roleValue = (int)userCompany.GetType().GetProperty(roleField.Value).GetValue(userCompany);
                                var resource = resources.Find(r => r.ResourceParameters.Any(rp => rp.Value.Contains(roleField.Value)));
                                if (resource == null) continue;

                                var hasResource = ResourcePeopleRepository.GetByPeopleID(people.PeopleID)
                                                                            .Where(pr => pr.ResourceID == resource.ResourceID && pr.PresActive == 1)
                                                                            .FirstOrDefault();
                                if (roleValue == 1 && hasResource == null)
                                {
                                    var rp = PrepareSync(people, resource, AccessRequestDTO.TYPE_ALTA);
                                    ResourcePeopleRepository.SaveResourcePeople(rp, AccessRequestDTO.TYPE_ALTA);
                                }
                                if (roleValue == 0 && hasResource != null)
                                {
                                    var rp = PrepareSync(people, resource, AccessRequestDTO.TYPE_BAJA);
                                    ResourcePeopleRepository.SaveResourcePeople(rp, AccessRequestDTO.TYPE_BAJA);
                                }
                            }
                            catch (Exception ex)
                            {
                                LogManager.Error($"> Integration.IcarusAccess [SyncFromIcarus]>> Error de sincronización al grabar nuevo recurso '{roleField.Key}' para el destinatario '{people.PeopleFullname}'.", ex);
                            }
                        }
                    }
                }
                else if(accessUser.UserCompanies.Count == 0 && accessUser.UserStatus == 0)
                {
                    var peopleResources = ResourcePeopleRepository.GetByPeopleID(people.PeopleID).Select(pr => pr.ResourceID).ToHashSet();
                    var resourcesToDelete = accessResources.Where(ar => peopleResources.Contains(ar.ResourceID)).ToList();                    

                    foreach (var resourceToDelete in resourcesToDelete)
                    {
                        try
                        {
                            var rp = PrepareSync(people, resourceToDelete, AccessRequestDTO.TYPE_BAJA);
                            ResourcePeopleRepository.SaveResourcePeople(rp, AccessRequestDTO.TYPE_BAJA);
                        }
                        catch (Exception ex)
                        {
                            LogManager.Error($"> Integration.IcarusAccess [SyncFromIcarus]>> Error de sincronización al dar de baja al recurso '{resourceToDelete.ResourceName}' para el destinatario '{people.PeopleFullname}'.", ex);
                        }                        
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"> Integration.IcarusAccess [SyncFromIcarus]>> Error al actualizar los datos desde Icarus. {ex.Message}", ex);
            }
        }

        private ResourcePeople PrepareSync(PeopleDTO people, AccessResourcesDTO resource, int accessRequestType)
        {
            var rp = new ResourcePeople
            {
                PeopleID = people.PeopleID,
                ResourceID = resource.ResourceID,
                PeopleDepartment = people.PeopleDepartment,
                ResourceFullName = resource.ResourceFullName,
                PresTemporal = AccessResourcesDTO.TEMPORAL_NO,
                PresCompany = resource.ResourceCompany,
                EditUser = USER_ID
            };

            rp.ResourcePeopleLog.Add(new ResourcePeopleLog()
            {
                Action = accessRequestType,
                Source = "ICARUS",
                Description = "Registro creado desde la sincronización de Icarus."
            });

            return rp;
        }

        public void SavePeopleAccess(WorkflowExecution wfExec, AccessRequestDetails requestDetail, Action<string, AccessRequestDetails> OnSuccess, Action<IntegrationResourceException> OnError)
        {
            if (requestDetail.RequestDetType == AccessRequestDTO.TYPE_ALTA)
            {
                var parameter = requestDetail.AccessResources.ResourceParameters.FirstOrDefault(x => x.ResourceParameterID == AccessResourceParameterDTO.INTEGRACION_ICARUS_ACCESS);
                if (parameter == null) throw new IntegrationResourceException(IntegrationResourceException.ERROR_RESOURCE_WITHOUT_EXTERNAL_ID, $"> Integration.IcarusAccess >> El recurso '{requestDetail.AccessResources.ResourceFullName}' NO tiene vinculado un acceso de Icarus.");
                try
                {
                    int roleValue = 1;
                    string roleAcess = parameter.Value.Split('-')[0];
                    int peopleId = requestDetail.AccessRequest.RequestTo;
                    string user = PeopleRepository.GetPeopleById(peopleId).Select(p => p.UserID).FirstOrDefault();
                    var accessUser = AccessUsersRepository.GetAccessUserQuery(au => au.UserInternalID == user).FirstOrDefault();

                    bool success = SaveRoleAccessFromRequest(roleAcess, roleValue, accessUser, requestDetail);
                    if (success)
                    {
                        OnSuccess("Se asignó el rol correctamente", requestDetail);
                    }
                    else
                    {
                        OnError(new IntegrationResourceException(IntegrationResourceException.ERROR_SERVER, "> Integration.IcarusAccess >> Error al grabar la ALTA del acceso en Icarus."));
                    }
                }
                catch (Exception ex)
                {
                    OnError(new IntegrationResourceException(IntegrationResourceException.ERROR_OTHER, "> Integration.IcarusAccess >> Error al grabar la ALTA del acceso en Icarus.", ex));
                }
            }
        }

        public void DeletePeopleAccess(WorkflowExecution wfExec, AccessRequestDetails requestDetail, Action<string, AccessRequestDetails> OnSuccess, Action<IntegrationResourceException> OnError)
        {
            if (requestDetail.RequestDetType == AccessRequestDTO.TYPE_BAJA)
            {
                var parameter = requestDetail.AccessResources.ResourceParameters.FirstOrDefault(x => x.ResourceParameterID == AccessResourceParameterDTO.INTEGRACION_ICARUS_ACCESS);
                if (parameter == null) throw new IntegrationResourceException(IntegrationResourceException.ERROR_RESOURCE_WITHOUT_EXTERNAL_ID, $"> Integration.IcarusAccess >> El recurso '{requestDetail.AccessResources.ResourceFullName}' NO tiene vinculado un acceso de Icarus.");
                try
                {
                    int roleValue = 0;
                    string roleAcess = parameter.Value.Split('-')[0];
                    int peopleId = requestDetail.AccessRequest.RequestTo;
                    string user = PeopleRepository.GetPeopleById(peopleId).Select(p => p.UserID).FirstOrDefault();
                    var accessUser = AccessUsersRepository.GetAccessUserQuery(au => au.UserInternalID == user).FirstOrDefault();

                    bool success = SaveRoleAccessFromRequest(roleAcess, roleValue, accessUser, requestDetail);
                    if (success)
                    {
                        OnSuccess("Se quitó el rol correctamente", requestDetail);
                    }
                    else
                    {
                        OnError(new IntegrationResourceException(IntegrationResourceException.ERROR_SERVER, "> Integration.IcarusAccess >> Error al grabar la BAJA del acceso en Icarus."));
                    }
                }
                catch (Exception ex)
                {
                    OnError(new IntegrationResourceException(IntegrationResourceException.ERROR_OTHER, "> Integration.IcarusAccess >> Error al grabar la BAJA del acceso en Icarus.", ex));
                }
            }
        }

        private bool SaveRoleAccessFromRequest(string roleAccess, int roleValue, AccessUserDTO accessUser, AccessRequestDetails requestDetail)
        {
            var companyRepository = new CompanyRepository();
            var company = companyRepository.GetCompanyById(requestDetail.AccessRequest.RequestCompany);
            var listToRemove = new List<AccessUserCompanyDTO>();
            bool existCompanyAccess = accessUser.UserCompanies.Any(uc => uc.CompanyID == company.CompanyID);

            if (!existCompanyAccess)
            {
                var addCompanyAccess = new AccessUserCompanyDTO()
                {
                    CompanyID = company.CompanyID,
                    UserID = accessUser.UserID,
                    CompanyActive = company.CompanyActive,
                    CompanyDisplay = company.CompanyDisplay,
                    CompanyName = company.CompanyName
                };
                accessUser.UserCompanies.Add(addCompanyAccess);
            }

            try
            {
                foreach (var userCompany in accessUser.UserCompanies)
                {
                    if (userCompany.CompanyID == company.CompanyID)
                    {
                        SetCompanyRole(roleAccess, roleValue, userCompany);
                    }

                    if (userCompany.GetPermissions() == 0)
                    {
                        listToRemove.Add(userCompany);
                    }
                }

                foreach (var item in listToRemove)
                {
                    accessUser.UserCompanies.Remove(item);
                }

                // Si no tiene ningún acceso, se inactiva al usuario
                if (accessUser.UserCompanies.Count == 0)
                {
                    accessUser.UserStatus = 0;
                }

                // Si tiene accesos por asignar y su estado es inactivo, se deberá activar al usuario
                if (accessUser.UserCompanies.Count > 0 && accessUser.UserStatus == 0)
                {
                    accessUser.UserStatus = 1;
                }

                var result = AccessUsersRepository.SaveAccessUser(accessUser, new AccessUsers() { EditUser = USER_ID, UserInternalID = USER_ID, UserRoleSysAdmin = 1 });
                if (result != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                string roleKey = null;
                foreach (var item in roleMapping)
                {
                    if (item.Value == roleAccess)
                    {
                        roleKey = item.Key;
                    }
                }
                LogManager.Error($"> Integration.IcarusAccess [SyncFromIcarus]>> Error de sincronización al {(roleValue == 1 ? "dar" : "dar de baja")} el acceso '{roleKey}' para el usuario '{accessUser.UserInternalID}' en la empresa {company.CompanyName}.", ex);
                return false;
            }

        }

        private void SetCompanyRole(string roleAcess, int roleValue, AccessUserCompanyDTO accessUserCompany)
        {
            switch (roleAcess)
            {
                case "UserRole1":
                    accessUserCompany.UserRole1 = roleValue;
                    break;
                case "UserRole2":
                    accessUserCompany.UserRole2 = roleValue;
                    break;
                case "UserRole3":
                    accessUserCompany.UserRole3 = roleValue;
                    break;
                case "UserRole4":
                    accessUserCompany.UserRole4 = roleValue;
                    break;
                case "UserRole5":
                    accessUserCompany.UserRole5 = roleValue;
                    break;
                case "UserRole6":
                    accessUserCompany.UserRole6 = roleValue;
                    break;
                case "UserRole7":
                    accessUserCompany.UserRole7 = roleValue;
                    break;
            }
        }
    }
}
