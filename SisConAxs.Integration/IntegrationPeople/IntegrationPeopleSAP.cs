using Newtonsoft.Json;
using SisConAxs.Integration.DTO;
using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using SisConAxs_DM.Repository;
using SisConAxs_DM.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Linq.Expressions;
using Microsoft.Graph.Models;
using AutoMapper;

namespace SisConAxs.Integration
{
    public class IntegrationPeopleSAP : IIntegrationPeople<SAPResponsePeopleDTO> //, PayloadCompanySAP>
    {
        public const int ERROR_NOT_FOUND = 0;       // registro no encontrado
        public const int ERROR_SERVER = 0;          // error del servidor

        public const string MOVEMENT_ALTA = "A";
        public const string MOVEMENT_BAJA = "B";
        public const string MOVEMENT_MODIFICACION = "M";

        private string API_ADDRESS = ConfigurationManager.AppSettings["ServiceApiSAP"].ToString();  //"http://sap.gym.com.pe/api";
        private const string USER_ID = "integration.sap";
        private SessionData USER_SESSIONDATA = new SessionData()
        {
            User = new AccessUserDTO() { UserFirstName = "Intergración SAP", UserLastName = "Intergración SAP" },
            sessionUser = USER_ID,
            UserRole3 = 1,        // ADMIN
            UserRoleSysAdmin = 1  // SYSADMIN
        };

        private Dictionary<string, int> TipoDocToID = new Dictionary<string, int>()
        {
            { "DOC. NACIONAL DE IDENTIDAD", PeopleDTO.DOC_TYPE_DNI },
            { "PASAPORTE", PeopleDTO.DOC_TYPE_PASAPORTE },
            { "CARNÉ DE EXTRANJERÍA", PeopleDTO.DOC_TYPE_CE },
            { "Diferente", PeopleDTO.DOC_TYPE_DIFERENTE }
        };


        CommonValueSetsRepository CommonValueSetsRepository;
        RequestTemplateRepository RequestTemplateRepository;
        AccessRequestRepository AccessRequestRepository;
        PeopleRepository PeopleRepository;
        ResourcePeopleRepository ResourcePeopleRepository;
        AccessUsersRepository UserRepository;
        CompanyRepository CompanyRepository;
        IntegrationSAPLogRepository IntegrationSAPLogRepository;

        public IntegrationPeopleSAP()
        {
            this.CommonValueSetsRepository = new CommonValueSetsRepository(USER_SESSIONDATA);
            this.RequestTemplateRepository = new RequestTemplateRepository(USER_SESSIONDATA);
            this.AccessRequestRepository = new AccessRequestRepository(USER_SESSIONDATA);
            this.PeopleRepository = new PeopleRepository(USER_SESSIONDATA);
            this.ResourcePeopleRepository = new ResourcePeopleRepository();
            this.UserRepository = new AccessUsersRepository();
            this.CompanyRepository = new CompanyRepository();
            this.IntegrationSAPLogRepository = new IntegrationSAPLogRepository();
        }

        public List<SAPResponsePeopleDTO> GetPeopleMovement(DateTime startDate, DateTime endDate, string type)
        {
            var list = new List<SAPResponsePeopleDTO>();
            var startDateString = startDate.ToString("yyyy-MM-dd");
            var endDateString = endDate.ToString("yyyy-MM-dd");
            var url = $"{API_ADDRESS}?fechaInicio={startDateString}&fechaFin={endDateString}&tipo={type}";
            var request = ServerApiRequest.Create(url, ServerApiRequest.HTTP_METHOD_GET);
            request.Send((response) =>
            {
                list = JsonConvert.DeserializeObject<List<SAPResponsePeopleDTO>>(response);
            },
            (ex, jsonMessage) =>
            {
                throw ex;
            });
            return list;
        }


