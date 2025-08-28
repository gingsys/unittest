using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using Newtonsoft.Json;
using SisConAxs_DM.Repository;
using SisConAxs_DM.Utils;
using System.Net;
using AutoMapper;
using SisConAxs.Integration.DTO;
using SisConAxs.Integration.Utils;
using System.Data.Entity;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace SisConAxs.Integration
{
    public class IntegrationResourceOracle : IIntegrationResource<OracleAccessPeopleDTO>
    {
        public const int ACTION_ID = 1;                           // ActionID de WorkflowItem
        public const int ERROR_BD_CONNECTION = 100;               // Error en la conexión con el servidor de la BD Oracle
        public const int ERROR_CREATE_USER = -2000;               // Error al crear Usuario Oracle
        public const int ERROR_USER_NOT_FOUND = -5000;            // Error Usuario Oracle no encontrado
        public const int ERROR_RESPONSABILITY_NOT_FOUND = -5001;  // Error responsabilidad no encontrada  !!!!!!!!!!!!!!!! MODIFICAR EL VALOR LUEGO DE ACTUALIZAR EL PAQUETE ORACLE !!!!!!!!!!!!!!!!!!!!!!!!!
        public const int ERROR_USER_INACTIVE = -60000;            // Usuario Oracle Inactivo

        const string USER_ID = "integration.oracle";

        private MetadataRepository metadataRepository = new MetadataRepository();
        private OracleConfigDTO OracleConfig = null;
        public IntegrationResourceOracle()
        {
#if RELEASE
            OracleConfig = OracleConfigDTO.FromSysConfig(new SystemConfigRepository().GetFromName(SystemConfigDTO.ORACLE_API));
#else
            OracleConfig = new OracleConfigDTO()
            {
                Address = "http://aplicacionestest.aenza.com.pe:8082/api",
                //Address = "http://localhost:8958/api",
                Username = "admin",
                Password = "123456"
            };
#endif
        }

        private Dictionary<string, string> GetToken()
        {
            var header = new Dictionary<string, string>();
            ServerApiRequest request = ServerApiRequest.Create(
                $"{OracleConfig.Address}/login/authenticate", ServerApiRequest.HTTP_METHOD_POST,
                $"{{ Username: \"{OracleConfig.Username}\", Password: \"{OracleConfig.Password}\" }}"
            );
            request.Send((response) =>
            {
                header.Add("Authorization", response.Trim().Substring(1, response.Length - 2));
            },
            (ex, jsonMessage) =>
            {
                LogManager.Error($"{JsonConvert.SerializeObject(OracleConfig)} => {ex.Message}");
                throw ex;
            });
            return header;
        }
        public string GetName()
        {
            return "ORACLE";
        }

        // Check Responsabilities Conflitcs -------------------------------------------------------------------------------------------------------- //
        public OracleCheckReponsabilityResponse CheckResponsabilityConflicts(string user, List<string> responsabilities)
        {
            var token = this.GetToken();
            var payload = new OracleCheckResponsabilitiesConflictsPayload()
            {
                UserName = user,
                Responsabilities = responsabilities
            };
            var response = new OracleCheckReponsabilityResponse();

            ServerApiRequest request = ServerApiRequest.Create(
                $"{OracleConfig.Address}/responsability/check-conflicts",
                ServerApiRequest.HTTP_METHOD_GET,
                JsonConvert.SerializeObject(payload),
                token
            );
            request.Send((r) =>
            {
                response = JsonConvert.DeserializeObject<OracleCheckReponsabilityResponse>(r);
            },
            (ex, jsonMessage) =>
            {
                throw ex;
            });
            return response;
        }

        public OracleCheckReponsabilityResponse CheckResponsabilityConflicts(AccessRequestDTO request)
        {
            SisConAxsContext db = new SisConAxsContext();
            var people = db.People.FirstOrDefault(p => p.PeopleID == request.RequestTo);
            var responsabilities = (from resource in db.AccessResources
                                     join reqDetails in request.AccessRequestDetails on resource.ResourceID equals reqDetails.ResourceID
                                     join resourceParameter in db.AccessResourceParameters on new { ResourceID = resource.ResourceID, ParameterID = AccessResourceParameterDTO.INTEGRACION_ORACLE_ID } equals
                                                                                              new { ResourceID = resourceParameter.ResourceID, ParameterID = resourceParameter.ResourceParameterID }
                                     join metadataResponsability in db.Metadata on resourceParameter.ResourceParameterMetadataID equals metadataResponsability.MetadataID
                                     //where
                                     //   request.AccessRequestDetails.Any(d => d.ResourceID == resource.ResourceID)
                                     select metadataResponsability.MetadataInt1
                                     ).ToList();
            if(responsabilities.Count() > 0)
            {
                return this.CheckResponsabilityConflicts(people.UserID.Trim(), responsabilities);
            }

            return new OracleCheckReponsabilityResponse()
            {
                Response = true
            };
        }
        // Check Responsabilities Conflitcs -------------------------------------------------------------------------------------------------------- //


        // Get METADATA ---------------------------------------------------------------------------------------------------------------------------- //
        private List<OracleCompanyDTO> GetCompanies(Dictionary<string, string> token)
        {
            var list = new List<OracleCompanyDTO>();
            ServerApiRequest request = ServerApiRequest.Create($"{OracleConfig.Address}/company", ServerApiRequest.HTTP_METHOD_GET, null, token);
            request.Send((response) =>
            {
                list = JsonConvert.DeserializeObject<List<OracleCompanyDTO>>(response);
            },
            (ex, jsonMessage) =>
            {
                throw ex;
            });
            return list;
        }
        private List<OracleProjectDTO> GetProjects(Dictionary<string, string> token)
        {
            var list = new List<OracleProjectDTO>();
            ServerApiRequest request = ServerApiRequest.Create($"{OracleConfig.Address}/project", ServerApiRequest.HTTP_METHOD_GET, null, token);
            request.Send((response) =>
            {
                list = JsonConvert.DeserializeObject<List<OracleProjectDTO>>(response);
            },
            (ex, jsonMessage) =>
            {
                throw ex;
            });
            return list;
        }
        private List<OracleProfileDTO> GetProfiles(Dictionary<string, string> token)
        {
            var list = new List<OracleProfileDTO>();
            ServerApiRequest request = ServerApiRequest.Create($"{OracleConfig.Address}/profiles", ServerApiRequest.HTTP_METHOD_GET, null, token);
            request.Send((response) =>
            {
                list = JsonConvert.DeserializeObject<List<OracleProfileDTO>>(response);
            },
            (ex, jsonMessage) =>
            {
                throw ex;
            });
            return list;
        }
        private List<OracleResponsabilityDTO> GetResponsabilities(Dictionary<string, string> token)
        {
            var list = new List<OracleResponsabilityDTO>();
            ServerApiRequest request = ServerApiRequest.Create($"{OracleConfig.Address}/responsability", ServerApiRequest.HTTP_METHOD_GET, null, token);
            request.Send((response) =>
            {
                list = JsonConvert.DeserializeObject<List<OracleResponsabilityDTO>>(response);
            },
            (ex, jsonMessage) =>
            {
                throw ex;
            },
            7 * 60); // timeout: 7 min
            return list;
        }
        // Get METADATA ---------------------------------------------------------------------------------------------------------------------------- //


        // Sync METADATA --------------------------------------------------------------------------------------------------------------------------- //
        public void SyncMetadata()
        {
            LogManager.Log($"> Integration.Oracle [SyncMetadata] >> Inicio sincronización de metadata.");
            var datetimeStart = DateTime.Now;

            SisConAxsContext db = new SisConAxsContext();
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // queries ----------------------------------------------------------------
                    var queryCompanies = metadataRepository.GetQuery(x => x.MetadataParentID == MetadataDTO.ORACLE_COMPANIES).ToList();
                    var queryProjects = metadataRepository.GetQuery(x => x.MetadataParentID == MetadataDTO.ORACLE_PROJECTS).ToList();
                    var queryProfiles = metadataRepository.GetQuery(x => x.MetadataParentID == MetadataDTO.ORACLE_PROFILES).ToList();
                    var queryResponsabilities = metadataRepository.GetQuery(x => x.MetadataParentID == MetadataDTO.ORACLE_RESPONSABILITIES).ToList();

                    var token = this.GetToken(); // TOKEN

                    // companies --------------------------------------------------------------
                    LogManager.Log($"> Integration.Oracle [SyncMetadata] >> Inicio sincronización de metadata : empresas.");
                    var companies = this.GetCompanies(token);
                    //companies = companies.FindAll(x => !x.CompanyID.Contains("."));  // PARA QUITAR??? (esto evita agregar las empresas con un punto "." al final) !!!!!!!!!!!!!!!

                    // deactivate all
                    foreach (var dto in queryCompanies)
                    {
                        dto.MetadataActive = 0;
                        metadataRepository.SaveMetadata(db, dto, USER_ID);
                    }
                    // save / activate
                    foreach (var company in companies)
                    {
                        var current = queryCompanies.FirstOrDefault(x => x.MetadataStr1 == company.CompanyID);
                        var dto = company.ToDTO();
                        if (current != null)
                        {
                            dto.MetadataID = current.MetadataID;
                            if (!current.Equals(dto))
                            {
                                metadataRepository.SaveMetadata(db, dto, USER_ID);
                            }
                        }
                        else
                        {
                            metadataRepository.SaveMetadata(db, dto, USER_ID);
                        }
                    }
                    LogManager.Log($"> Integration.Oracle [SyncMetadata] >> Final sincronización de metadata : empresas.");

                    // projects ----------------------------------------------------------------
                    LogManager.Log($"> Integration.Oracle [SyncMetadata] >> Inicio sincronización de metadata : proyectos.");
                    var projects = this.GetProjects(token);
                    // desactiva los que no esten en la lista obtenida desde el API
                    var projectsToDeactivate = queryProjects.FindAll(q => !projects.Any(p => q.MetadataInt1 == p.ProjectID) && q.MetadataActive > 0);
                    foreach (var dto in projectsToDeactivate)
                    {
                        dto.MetadataActive = 0;
                        metadataRepository.SaveMetadata(db, dto, USER_ID);
                    }
                    // save / activate
                    foreach (var company in companies)
                    {
                        var companyProjects = projects.FindAll(r => r.ProjectCompanyID == company.CompanyID);
                        foreach (var project in companyProjects)
                        {
                            var current = queryProjects.FirstOrDefault(x => x.MetadataInt1 == project.ProjectID);
                            var dto = project.ToDTO();
                            if (current != null)
                            {
                                dto.MetadataID = current.MetadataID;
                                if (!current.Equals(dto))
                                {
                                    metadataRepository.SaveMetadata(db, dto, USER_ID);
                                }
                            }
                            else
                            {
                                metadataRepository.SaveMetadata(db, dto, USER_ID);
                            }
                        }
                    }
                    LogManager.Log($"> Integration.Oracle [SyncMetadata] >> Final sincronización de metadata : proyectos.");

                    // profiles ----------------------------------------------------------------
                    LogManager.Log($"> Integration.Oracle [SyncMetadata] >> Inicio sincronización de metadata : perfiles.");
                    var profiles = this.GetProfiles(token);
                    // desactiva los que no esten en la lista obtenida desde el API
                    var profilesToDeactivate = queryProfiles.FindAll(q => !profiles.Any(p => q.MetadataInt1 == p.ProfileID) && q.MetadataActive > 0);
                    foreach (var dto in profilesToDeactivate)
                    {
                        dto.MetadataActive = 0;
                        metadataRepository.SaveMetadata(db, dto, USER_ID);
                    }
                    // save / activate
                    foreach (var profile in profiles)
                    {
                        var current = queryProfiles.FirstOrDefault(x => x.MetadataInt1 == profile.ProfileID);
                        var dto = profile.ToDTO();
                        if (current != null)
                        {
                            dto.MetadataID = current.MetadataID;
                            if (!current.Equals(dto))
                            {
                                metadataRepository.SaveMetadata(db, dto, USER_ID);
                            }
                        }
                        else
                        {
                            metadataRepository.SaveMetadata(db, dto, USER_ID);
                        }
                    }
                    LogManager.Log($"> Integration.Oracle [SyncMetadata] >> Final sincronización de metadata : perfiles.");

                    // responsabilities --------------------------------------------------------
                    LogManager.Log($"> Integration.Oracle [SyncMetadata] >> Inicio sincronización de metadata : responsabilidades.");
                    bool responsabilitiesDone = false;
                    int errorCount = 0;
                    while (!responsabilitiesDone)
                    {
                        try
                        {
                            var responsabilities = this.GetResponsabilities(token);
                            // desactiva los que no esten en la lista obtenida desde el API
                            var responsabilitiesToDeactivate = queryResponsabilities.FindAll(q => !responsabilities.Any(r => q.MetadataInt1 == r.ResponsabilityID) && q.MetadataActive > 0);
                            foreach (var dto in responsabilitiesToDeactivate)
                            {
                                dto.MetadataActive = 0;
                                metadataRepository.SaveMetadata(db, dto, USER_ID);
                            }
                            // save / activate
                            foreach (var company in companies)
                            {
                                var companyResponsabilities = responsabilities.FindAll(r => r.ResponsabilityCompanyID == company.CompanyID);
                                foreach (var responsability in companyResponsabilities)
                                {
                                    var current = queryResponsabilities.FirstOrDefault(x => x.MetadataInt1 == responsability.ResponsabilityID);
                                    var dto = responsability.ToDTO();
                                    if (current != null)
                                    {
                                        dto.MetadataID = current.MetadataID;
                                        if (!current.Equals(dto))
                                        {
                                            metadataRepository.SaveMetadata(db, dto, USER_ID);
                                        }
                                    }
                                    else
                                    {
                                        metadataRepository.SaveMetadata(db, dto, USER_ID);
                                    }
                                }
                            }
                            responsabilitiesDone = true;
                        }
                        catch (Exception ex)
                        {
                            errorCount++;
                            if (errorCount > 3)
                            {
                                throw ex;
                            }
                        }
                    }
                    LogManager.Log($"> Integration.Oracle [SyncMetadata] >> Final sincronización de metadata : responsabilidades.");

                    transaction.Commit();
                    var timeExecution = DateTime.Now.Subtract(datetimeStart);
                    LogManager.Log($"> Integration.Oracle [SyncMetadata] >> Final sincronización de metadata. Operación ejecutada en {timeExecution.TotalMinutes} minutos.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    LogManager.Error($"> Integration.Oracle [SyncMetadata] >> Error de sincronización de metadata.", ex);
                }
            }
        }
        // Sync METADATA --------------------------------------------------------------------------------------------------------------------------- //

        public void SyncResources()
        {
            LogManager.Log($"> Integration.Oracle [SyncResources] >> Inicio sincronización de responsabilidades.");
            var datetimeStart = DateTime.Now;

            SisConAxsContext db = new SisConAxsContext();
            var accessResourcesRepository = new AccessResourcesRepository(null);
            var resourcesMountPoint = db.AccessResourceParameters.Where(p => p.ResourceParameterID == AccessResourceParameterDTO.INTEGRACION_ORACLE_COMPANY_MOUNT);

            foreach (var mountPoint in resourcesMountPoint)
            {
                var resourcePoint = mountPoint.AccessResource;
                var oracleCompanyID = mountPoint.Value;
                var oracleCompanyName = db.Metadata.FirstOrDefault(m => m.MetadataID == mountPoint.ResourceParameterMetadataID).MetadataDisplay;

                try
                {
                    LogManager.Log($"> Integration.Oracle [SyncResources] >> Sincronización de responsabilidades para '{oracleCompanyName}' - OracleCompanyID: {oracleCompanyID}.");

                    var metadataOracle = (from m in db.Metadata
                                          where m.MetadataParentID == MetadataDTO.ORACLE_RESPONSABILITIES
                                                && m.MetadataStr1 == oracleCompanyID  // RUC
                                                && m.MetadataActive > 0
                                          select m).ToList();

                    var resourcesINDIGO = (from r in db.Database.SqlQuery<AccessResourcesDTO>("EXEC AXSCONTROL.SP_GET_RESOURCES_TREE @resourceID = {0}", resourcePoint.ResourceID)
                                           join resourceParameter in db.AccessResourceParameters on r.ResourceID equals resourceParameter.ResourceID
                                           where
                                              resourceParameter.ResourceParameterID == AccessResourceParameterDTO.INTEGRACION_ORACLE_ID
                                           select new AccessResourcesOracleDTO()
                                           {
                                               ResourceID = r.ResourceID,
                                               ResourceOracleID = resourceParameter.Value
                                           }).ToList();

                    var resources = from responsability in metadataOracle
                                    join dataResourceINDIGO in resourcesINDIGO on responsability.MetadataInt1.ToString() equals dataResourceINDIGO.ResourceOracleID into Resources
                                    from resource in Resources.DefaultIfEmpty()
                                    select new
                                    {
                                        MetadataID = responsability.MetadataID,
                                        Responsability = new OracleResponsabilityDTO()
                                        {
                                            ResponsabilityID = responsability.MetadataInt1,
                                            ResponsabilityName = responsability.MetadataDisplay
                                        },
                                        Resource = resource == null ? null : resource
                                    };

                    // nuevos recursos/responsabilidades --------------------------------------------------------------------------------------
                    var itemsAdd = resources.Where(x => x.Resource == null);
                    foreach (var item in itemsAdd)
                    {
                        try
                        {
                            var r = new AccessResourcesDTO();
                            r.ResourceName = item.Responsability.ResponsabilityName;
                            r.ResourceParent = resourcePoint.ResourceID;
                            r.ResourceCategory = resourcePoint.ResourceCategory;
                            r.ResourceAccessType = resourcePoint.ResourceAccessType;
                            r.ResourceTemporal = AccessResourcesDTO.TEMPORAL_OPTIONAL;  // Temporal: opcional
                            r.ResourceActive = 0;
                            r.ResourceCompany = resourcePoint.ResourceCompany;
                            r.ResourceParameters.Add(new AccessResourceParameterDTO()
                            {
                                ResourceParameterID = AccessResourceParameterDTO.INTEGRACION_ORACLE_ID,
                                ResourceParameterMetadataID = item.MetadataID,
                                Value = item.Responsability.ResponsabilityID.ToString()
                            });
                            accessResourcesRepository.SaveAccessResource(r, "integration.sync.oracle");
                        }
                        catch (Exception ex)
                        {
                            LogManager.Error($"> Integration.Oracle [SyncResources] >> Error de sincronización al grabar nuevo recurso OracleResponsability: {item.Responsability.ResponsabilityName} - ID: {item.Responsability.ResponsabilityID}.", ex);
                        }
                    }

                    // desactivar recursos/responsabilidades ----------------------------------------------------------------------------------
                    var itemsDeactivate = resourcesINDIGO.Where(x => !resources.Any(y => y.Resource?.ResourceID == x.ResourceID));
                    foreach (var item in itemsDeactivate)
                    {
                        try
                        {
                            var r = accessResourcesRepository.GetAccessResourceById(item.ResourceID).FirstOrDefault();
                            r.ResourceActive = 0;
                            accessResourcesRepository.SaveAccessResource(r, "integration.sync.oracle");
                        }
                        catch (Exception ex)
                        {
                            LogManager.Error($"> Integration.Oracle [SyncResources] >> Error de sincronización al desactivar recurso: {item.ResourceFullName} - ID: {item.ResourceID}.", ex);
                        }
                    }

                    LogManager.Log($"> Integration.Oracle [SyncResources] >> Final para '{oracleCompanyName}' - OracleCompanyID {oracleCompanyID}.");
                }
                catch (Exception ex)
                {
                    LogManager.Error($"> Integration.Oracle [SyncResources] >> Error de sincronización de responsabilidades para Recurso (punto de montaje) {resourcePoint.ResourceFullName} en '{oracleCompanyName}' - OracleCompanyID {oracleCompanyID}.", ex);
                }
            }

            var timeExecution = DateTime.Now.Subtract(datetimeStart);
            LogManager.Log($"> Integration.Oracle [SyncResources] >> Final sincronización de responsabilidades.  Operación ejecutada en {timeExecution.TotalMinutes} minutos.");
        }

        public List<OracleAccessPeopleDTO> GetPeopleAccess()
        {
            var list = new List<OracleAccessPeopleDTO>();
            var token = GetToken();
            ServerApiRequest request = ServerApiRequest.Create($"{OracleConfig.Address}/access/", ServerApiRequest.HTTP_METHOD_GET, null, token);
            request.Send((response) =>
            {
                list = JsonConvert.DeserializeObject<List<OracleAccessPeopleDTO>>(response);
            },
            (ex, jsonMessage) =>
            {
                throw ex;
            });
            return list;
        }

        public List<OracleAccessPeopleDTO> GetPeopleAccess(Dictionary<string, string> token, string companyID)
        {
            var list = new List<OracleAccessPeopleDTO>();
            ServerApiRequest request = ServerApiRequest.Create($"{OracleConfig.Address}/access/{companyID}", ServerApiRequest.HTTP_METHOD_GET, null, token);
            request.Send((response) =>
            {
                list = JsonConvert.DeserializeObject<List<OracleAccessPeopleDTO>>(response);
            },
            (ex, jsonMessage) =>
            {
                throw ex;
            },
            3 * 60); // 3 min
            return list;
        }

        public List<OracleAccessPeopleDTO> GetPeopleAccess(string companyID, string peopleID)
        {
            var list = new List<OracleAccessPeopleDTO>();
            var token = GetToken();
            //ServerApiRequest request = ServerApiRequest.Create($"{OracleConfig.Address}/access/?companyID={companyID}&peopleID={peopleID}", ServerApiRequest.HTTP_METHOD_GET, null, token);
            ServerApiRequest request = ServerApiRequest.Create($"{OracleConfig.Address}/access/{companyID}/{peopleID}", ServerApiRequest.HTTP_METHOD_GET, null, token);
            request.Send((response) =>
            {
                list = JsonConvert.DeserializeObject<List<OracleAccessPeopleDTO>>(response);
            },
            (ex, jsonMessage) =>
            {
                throw ex;
            });
            return list;
        }

        public void SyncPeopleAccess()
        {
            LogManager.Log($"> Integration.Oracle [SyncPeopleAccess] >> Inicio sincronización de accesos.");
            var datetimeStart = DateTime.Now;

            SisConAxsContext db = new SisConAxsContext();
            var resourcePeopleRepository = new ResourcePeopleRepository();

            var oracleCompanies = db.Metadata.Where(m => m.MetadataParentID == MetadataDTO.ORACLE_COMPANIES)
                .Select(s => new OracleCompanyDTO()
                {
                    CompanyID = s.MetadataStr1,
                    CompanyName = s.MetadataDisplay
                }).ToList();

            var people = (from p in db.People
                          select p.PeopleInternalID).ToList();

            var resources = (from resource in db.AccessResources
                             join resourceParameter in db.AccessResourceParameters on new { ResourceID = resource.ResourceID, ParameterID = AccessResourceParameterDTO.INTEGRACION_ORACLE_ID } equals
                                                                                      new { ResourceID = resourceParameter.ResourceID, ParameterID = resourceParameter.ResourceParameterID }
                             join metadataResponsability in db.Metadata on resourceParameter.ResourceParameterMetadataID equals metadataResponsability.MetadataID
                             select new
                             {
                                 OracleCompanyID = metadataResponsability.MetadataStr1,
                                 OracleResponsabilityID = metadataResponsability.MetadataInt1,
                                 ResourceID = resource.ResourceID,
                                 Resource = resource
                             }).ToList();

            var resourcePeople = (from rp in db.ResourcePeople
                                  join p in db.People on rp.PeopleID equals p.PeopleID
                                  join resource in db.AccessResources on rp.ResourceID equals resource.ResourceID
                                  join resourceParameter in db.AccessResourceParameters on new { ResourceID = resource.ResourceID, ParameterID = AccessResourceParameterDTO.INTEGRACION_ORACLE_ID } equals
                                                                                           new { ResourceID = resourceParameter.ResourceID, ParameterID = resourceParameter.ResourceParameterID }
                                  join metadataResponsability in db.Metadata on resourceParameter.ResourceParameterMetadataID equals metadataResponsability.MetadataID
                                  where
                                     rp.PresActive > 0
                                  select new OracleQueryResourcePeople()
                                  {
                                      OracleCompanyID = metadataResponsability.MetadataStr1,
                                      //OracleResponsabilityID = metadataResponsability.MetadataInt1,
                                      OracleResponsabilityID = metadataResponsability.MetadataInt1,
                                      PeopleInternalID = p.PeopleInternalID,
                                      People = p,
                                      //Resource = resource,
                                      ResourcePeople = rp
                                  }).ToList();

            // registros no coicidentes
            var unmatchPeople = new List<PeopleDTO>();
            var unmatchResponsabilities = new List<OracleResponsabilityDTO>();

            // Se filtran solo las empresas de las responsabilidades vinculadas a un recurso
            oracleCompanies = oracleCompanies.FindAll(c => resources.Any(r => r.OracleCompanyID == c.CompanyID));

            var token = this.GetToken();

            OracleCompanyDTO oracleCompany;
            int errorCount = 0;
            for (int i = 0; i < oracleCompanies.Count;)
            {
                oracleCompany = oracleCompanies[i];
                try
                {
                    LogManager.Log($"> Integration.Oracle [SyncPeopleAccess] >> Sincronización de accesos para '{oracleCompany.CompanyName}' - OracleCompanyID: {oracleCompany.CompanyID}.");

                    var oracleAccesses = GetPeopleAccess(token, oracleCompany.CompanyID); //.FindAll(a => people.Any(p => p == a.AccessUserCode));
                    var resourcePeopleCompany = resourcePeople.FindAll(r => r.OracleCompanyID == oracleCompany.CompanyID);

                    var itemsToRemove = oracleAccesses.Where(item => item.AccessUserCode == null).ToList(); //Se filtran los accesos con AccessUserCode en null

                    foreach (var item in itemsToRemove)
                    {
                        oracleAccesses.Remove(item); //Quitamos de la lista los items con AccessUserCode en null filtrados
                    }

                    var query = oracleAccesses.FullOuterJoinJoin(
                        resourcePeopleCompany,
                        a => $"{a?.AccessUserCode.Trim()}::{a?.AccessResponsabilityID}",
                        b => $"{b?.PeopleInternalID.Trim()}::{b?.OracleResponsabilityID}",
                        (a, b, key) => new
                        {
                            Key = key,
                            PeopleInternalID = a?.AccessUserCode ?? b?.PeopleInternalID,
                            //OracleResponsabilityID = a?.AccessResponsabilityID ?? b?.OracleResponsabilityID,
                            OracleResponsabilityID = a?.AccessResponsabilityID ?? b?.OracleResponsabilityID,

                            OracleAccess = a,
                            IcarusAccess = b
                        })
                        .DistinctBy(x => x.Key)
                        .GroupBy(x => x.PeopleInternalID);

                    foreach (var peopleGrp in query) // foreach => people
                    {
                        if (!people.Any(p => p == peopleGrp.Key))
                        {
                            var unmacthPerson = new PeopleDTO()
                            {
                                PeopleInternalID = peopleGrp.Key,
                                PeopleFirstName = peopleGrp.FirstOrDefault().OracleAccess.AccessUserDescription
                            };
                            unmatchPeople.Add(unmacthPerson);
                            //LogManager.Error($"> Integration.Oracle [SyncPeopleAccess] >> No hay un persona con el ID de Oracle '{unmacthPerson.PeopleInternalID}', '{unmacthPerson.PeopleFirstName}'.");
                            continue;
                        }

                        var person = Mapper.Map<PeopleDTO>(db.People.FirstOrDefault(p => p.PeopleInternalID == peopleGrp.Key));

                        foreach (var access in peopleGrp) // foreach => access
                        {
                            if (!resources.Any(r => r.OracleResponsabilityID == access.OracleResponsabilityID))
                            {
                                var unmacthResponsability = (from metadata in db.Metadata
                                                             where
                                                                metadata.MetadataInt1 == access.OracleResponsabilityID
                                                             select new OracleResponsabilityDTO()
                                                             {
                                                                 ResponsabilityID = metadata.MetadataInt1,
                                                                 ResponsabilityName = metadata.MetadataDisplay,
                                                                 ResponsabilityCompanyID = metadata.MetadataStr1
                                                             }).FirstOrDefault();
                                unmatchResponsabilities.Add(unmacthResponsability);
                                //LogManager.Error($"> Integration.Oracle [SyncPeopleAccess] >> No hay un recurso vinculado para la responsabilidad '{unmacthResponsability.ResponsabilityName}', ID: {unmacthResponsability.ResponsabilityID}.");
                                continue;
                            };

                            var resource = resources.FirstOrDefault(r => r.OracleResponsabilityID == access.OracleResponsabilityID).Resource;

                            // Oracle: 1 | Icarus: 0 ------------------------------------------------------------------------------------------------ //
                            if (access.OracleAccess != null && access.IcarusAccess == null)
                            {
                                try
                                {
                                    // Save Resource People
                                    var rp = new ResourcePeople();
                                    rp.PeopleID = person.PeopleID;                             // <= Persona
                                    rp.PeopleDepartment = person.PeopleDepartment;
                                    rp.ResourceID = resource.ResourceID;                       // <= Recurso
                                    rp.ResourceFullName = resource.ResourceFullName;
                                    //rp.AddedRequestID = resource.RequestID;
                                    //rp.AddedRequestDetID = resource.RequestDetID;
                                    //rp.PresIntValue = resource.RequestDetIntValue;
                                    //rp.PresStrValue = resource.RequestDetStrValue;
                                    //rp.PresDateValue = null; // no usado por ahora
                                    //rp.PresDateStart = DateTime.Now;
                                    //rp.PresDateEnd = null;
                                    //rp.PresActive = 1;                                                         
                                    rp.PresTemporal = 1; //resource.PresTemporal;
                                    rp.PresValidityFrom = access.OracleAccess.AccessStartDate; //resource.PresValidityFrom;
                                    rp.PresValidityUntil = access.OracleAccess.AccessEndDate; //resource.PresValidityUntil;                                    
                                    //rp.PresAdditional = resource.PresAdditional;
                                    //rp.PresAdditionalIntValue = resource.PresAdditionalIntValue;
                                    //rp.PresAdditionalStrValue = resource.PresAdditionalStrValue;
                                    //rp.PresDisplayValue = resource.PresDisplayValue;
                                    rp.PresCompany = resource.ResourceCompany;                 // <= Empresa
                                    rp.EditUser = "integration.oracle";
                                    rp.ResourcePeopleLog.Add(new ResourcePeopleLog()
                                    {
                                        Action = AccessRequestDTO.TYPE_ALTA,
                                        Source = "ORACLE",
                                        Description = "Registro creado desde la sincronización de Oracle."
                                    });
                                    resourcePeopleRepository.SaveResourcePeople(rp, AccessRequestDTO.TYPE_ALTA); //db, people, rp, AccessRequestDTO.TYPE_ALTA);
                                }
                                catch (Exception ex)
                                {
                                    LogManager.Error($"> Integration.Oracle [SyncPeopleAccess] >> Error de sincronización en la iteración '{i}' al registrar la ALTA del acceso al Recurso: '{resource.ResourceFullName}' - ID: {resource.ResourceID} para la Persona: '{person.PeopleFullname} - Codigo: {person.PeopleInternalID}'.", ex);
                                }
                            }
                            // Oracle: 0 | Icarus: 1 ------------------------------------------------------------------------------------------------ //
                            else if (access.OracleAccess == null && access.IcarusAccess != null)
                            {
                                try
                                {
                                    var rp = access.IcarusAccess.ResourcePeople;
                                    rp.EditUser = "integration.oracle";
                                    rp.ResourcePeopleLog.Add(new ResourcePeopleLog()
                                    {
                                        Action = AccessRequestDTO.TYPE_BAJA,
                                        Source = "ORACLE",
                                        Description = "Registro modificado desde la sincronización de Oracle."
                                    });
                                    resourcePeopleRepository.SaveResourcePeople(rp, AccessRequestDTO.TYPE_BAJA);
                                }
                                catch (Exception ex)
                                {
                                    LogManager.Error($"> Integration.Oracle [SyncPeopleAccess] >> Error de sincronización en la iteración '{i}' al registrar la BAJA del acceso al Recurso: '{resource.ResourceFullName}' - ID: {resource.ResourceID} para la Persona: '{person.PeopleFullname} - Codigo: {person.PeopleInternalID}'.", ex);
                                }
                            }
                            // Oracle: 1 | Icarus: 1 ------------------------------------------------------------------------------------------------ //
                            else if (access.OracleAccess != null && access.IcarusAccess != null)
                            {
                                using (var transaction = db.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        var rp = access.IcarusAccess.ResourcePeople;
                                        var oracleAccess = access.OracleAccess;
                                        if (rp.PresValidityUntil != null && oracleAccess.AccessEndDate != null)
                                        {
                                            if (rp.PresValidityUntil.Value.Date != oracleAccess.AccessEndDate.Value.Date)  // Si la fecha de fin es distinta entre ResourcePeople y OracleAccess
                                            {
                                                rp.EditUser = "integration.oracle";

                                                // primero se da de baja --------------------------------------------------------------------------------- //
                                                rp.ResourcePeopleLog.Add(new ResourcePeopleLog()
                                                {
                                                    Action = AccessRequestDTO.TYPE_BAJA,
                                                    Source = "ORACLE",
                                                    Description = "Registro modificado desde la sincronización de Oracle."
                                                });
                                                resourcePeopleRepository.SaveResourcePeople(db, rp, AccessRequestDTO.TYPE_BAJA);
                                                db.SaveChanges();

                                                // luego se inserta el nuevo registro con la fecha actualizada ------------------------------------------- //

                                                //// SOLO SI la fecha de fin de OracleAccess es mayor a HOY se crea el registro de ALTA, sino solo se registra la baja (DEBERIA ESTAR ESTA CONDICION???)
                                                //if (oracleAccess.AccessEndDate.Value.Date > DateTime.Now.Date)
                                                //{
                                                rp = new ResourcePeople();
                                                rp.PeopleID = person.PeopleID;                             // <= Persona
                                                rp.PeopleDepartment = person.PeopleDepartment;
                                                rp.ResourceID = resource.ResourceID;                       // <= Recurso
                                                rp.ResourceFullName = resource.ResourceFullName;
                                                //rp.AddedRequestID = resource.RequestID;
                                                //rp.AddedRequestDetID = resource.RequestDetID;
                                                //rp.PresIntValue = resource.RequestDetIntValue;
                                                //rp.PresStrValue = resource.RequestDetStrValue;
                                                //rp.PresDateValue = null; // no usado por ahora
                                                //rp.PresDateStart = DateTime.Now;
                                                //rp.PresDateEnd = null;
                                                //rp.PresActive = 1;                                                         
                                                rp.PresTemporal = 1; //resource.PresTemporal;
                                                rp.PresValidityFrom = access.OracleAccess.AccessStartDate; //resource.PresValidityFrom;
                                                rp.PresValidityUntil = access.OracleAccess.AccessEndDate; //resource.PresValidityUntil;
                                                                                                          //rp.PresAdditional = resource.PresAdditional;
                                                                                                          //rp.PresAdditionalIntValue = resource.PresAdditionalIntValue;
                                                                                                          //rp.PresAdditionalStrValue = resource.PresAdditionalStrValue;
                                                                                                          //rp.PresDisplayValue = resource.PresDisplayValue;
                                                rp.PresCompany = resource.ResourceCompany;                 // <= Empresa
                                                rp.EditUser = "integration.oracle";
                                                rp.ResourcePeopleLog.Add(new ResourcePeopleLog()
                                                {
                                                    Action = AccessRequestDTO.TYPE_ALTA,
                                                    Source = "ORACLE",
                                                    Description = "Registro creado desde la sincronización de Oracle."
                                                });
                                                resourcePeopleRepository.SaveResourcePeople(db, rp, AccessRequestDTO.TYPE_ALTA);
                                                db.SaveChanges();
                                                //}

                                                transaction.Commit();
                                            }
                                        }
                                        else if (rp.PresValidityUntil != null && oracleAccess.AccessEndDate == null)
                                        {
                                            rp.EditUser = "integration.oracle";

                                            // primero se da de baja --------------------------------------------------------------------------------- //
                                            rp.ResourcePeopleLog.Add(new ResourcePeopleLog()
                                            {
                                                Action = AccessRequestDTO.TYPE_BAJA,
                                                Source = "ORACLE",
                                                Description = "Registro modificado desde la sincronización de Oracle."
                                            });
                                            resourcePeopleRepository.SaveResourcePeople(db, rp, AccessRequestDTO.TYPE_BAJA);
                                            db.SaveChanges();

                                            // luego se inserta el nuevo registro con la fecha actualizada ------------------------------------------- //

                                            //// SOLO SI la fecha de fin de OracleAccess es mayor a HOY se crea el registro de ALTA, sino solo se registra la baja (DEBERIA ESTAR ESTA CONDICION???)
                                            //if (oracleAccess.AccessEndDate.Value.Date > DateTime.Now.Date)
                                            //{
                                            rp = new ResourcePeople();
                                            rp.PeopleID = person.PeopleID;                             // <= Persona
                                            rp.PeopleDepartment = person.PeopleDepartment;
                                            rp.ResourceID = resource.ResourceID;                       // <= Recurso
                                            rp.ResourceFullName = resource.ResourceFullName;
                                            //rp.AddedRequestID = resource.RequestID;
                                            //rp.AddedRequestDetID = resource.RequestDetID;
                                            //rp.PresIntValue = resource.RequestDetIntValue;
                                            //rp.PresStrValue = resource.RequestDetStrValue;
                                            //rp.PresDateValue = null; // no usado por ahora
                                            //rp.PresDateStart = DateTime.Now;
                                            //rp.PresDateEnd = null;
                                            //rp.PresActive = 1;                                                         
                                            rp.PresTemporal = 1; //resource.PresTemporal;
                                            rp.PresValidityFrom = access.OracleAccess.AccessStartDate; //resource.PresValidityFrom;
                                            rp.PresValidityUntil = access.OracleAccess.AccessEndDate; //resource.PresValidityUntil;
                                                                                                      //rp.PresAdditional = resource.PresAdditional;
                                                                                                      //rp.PresAdditionalIntValue = resource.PresAdditionalIntValue;
                                                                                                      //rp.PresAdditionalStrValue = resource.PresAdditionalStrValue;
                                                                                                      //rp.PresDisplayValue = resource.PresDisplayValue;
                                            rp.PresCompany = resource.ResourceCompany;                 // <= Empresa
                                            rp.EditUser = "integration.oracle";
                                            rp.ResourcePeopleLog.Add(new ResourcePeopleLog()
                                            {
                                                Action = AccessRequestDTO.TYPE_ALTA,
                                                Source = "ORACLE",
                                                Description = "Registro creado desde la sincronización de Oracle."
                                            });
                                            resourcePeopleRepository.SaveResourcePeople(db, rp, AccessRequestDTO.TYPE_ALTA);
                                            db.SaveChanges();
                                            //}

                                            transaction.Commit();
                                        }
                                        else if (rp.PresValidityUntil == null && oracleAccess.AccessEndDate != null)
                                        {
                                            rp.EditUser = "integration.oracle";

                                            // primero se da de baja --------------------------------------------------------------------------------- //
                                            rp.ResourcePeopleLog.Add(new ResourcePeopleLog()
                                            {
                                                Action = AccessRequestDTO.TYPE_BAJA,
                                                Source = "ORACLE",
                                                Description = "Registro modificado desde la sincronización de Oracle."
                                            });
                                            resourcePeopleRepository.SaveResourcePeople(db, rp, AccessRequestDTO.TYPE_BAJA);
                                            db.SaveChanges();

                                            // luego se inserta el nuevo registro con la fecha actualizada ------------------------------------------- //

                                            //// SOLO SI la fecha de fin de OracleAccess es mayor a HOY se crea el registro de ALTA, sino solo se registra la baja (DEBERIA ESTAR ESTA CONDICION???)
                                            //if (oracleAccess.AccessEndDate.Value.Date > DateTime.Now.Date)
                                            //{
                                            rp = new ResourcePeople();
                                            rp.PeopleID = person.PeopleID;                             // <= Persona
                                            rp.PeopleDepartment = person.PeopleDepartment;
                                            rp.ResourceID = resource.ResourceID;                       // <= Recurso
                                            rp.ResourceFullName = resource.ResourceFullName;
                                            //rp.AddedRequestID = resource.RequestID;
                                            //rp.AddedRequestDetID = resource.RequestDetID;
                                            //rp.PresIntValue = resource.RequestDetIntValue;
                                            //rp.PresStrValue = resource.RequestDetStrValue;
                                            //rp.PresDateValue = null; // no usado por ahora
                                            //rp.PresDateStart = DateTime.Now;
                                            //rp.PresDateEnd = null;
                                            //rp.PresActive = 1;                                                         
                                            rp.PresTemporal = 1; //resource.PresTemporal;
                                            rp.PresValidityFrom = access.OracleAccess.AccessStartDate; //resource.PresValidityFrom;
                                            rp.PresValidityUntil = access.OracleAccess.AccessEndDate; //resource.PresValidityUntil;
                                                                                                      //rp.PresAdditional = resource.PresAdditional;
                                                                                                      //rp.PresAdditionalIntValue = resource.PresAdditionalIntValue;
                                                                                                      //rp.PresAdditionalStrValue = resource.PresAdditionalStrValue;
                                                                                                      //rp.PresDisplayValue = resource.PresDisplayValue;
                                            rp.PresCompany = resource.ResourceCompany;                 // <= Empresa
                                            rp.EditUser = "integration.oracle";
                                            rp.ResourcePeopleLog.Add(new ResourcePeopleLog()
                                            {
                                                Action = AccessRequestDTO.TYPE_ALTA,
                                                Source = "ORACLE",
                                                Description = "Registro creado desde la sincronización de Oracle."
                                            });
                                            resourcePeopleRepository.SaveResourcePeople(db, rp, AccessRequestDTO.TYPE_ALTA);
                                            db.SaveChanges();
                                            //}

                                            transaction.Commit();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        transaction.Rollback();
                                        LogManager.Error($"> Integration.Oracle [SyncPeopleAccess] >> Error de sincronización en la iteración '{i}' al registrar la MODIFICACION del acceso al Recurso: '{resource.ResourceFullName}' - ID: {resource.ResourceID} para la Persona: '{person.PeopleFullname} - Codigo: {person.PeopleInternalID}'.", ex);
                                    }
                                }
                            }
                        }
                    }

                    i++;
                    LogManager.Log($"> Integration.Oracle [SyncPeopleAccess] >> Final para '{oracleCompany.CompanyName}' - OracleCompanyID: {oracleCompany.CompanyID}.");
                }
                catch (Exception ex)
                {
                    errorCount++;
                    if (errorCount > 3)
                    {
                        i++;
                        errorCount = 0;
                        LogManager.Error($"> Integration.Oracle [SyncPeopleAccess] >> Error de sincronización en la iteración '{i}' al sincronizar la empresa: {oracleCompany.CompanyName} - ID: {oracleCompany.CompanyID}.", ex);
                    }
                }
            }

            // Graba los registros no coincidentes ------------------------------------------------------------------------ //
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var integrationOracleLogRepository = new IntegrationOracleLogRepository();

                    unmatchPeople = unmatchPeople.DistinctBy(p => p.PeopleInternalID).ToList();
                    var logUnmatchPeople = integrationOracleLogRepository.GetLogByType(IntegrationOracleLogDTO.TYPE_UNMATCH_PEOPLE).ToList();
                    unmatchResponsabilities = unmatchResponsabilities.Where(r => r != null).DistinctBy(r => r.ResponsabilityID).ToList();
                    var logUnmatchResponsabilities = integrationOracleLogRepository.GetLogByType(IntegrationOracleLogDTO.TYPE_UNMATCH_RESPONSABILITY).ToList();

                    foreach (var dto in logUnmatchPeople)
                    {
                        dto.LogActive = 0;
                        integrationOracleLogRepository.SaveOracleLog(db, dto, USER_ID);
                    }
                    foreach (var item in unmatchPeople)
                    {
                        var dto = new IntegrationOracleLogDTO()
                        {
                            LogType = IntegrationOracleLogDTO.TYPE_UNMATCH_PEOPLE,
                            LogData1 = item.PeopleInternalID,
                            LogData2 = item.PeopleFirstName
                        };
                        integrationOracleLogRepository.SaveOracleLog(db, dto, USER_ID);
                    }

                    foreach (var dto in logUnmatchResponsabilities)
                    {
                        dto.LogActive = 0;
                        integrationOracleLogRepository.SaveOracleLog(db, dto, USER_ID);
                    }
                    foreach (var item in unmatchResponsabilities)
                    {
                        var dto = new IntegrationOracleLogDTO()
                        {
                            LogType = IntegrationOracleLogDTO.TYPE_UNMATCH_RESPONSABILITY,
                            LogData1 = item.ResponsabilityID.ToString(),
                            LogData2 = item.ResponsabilityName
                        };
                        integrationOracleLogRepository.SaveOracleLog(db, dto, USER_ID);
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    LogManager.Error($"> Integration.Oracle [SyncPeopleAccess] >> Error al grabar el Log de Oracle.", ex);
                }
            }
            // Graba los registros no coincidentes ------------------------------------------------------------------------ //

            var timeExecution = DateTime.Now.Subtract(datetimeStart);
            LogManager.Log($"> Integration.Oracle [SyncPeopleAccess] >> Final sincronización de accesos. Operación ejecutada en {timeExecution.TotalMinutes} minutos");
        }

        private string GetFormatNewUserEmail(AccessRequestDetails requestDetail, OracleAccessPeopleResponse message)
        {
            //string url = "<a href=\"cccc\">http://oraclegram.gym.com.pe:8000</a>";
            string url = "<a href=\"cccc\">https://oracleebs.aenza.com.pe:4443/OA_HTML/AppsLocalLogin.jsp</a>";

            return $"<p>Estimado Sr(a):</p>" +
                   $"<p><u><strong>[[solicitud_para]]</strong></u></p>" +
                   $"<p>El Sistema ICARUS registr&oacute; correctamente su acceso a la aplicaci&oacute;n ORACLE con la responsabilidad: <u><strong>\"{requestDetail.ResourceFullName}\"</strong></u>. A continuaci&oacute;n se env&iacute;a sus credenciales para poder acceder a la aplicaci&oacute;n:</p>" +
                   $"<p>Url: {url}</p>  <p>Usuario: {message.UserName}</p>" +
                   $"<p>Contrase&ntilde;a: {message.UserPassword}</p>" +
                   $"<p>Vigencia del acceso: {String.Format("{0:dd/MM/yyyy}", message.AccessStartDate)} - {String.Format("{0:dd/MM/yyyy}", message.AccessEndDate)}</p>" +
                   $"<p>Recuerde que la contrase&ntilde;a es temporal y debe cambiarla al ingresar por primera vez a la aplicaci&oacute;n.</p>" +
                   $"<p>&nbsp;</p>  <hr /> <h2>Informaci&oacute;n de la Solicitud</h2>" +
                   $"<table border=\"1\" cellpadding=\"1\" cellspacing=\"0\" >" +
                   $"<tbody>" +
                   $"<tr>    <td><strong>Solicitud N&uacute;mero:</strong></td>    <td>[[solicitud_numero]]</td>   </tr>" +
                   $"<tr>    <td><strong>Solicitado por:</strong></td>    <td>[[solicitud_solicitante]]</td>   </tr>" +
                   $"<tr>    <td><strong>Solicitado para:</strong></td>    <td>[[solicitud_para]]</td>   </tr>" +
                   $"<tr>    <td><strong>Fecha de Solicitud:</strong></td>    <td>[[solicitud_fecha]]</td>   </tr>" +
                   $"<tr>    <td><strong>Observaciones:</strong></td>    <td>[[solicitud_observacion]]</td>   </tr>" +
                   $"</tbody>" +
                   $"</table>" +
                   $"<hr /> <h2>Historial de la Solicitud</h2>  <p>[[solicitud_historia]]</p>";
        }

        private void UpdateRequestDetail(AccessRequestDetails requestDetail, OracleAccessPeopleResponse message)
        {
            SisConAxsContext db = new SisConAxsContext();
            var model = db.AccessRequestDetails.FirstOrDefault(x => x.RequestDetID == requestDetail.RequestDetID);
            model.ReqDetTemporal = 1;
            model.ReqDetValidityFrom = message.AccessStartDate;
            model.ReqDetValidityUntil = message.AccessEndDate;
            db.AccessRequestDetails.Attach(model);
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void SavePeopleAccess(WorkflowExecution wfExec, AccessRequestDetails requestDetail, Action<string, AccessRequestDetails> OnSuccess, Action<IntegrationResourceException> OnError)
        {
            if (requestDetail.RequestDetType == AccessRequestDTO.TYPE_ALTA || requestDetail.RequestDetType == AccessRequestDTO.TYPE_MODIFICACION)
            {
                try
                {
                    var payload = OracleSaveResourcePeoplePayload.FromRequestDetail(requestDetail);
                    var headers = GetToken();
                    var json = JsonConvert.SerializeObject(payload);
                    ServerApiRequest request = ServerApiRequest.Create($"{OracleConfig.Address}/access", ServerApiRequest.HTTP_METHOD_POST, json, headers);
                    request.Send(
                        (response) =>
                        {
                            var message = JsonConvert.DeserializeObject<OracleAccessPeopleResponse>(response.Replace("\"null\"", "null"), new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

                            // Si el API crea un usuario en Oracle, se prepara el correo de notificación al usuario
                            if (!String.IsNullOrWhiteSpace(message.UserCode))
                            {
                                string body = GetFormatNewUserEmail(requestDetail, message);
                                RequestEmailDataStorage.PrepareEmailData(wfExec, defaultSubject: "ICARUS: Creación de Cuenta de Oracle", emailDest: payload.UserEmail, body: body);
                            }

                            // Valida que la fecha de fin no sea menor o igual a Hoy
                            if (message.AccessEndDate != null)
                            {
                                if (message.AccessEndDate.Value.Date <= DateTime.Now.Date)
                                {
                                    OnError(new IntegrationResourceException(IntegrationResourceException.ERROR_SERVER_VALIDATION, "> Integration.Oracle >> La fecha de fin del acceso no puede ser menor o igual a hoy, la responsabilidad ha caducado o la fecha ingresada es menor a hoy."));
                                    return;
                                }
                            }

                            // Actualiza el detalle de la solicitud
                            try
                            {
                                UpdateRequestDetail(requestDetail, message);
                            }
                            catch (Exception ex)
                            {
                                OnError(new IntegrationResourceException(IntegrationResourceException.ERROR_OTHER, "> Integration.Oracle >> Error al actualizar el detalle de la solicitud.", ex));
                                return;
                            }

                            OnSuccess(response, requestDetail);
                        },
                        (ex, jsonMessage) =>
                        {
                            if (ex.Status == WebExceptionStatus.Timeout)
                            {
                                OnError(new IntegrationResourceException(IntegrationResourceException.ERROR_TIMEOUT, "> Integration.Oracle >> Tiempo de respuesta del servidor expirado.", ex));
                                return;
                            }
                            var statusCode = ((HttpWebResponse)ex.Response).StatusCode;
                            if (statusCode != HttpStatusCode.InternalServerError)
                            {
                                OnError(new IntegrationResourceException(IntegrationResourceException.ERROR_SERVER, $"> Integration.Oracle >> Error al realizar la solicitud al servidor. Status code: {statusCode}", ex));
                                return;
                            }

                            var error = JsonConvert.DeserializeObject<OracleAccessPeopleResponse>(jsonMessage);
                            if (error.Code == ERROR_BD_CONNECTION)
                            {
                                OnError(new IntegrationResourceException(IntegrationResourceException.ERROR_CONNECTION, "> Integration.Oracle >> Error en la conexión con la BD Oracle.", ex));
                                return;
                            }
                            if (error.Code == ERROR_RESPONSABILITY_NOT_FOUND)
                            {
                                OnError(new IntegrationResourceException(IntegrationResourceException.ERROR_SERVER_VALIDATION, $"> Integration.Oracle >> Responsabilidad no encontrada > ID: {payload.Access.AccessResponsabilityID}.", ex));
                                return;
                            }
                            if (error.Code <= -10000) // Si es un error de validación del servidor
                            {
                                OnError(new IntegrationResourceException(IntegrationResourceException.ERROR_SERVER_VALIDATION, $"> Integration.Oracle >> Error al registrar al nuevo usuario Oracle {error.Message}.", ex));
                                return;
                            }
                            //if (error.Code == ERROR_CREATE_USER)
                            //{
                            //    OnError(new IntegrationResourceException(IntegrationResourceException.ERROR_SERVER_VALIDATION, "> Integration.Oracle >> Error al crear usuario Oracle.", ex));
                            //    return;
                            //}
                            OnError(new IntegrationResourceException(IntegrationResourceException.ERROR_SERVER, $"> Integration.Oracle >> Error al grabar la ALTA/MODIFICACION del acceso en el servidor Oracle > Code: {error.Code}, Message: {error.Message}", ex));
                        }
                    );
                }
                catch (IntegrationResourceException ex)
                {
                    OnError(ex);
                }
                catch (Exception ex)
                {
                    OnError(new IntegrationResourceException(IntegrationResourceException.ERROR_OTHER, "> Integration.Oracle >> Error al grabar la ALTA/MODIFICACION del acceso en Oracle.", ex));
                }
            }
            else
            {
                OnError(new IntegrationResourceException(IntegrationResourceException.ERROR_CLIENT_VALIDATION, "> Integration.Oracle >> Error al grabar la ALTA/MODIFICACION el detalle de solicitud NO es de tipo ALTA/MODIFICACION."));
            }
        }

        public void DeletePeopleAccess(WorkflowExecution wfExec, AccessRequestDetails requestDetail, Action<string, AccessRequestDetails> OnSuccess, Action<IntegrationResourceException> OnError)
        {
            if (requestDetail.RequestDetType == AccessRequestDTO.TYPE_BAJA)
            {
                try
                {
                    var payload = OracleDeleteResourcePeoplePayload.FromRequestDetail(requestDetail);
                    var headers = GetToken();
                    ServerApiRequest request = ServerApiRequest.Create($"{OracleConfig.Address}/access/delete", ServerApiRequest.HTTP_METHOD_POST, JsonConvert.SerializeObject(payload), headers);
                    request.Send(
                        (response) =>
                        {
                            OnSuccess(response, requestDetail);
                        },
                        (ex, jsonMessage) =>
                        {
                            if (ex.Status == WebExceptionStatus.Timeout)
                            {
                                OnError(new IntegrationResourceException(IntegrationResourceException.ERROR_TIMEOUT, "> Integration.Oracle >> Error en tiempo de respuesta del servidor expirado.", ex));
                                return;
                            }
                            var statusCode = ((HttpWebResponse)ex.Response).StatusCode;
                            if (statusCode != HttpStatusCode.InternalServerError)
                            {
                                OnError(new IntegrationResourceException(IntegrationResourceException.ERROR_SERVER, $"> Integration.Oracle >> Error al realizar la solicitud al servidor. Status code: {statusCode}", ex));
                                return;
                            }

                            var error = JsonConvert.DeserializeObject<OracleAccessPeopleResponse>(jsonMessage);
                            if (error.Code == ERROR_BD_CONNECTION)
                            {
                                OnError(new IntegrationResourceException(IntegrationResourceException.ERROR_CONNECTION, "> Integration.Oracle >> Error en la conexión con la BD Oracle.", ex));
                                return;
                            }
                            if (error.Code == ERROR_USER_NOT_FOUND)
                            {
                                OnError(new IntegrationResourceException(IntegrationResourceException.ERROR_SERVER_VALIDATION, $"> Integration.Oracle >> Usuario ORACLE no encontrado > ID: {payload.UserCode}.", ex));
                                return;
                            }
                            if (error.Code == ERROR_RESPONSABILITY_NOT_FOUND)
                            {
                                OnError(new IntegrationResourceException(IntegrationResourceException.ERROR_SERVER_VALIDATION, $"> Integration.Oracle >> Responsabilidad no encontrada > ID: {payload.Access.AccessResponsabilityID}.", ex));
                                return;
                            }
                            if (error.Code == ERROR_USER_INACTIVE)
                            {
                                OnError(new IntegrationResourceException(IntegrationResourceException.ERROR_SERVER_VALIDATION, $"> Integration.Oracle >> Este Usuario Oracle está Inactivo, ya no es necesario ejecutar la baja > ID: {payload.UserCode}.", ex));
                                return;
                            }
                            OnError(new IntegrationResourceException(IntegrationResourceException.ERROR_SERVER, $"> Integration.Oracle >> Error al grabar la BAJA del acceso en Oracle > Code: {error.Code}, Message: {error.Message}", ex));
                        }
                    );
                }
                catch (IntegrationResourceException ex)
                {
                    OnError(ex);
                }
                catch (Exception ex)
                {
                    OnError(new IntegrationResourceException(IntegrationResourceException.ERROR_OTHER, "> Integration.Oracle >> Error al grabar la BAJA del acceso en el servidor Oracle.", ex));
                }
            }
            else
            {
                OnError(new IntegrationResourceException(IntegrationResourceException.ERROR_CLIENT_VALIDATION, "> Integration.Oracle >> Error al grabar la BAJA el deatalle de solicitud NO es de tipo BAJA."));
            }
        }

        public List<OracleAccessPeopleDTO> GetPeopleAccess(string companyID)
        {
            throw new NotImplementedException();
        }
    }
}