        public void SyncPeopleDeactivate()
        {
            try
            {
                SisConAxsContext db = new SisConAxsContext();
                var startDate = DateTime.Now.AddDays(-1);
                var endDate = DateTime.Now.AddDays(-1);
                var movements = GetPeopleMovement(startDate, endDate, MOVEMENT_BAJA);

                //List<SAPResponsePeopleDTO> movements = new List<SAPResponsePeopleDTO>();
                //movements.Add(new SAPResponsePeopleDTO()
                //{
                //    NumeroDocumento = "74976448",
                //    Nombre = "Aarom",
                //    PrimerApellido = "Rojas",
                //    SegundoApellido = "Nizama",
                //    NombreEmpresa = "UNNA ENERGÍA",
                //    FechaCese = DateTime.Today
                //});


                // filtro para omitir los recursos de Oracle ERP
                Expression<Func<ResourcePeople, bool>> filter = rp => rp.AccessResources.ResourceCategory != Constants.CATEGORY_ORACLE_ERP;
                int count = 0;

                if (movements.Count > 0)
                {
                    foreach (var mov in movements)
                    {
                        count++;
                        string messageUser = "Usuario: No ejecutado.\n\n";
                        string messagePeople = "Destinatario (Persona): No ejecutado.\n\n";
                        string messageCancelRequest = "Cancelación de solicitides pendientes: No ejecutado.\n\n";
                        string messageRequest = "Creación de solicitud para baja de recursos: No ejecutado.\n\n";

                        IntegrationSAPLogDTO log = mov.ToIntegrationSAPLogDTO(AccessRequestDTO.TYPE_BAJA);
                        try
                        {
                            if (log.LogTypeDate == null)
                            {
                                log.LogTypeDate = DateTime.Now;
                                throw new Exception("La fecha de baja no es válida.");
                            }                                

                            var people = PeopleRepository.GetPeopleByDocumentNumber(mov.NumeroDocumento);
                            if (people != null)
                            {
                                try
                                {
                                    // 1. Se desactiva el usuario vinculado y se elimina sus roles -------------------------------------------------------------
                                    var user = UserRepository.GetAccessUserQuery(u => u.UserInternalID == people.UserID && u.UserStatus == 1).FirstOrDefault();
                                    if (user != null)
                                    {
                                        UserRepository.UpdateStatusAndDeleteRoles(user.UserID, false, USER_ID);
                                        messageUser = "Usuario: Inactivado y roles eliminados.\n\n";
                                    }
                                    else
                                    {
                                        messageUser = "Usuario: No ejecutado, sin usuario relacionado.\n\n";
                                    }

                                    // 2. Se cancelan las solicitudes en espera --------------------------------------------------------
                                    int resultCancelRequests = AccessRequestRepository.CancelAllRequestByPeople(people.PeopleID, USER_ID);
                                    if (resultCancelRequests == 0)
                                    {
                                        messageCancelRequest = "Cancelación de solicitides pendientes: Sin ejecuciones.\n\n";
                                    }
                                    else if (resultCancelRequests > 0)
                                    {
                                        messageCancelRequest = "Cancelación de solicitides pendientes: Ejecutada.\n\n";
                                    }
                                    else
                                    {
                                        //message = $"La persona PeopleID: {people.PeopleID}, Num Documento: {mov.NumeroDocumento}, Nombre: {mov.Nombre} {mov.PrimerApellido} {mov.SegundoApellido}, aún tiene solicitudes pendientes.";
                                        log.LogResult = IntegrationSAPLogDTO.RESULT_PARCIAL;
                                        messageCancelRequest = "Cancelación de solicitides pendientes: El destinatario aún tiene solicitudes pendientes.\n\n";
                                    }

                                    // 3. Se desactivan los recursos de la persona -----------------------------------------------------
                                    int resultDeactivateResources = AccessRequestRepository.DeactivateAllAccessByPeople(
                                        people.PeopleID,
                                        "Realizado por baja automática (Integración Empleado SAP).",
                                        //true
                                        filter
                                    );
                                    if (resultDeactivateResources == 0)
                                    {
                                        messageRequest = "Creación de solicitud para baja de recursos: sin ejecuciones.\n\n";
                                    }
                                    else if (resultDeactivateResources > 0)
                                    {
                                        messageRequest = "Creación de solicitud para baja de recursos: ejecutada.\n\n";
                                    }
                                    else
                                    {
                                        //message = $"La persona PeopleID: {people.PeopleID}, Num Documento: {mov.NumeroDocumento}, Nombre: {mov.Nombre} {mov.PrimerApellido} {mov.SegundoApellido}, no se pudo crear la solicitud de Baja.";
                                        log.LogResult = IntegrationSAPLogDTO.RESULT_PARCIAL;
                                        messageRequest = "Creación de solicitud para baja de recursos: No se pudo crear la solicitud.\n\n";
                                    }

                                    // verifica si aún quedan recursos sin darse de baja ----------------------------------------------
                                    var queryIssues = db.ResourcePeople.Where(
                                        rp => rp.PeopleID == people.PeopleID && rp.PresActive == 1
                                                && (
                                                    rp.AccessResources == null || rp.AccessResources.Workflows == null || rp.AccessResources.Workflows.WorkflowItems.Count == 0
                                                //|| (
                                                //    // Si no tiene un item de execución
                                                //    (rp.AccessResources.Workflows.WorkflowItems.Count(x => x.WfItemType == WorkflowItemsDTO.TYPE_ACTION &&
                                                //                                                           x.WfItemActionProperty == WorkflowItemsDTO.ACTION_TYPE_EXECUTE_IN_SERVER) == 0) &&
                                                //    // Si no tiene notificación al ejecutor
                                                //    (rp.AccessResources.Workflows.WorkflowItems.Count(x => x.CommonValues.CommonValueName == "NOTIFICACION" &&
                                                //                                                           x.WfItemDestType == (int)WfItemDestType.Ejecutor) == 0)
                                                //)
                                                )
                                    );
                                    if (filter != null)
                                    {
                                        queryIssues = queryIssues.Where(filter);
                                    }
                                    if (queryIssues.Count() > 0)
                                    {
                                        //message = $"La persona PeopleID: {people.PeopleID}, Num Documento: {mov.NumeroDocumento}, Nombre: {mov.Nombre} {mov.PrimerApellido} {mov.SegundoApellido}, tiene recursos q no han sido dados de baja.";
                                        log.LogResult = IntegrationSAPLogDTO.RESULT_PARCIAL;
                                        messageRequest += $"Creación de solicitud para baja de recursos: Este destinatario (persona) aún tiene recursos que no han sido dados de baja.\n\n";
                                    }
                                    else
                                    {
                                        // 4. Se desactiva el registro de persona -------------------------------------------------------
                                        PeopleRepository.UpdateActive(people.PeopleID, false, USER_ID);
                                        if (log.LogResult == IntegrationSAPLogDTO.RESULT_WITHOUT_CHANGES)
                                        {
                                            log.LogResult = IntegrationSAPLogDTO.RESULT_SUCCESS;
                                            messagePeople = "Destinatario (Persona): inactivada.\n\n";
                                        }
                                    }

                                    // Log SAP
                                    log.LogMessage = messageUser + messageCancelRequest + messageRequest + messagePeople;
                                    IntegrationSAPLogRepository.SaveSAPLog(log, USER_ID);
                                }
                                catch (Exception ex)
                                {
                                    //message = $"Error al inactivar la persona PeopleID: {people.PeopleID}, Num Documento: {mov.NumeroDocumento}, Nombre: {mov.Nombre} {mov.PrimerApellido} {mov.SegundoApellido}.";
                                    log.LogException = ex;
                                    log.LogResult = IntegrationSAPLogDTO.RESULT_ERROR; //log.LogResult == IntegrationSAPLogDTO.RESULT_PARCIAL ? IntegrationSAPLogDTO.RESULT_PARCIAL : IntegrationSAPLogDTO.RESULT_ERROR;
                                    log.LogMessage = $"Error al inactivar al destinatario (persona).\n\n" + messageUser + messageCancelRequest + messageRequest + messagePeople;
                                    IntegrationSAPLogRepository.SaveSAPLog(log, USER_ID);
                                }
                            }
                            else
                            {
                                log.LogMessage += $"Destinatario (persona) no encontrada, no cuenta con un registro en ICARUS.";
                                IntegrationSAPLogRepository.SaveSAPLog(log, USER_ID);
                            }
                        }
                        catch (Exception ex)
                        {
                            log.LogException = ex;
                            log.LogResult = IntegrationSAPLogDTO.RESULT_ERROR;
                            log.LogMessage = $"Error al inactivar al destinatario (persona).\n\n" + messageUser + messageCancelRequest + messageRequest + messagePeople;
                            IntegrationSAPLogRepository.SaveSAPLog(log, USER_ID);
                        }
                    }
                }
                else
                {
                    LogManager.Log($"> Integration.SAP [SyncPeopleDeactivate] >> Sin nuevos registros de BAJA.");
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"> Integration.SAP [SyncPeopleDeactivate] >> Error al realizar la consulta.", ex);
            }
        }

        public void SyncPeopleActivate()
        {
            //System.Threading.Thread.Sleep(40000);
            try
            {
                var startDate = DateTime.Now.AddDays(-1);
                var endDate = DateTime.Now.AddDays(-1);
                var movements = GetPeopleMovement(startDate, endDate, MOVEMENT_ALTA);
                //movements = movements.FindAll(m => m.NumeroDocumento == "09725115");

                if (movements.Count > 0)
                {
                    foreach (var mov in movements)
                    {
                        string messagePeople = "Destinatario (Persona): No ejecutado.\n\n";
                        string messageUser = "Usuario: No ejecutado.\n\n";
                        string messageRequest = "Solicitud de Alta: No ejecutado.\n\n";

                        var log = mov.ToIntegrationSAPLogDTO(AccessRequestDTO.TYPE_ALTA);
                        try
                        {
                            // 1. Registra datos de la persona -----------------------------------------------------------------------
                            var people = PeopleRepository.GetPeopleByDocumentNumber(mov.NumeroDocumento);
                            try
                            {
                                if (people == null)
                                {
                                    // crea registro persona
                                    people = new PeopleDTO();
                                    people = SavePeopleFromSAPResponse(mov, people);
                                    messagePeople = "Destinatario (Persona): Creado.\n\n";
                                }
                                else
                                {
                                    // Actualiza registro persona
                                    people = SavePeopleFromSAPResponse(mov, people);
                                    messagePeople = "Destinatario (Persona): Actualizado.\n\n";
                                }
                                log.LogResult = IntegrationSAPLogDTO.RESULT_SUCCESS;
                            }
                            catch (Exception ex)
                            {
                                messagePeople = "Destinatario (Persona): Error al ejecutar.\n\n";
                                throw ex;
                            }

                            // 2. Registra datos del usuario asociado ----------------------------------------------------------------
                            var user = UserRepository.GetAccessUserQuery(u => u.UserInternalID == people.UserID).FirstOrDefault();
                            try
                            {
                                if (user == null)
                                {
                                    if (!String.IsNullOrWhiteSpace(people.PeopleEmail))
                                    {
                                        user = new AccessUserDTO();
                                        SaveUserFromPeople(people, user);

                                        // vincula a la persona con el usuario
                                        people.UserID = user.UserInternalID;
                                        PeopleRepository.SavePeople(people, people.PeopleCompany);
                                        messageUser = "Usuario: Creado.\n\n";
                                    }
                                    else
                                    {
                                        messageUser = "Usuario: No creado, el destinatario no tiene correo.\n\n";
                                    }
                                }
                                else
                                {
                                    // actualiza usuario
                                    SaveUserFromPeople(people, user);
                                    messageUser = "Usuario: Actualizado.\n\n";
                                }
                                log.LogResult = IntegrationSAPLogDTO.RESULT_SUCCESS;
                            }
                            catch (Exception ex)
                            {
                                messageUser = "Usuario: Error al ejecutar.\n\n";
                                throw ex;
                            }

                            // 3. Crea la solicitud de alta --------------------------------------------------------------------------
                            try
                            {
                                var template = this.RequestTemplateRepository.GetReqTemplatesQuery(t =>
                                t.ReqTemplateType == AccessRequestDTO.TYPE_ALTA &&
                                t.ReqTemplateCompany == people.PeopleCompany &&
                                t.ReqTemplateEmployeeType == people.PeopleEmployeeType &&
                                t.ReqTemplateActive
                            ).FirstOrDefault();
                                if (template != null)
                                {
                                    var requestDTO = BuildRequestFromTemplate(people, template.ReqTemplateID);
                                    if (requestDTO.AccessRequestDetails.Count > 0)
                                    {
                                        this.AccessRequestRepository.InsertAccessRequest(requestDTO, USER_ID, people.PeopleCompany);
                                        messageRequest = $"Asignación de solicitud base: Ejecutado.\n\n";
                                    }
                                    else
                                    {
                                        //message = $"La Solicitud de ALTA no tiene recursos disponibles para crear el detalle, Num Documento: {mov.NumeroDocumento}, Nombre: {mov.Nombre} {mov.PrimerApellido} {mov.SegundoApellido}, Tipo Empleado: {mov.TipoPersonal}.";
                                        log.LogResult = IntegrationSAPLogDTO.RESULT_PARCIAL;
                                        messageRequest = $"Asignación de solicitud base: No ejecutado, no tiene recursos disponibles para crear el detalle.\n\n";
                                    }
                                }
                                else
                                {
                                    //message = $"Solicitud Base no encontrada para Num Documento: {mov.NumeroDocumento}, Nombre: {mov.Nombre} {mov.PrimerApellido} {mov.SegundoApellido}, Tipo Empleado: {mov.TipoPersonal}.";
                                    log.LogResult = IntegrationSAPLogDTO.RESULT_PARCIAL;
                                    messageRequest = $"Asignación de solicitud base: No ejecutado, solicitud base no encontrada.\n\n";
                                }
                            }
                            catch (Exception ex)
                            {
                                messageRequest = "Asignación de solicitud base: Error al ejecutar.\n\n";
                                throw ex;
                            }

                            // Log SAP
                            if (log.LogResult == IntegrationSAPLogDTO.RESULT_SUCCESS)
                            {
                                log.LogMessage = $"Ejecutado correctamente.\n\n" + messagePeople + messageUser + messageRequest;
                            }
                            else
                            {
                                log.LogMessage = $"Ejecutado parcialmente.\n\n" + messagePeople + messageUser + messageRequest;
                            }
                            IntegrationSAPLogRepository.SaveSAPLog(log, USER_ID);
                        }
                        catch (Exception ex)
                        {
                            //message = $"Error al registrar la ALTA de la Persona, Num Documento: {mov.NumeroDocumento}, Nombre: {mov.Nombre} {mov.PrimerApellido} {mov.SegundoApellido}.";
                            log.LogException = ex;
                            if (log.LogResult == IntegrationSAPLogDTO.RESULT_SUCCESS || log.LogResult == IntegrationSAPLogDTO.RESULT_PARCIAL)
                            {
                                log.LogResult = IntegrationSAPLogDTO.RESULT_PARCIAL;
                            }
                            else
                            {
                                log.LogResult = IntegrationSAPLogDTO.RESULT_ERROR;
                            }
                            log.LogMessage += $"Error al registrar la ALTA del destinatario (persona).\n\n" + messagePeople + messageUser + messageRequest;
                            IntegrationSAPLogRepository.SaveSAPLog(log, USER_ID);
                        }
                    }
                }
                else
                {
                    LogManager.Log($"> Integration.SAP [SyncPeopleActivate] >> Sin nuevos registros de ALTA.");
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"> Integration.SAP [SyncPeopleActivate] >> Error al realizar la consulta.", ex);
            }
        }

        public void SyncPeopleModify()
        {
            //throw new NotImplementedException();
        }


        // People -----------------------------------------------------------------------------------------------------------
        private void SetValuesFromSAPResponseToPeople(SAPResponsePeopleDTO response, PeopleDTO people)
        {
            // Empresa
            var companyName = response.NombreEmpresa.ToUpper().Trim();
            var company = CompanyRepository.GetCompanyBySAP(companyName).FirstOrDefault();
            if (company == null)
            {
                throw new Exception($"Empresa no válida: '{companyName}'");
            }
            people.PeopleCompany = company.CompanyID;

            // Tipo Empleado
            var tipoPersonal = CommonValueSetsRepository.GetCommonValuesQuery(cv =>
                                    cv.CommonValueSetID == CommonValueSetsDTO.SET_TIPO_EMPLEADO
                                    && cv.CommonValueName.ToUpper().Trim() == response.TipoPersonal.ToUpper().Trim()
                               ).FirstOrDefault();
            if (tipoPersonal == null)
            {
                throw new Exception($"Tipo de Personal no válido: '{response.TipoPersonal.Trim()}'");
            }
            people.PeopleEmployeeType = tipoPersonal.CommonValueID;

            // Correo
            var email = response.CorreoPersonal.Trim();
            Regex reg = new Regex(@"[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,6}", RegexOptions.IgnoreCase);
            Match match = reg.Match(email);
            if (match.Success)
            {
                people.PeopleEmail = match.Value.Trim();
            }

            if (people.PeopleID == 0)
            {
                // Tipo Documento
                int tipoDoc = 0;
                if (!TipoDocToID.TryGetValue(response.NombreTipoDocumento.ToUpper().Trim(), out tipoDoc))
                {
                    throw new Exception($"Tipo de Documento no válido: '{response.NombreTipoDocumento.Trim()}'");
                }
                people.PeopleDocType = tipoDoc;
                people.PeopleDocNum = response.NumeroDocumento.Trim();

                people.PeopleInternalID = response.NumeroDocumento.ToString();
            }

            //people.PeopleProject = null;
            people.PeopleLastName = $"{response.PrimerApellido.Trim()} {response.SegundoApellido.Trim()}";
            people.PeopleLastName2 = "";
            people.PeopleFirstName = response.Nombre.Trim();
            people.PeopleFirstName2 = "";
            people.PeopleAttribute3 = response.PuestoTrabajo.Trim();
            people.PeopleTypeClasificacion = PeopleDTO.CLAS_TYPE_COLABORADOR;
            people.PeopleIsSourceSAP = true;
            people.PeopleStartDate = response.FechaIngreso;
            //people.UserID = "";
            people.PeopleStatus = 1;
        }
        private PeopleDTO SavePeopleFromSAPResponse(SAPResponsePeopleDTO response, PeopleDTO people)
        {
            SetValuesFromSAPResponseToPeople(response, people);
            return PeopleRepository.SavePeople(people, people.PeopleCompany);
        }


        // User -----------------------------------------------------------------------------------------------------------
        private void SetValuesFromPeopleToUser(PeopleDTO people, AccessUserDTO user)
        {
            if (user.UserID == 0)
            {
                MailAddress email = new MailAddress(people.PeopleEmail.Trim());
                user.UserInternalID = email.User;
            }
            user.UserLastName = people.PeopleLastName.Trim();
            user.UserFirstName = people.PeopleFirstName.Trim();
            user.UserPassword = "";
            user.UserEMail = people.PeopleEmail.Trim();
            //user.UserDocNum = this.numeroDocumento.Trim();
            user.UserStatus = 1;

            // companies
            var userCompany = user.UserCompanies.FirstOrDefault(uc => uc.CompanyID == people.PeopleCompany);
            if (userCompany == null)
            {
                user.UserCompanies.Add(new AccessUserCompanyDTO()
                {
                    CompanyID = people.PeopleCompany,
                    UserRole1 = 1,           // Solicitante
                    UserRole2 = 0,
                    UserRole3 = 0,
                    UserRole4 = 0,
                    UserRole5 = 0,
                    UserRole6 = 0
                });
            }
            else
            {
                userCompany.UserRole1 = 1;   // Solicitante
            }
        }

        private void SaveUserFromPeople(PeopleDTO people, AccessUserDTO user)
        {
            // Validate email
            //var email = people.PeopleEmail.Trim();
            //Regex reg = new Regex(@"[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,6}", RegexOptions.IgnoreCase);
            //Match match = reg.Match(email);
            //if (!match.Success)
            //{
            //    return;   // Si no tiene correo no se actualiza al usuario
            //}

            SetValuesFromPeopleToUser(people, user);
            UserRepository.SaveAccessUser(
                user,                               // Nuevo usuario
                new AccessUsers()                   // Usuario creador : SAP
                {
                    UserInternalID = USER_ID,
                    UserRoleSysAdmin = 1
                }
            );
        }


        // Request --------------------------------------------------------------------------------------------------------
        private AccessRequestDTO BuildRequestFromTemplate(PeopleDTO people, int templateID)
        {
            var requestDTO = this.RequestTemplateRepository.BuildRequestDTOFromTemplate(templateID);
            requestDTO.RequestPriority = AccessRequestDTO.PRIORITY_NORMAL;
            requestDTO.RequestTo = people.PeopleID;
            requestDTO.RequestNote = $"[{System.DateTime.Now.ToString()} - {USER_ID}] Creado por el proceso automático de integración SAP.";

            // detalle  -------------------------------------------------------------------------
            SisConAxsContext db = new SisConAxsContext();
            var resPeopActive = ResourcePeopleRepository.GetByPeopleID(people.PeopleID, true);
            var reqDetCurrent = db.AccessRequestDetails.Where(d => d.AccessRequest.RequestTo == people.PeopleID && d.RequestDetStatus == AccessRequestDTO.STATUS_PENDING);

            // Se omiten los recursos que ya se tienen o que están como pendientes
            requestDTO.AccessRequestDetails = requestDTO.AccessRequestDetails.Where(d =>
                !resPeopActive.Any(rp => rp.ResourceID == d.ResourceID) &&
                !reqDetCurrent.Any(rd => rd.ResourceID == d.ResourceID)
            ).ToList();
            foreach (var item in requestDTO.AccessRequestDetails)
            {
                if (item.ReqDetTemporal > 0)
                {
                    item.ReqDetValidityFrom = people.PeopleStartDate;

                    var resource = db.AccessResources.FirstOrDefault(r => r.ResourceID == item.ResourceID);
                    if (resource.ResourceParameters.Any(rp => rp.ResourceParameterID == AccessResourceParameterDTO.INTEGRACION_ORACLE_ID))  // perfil Oracle
                    {
                        item.ReqDetValidityUntil = item.ReqDetValidityFrom?.AddYears(2);
                    }
                    else
                    {
                        item.ReqDetValidityUntil = new DateTime(2099, 12, 31);
                    }
                }
            }
            // detalle  -------------------------------------------------------------------------

            return requestDTO;
        }

        public void SyncUpdateFromServer()
        {
            throw new NotImplementedException();
        }
    }
}
