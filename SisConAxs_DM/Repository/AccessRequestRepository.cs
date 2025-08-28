using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using AutoMapper;
using System.Configuration;
using System.IO;
using SH = Microsoft.SharePoint.Client;
using System.Net;
using System.Security;
using System.Web;
using System.Globalization;
using SisConAxs_DM.Utils;
using System.Linq.Expressions;

namespace SisConAxs_DM.Repository
{
    public class AccessRequestApproveDTO
    {
        public int RequestID;
        public string RequestNote;
        public string AttentionTicket;
        public string OracleUser;
        public string OracleMenu;
        public HttpPostedFile FileAttached { get; set; }
        public List<AccessRequestApproveDetailsDTO> ApproveDetail;
    }

    // Filters
    public class AccessRequestDetailFilter
    {
        public int PeopleID { get; set; }
        public string RequestType { get; set; }
        public int? CompanyID { get; set; }
    }

    public class DownloadFileFilter
    {
        public string FileName { get; set; }
        public string RequestNumber { get; set; }
    }

    public class AccessRequestFilter
    {
        public string ViewType { get; set; }

        public int RequestID { get; set; } = 0;
        public string RequestNumber { get; set; } = "";
        public string RequestByName { get; set; } = "";
        public string RequestToName { get; set; } = "";
        public string RequestToProject { get; set; } = "";
        public string RequestToPosition { get; set; } = "";
        public string RequestToNumDoc { get; set; } // para visualizarse en la solicitud
        public string RequestTypeDisplay { get; set; } = "";
        public string RequestPriorityName { get; set; } = "";
        public string RequestStatusName { get; set; } = "";
        public string RequestDepartmentName { get; set; } = "";
        public string RequestDateDisplay { get; set; } = "";
        public string RequestCompletedDateDisplay { get; set; } = "";
        public string RequestNroItems { get; set; } = "";

        public string RequestDetailResource { get; set; } = "";
        public string RequestDetailCategory { get; set; } = "";

        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

    // Paging
    public class AccessRequestPagedData
    {
        public List<AccessRequestDTO> data { get; set; }
        public int totalServerItems { get; set; }
    }

    //
    public class AccesRequestExcelFilter
    {
        public List<int> listPeopleID;
        public bool deleteFromExcel;
    }

    public class AccessRequestRepository : AxsBaseRepository
    {

        public AccessRequestRepository(SessionData sessionData)
        {
            this.sessionData = sessionData;
            dbSet = db.AccessRequests;
        }

        //FTP CONST
        public static string FTP_URL = ConfigurationManager.AppSettings["ServiceFtpUrl"].ToString();
        public static string FTP_USER = ConfigurationManager.AppSettings["ServiceFtpUser"].ToString();
        public static string FTP_PASSWORD = ConfigurationManager.AppSettings["ServiceFtpPassword"].ToString();        
        private static readonly string ROL_APROBADOR = "UserRole2";

        /// <summary>Llamada para obtener la lista de AccessRequest.</summary>
        /// <para>GET api/AccessRequests </para>
        /// <returns>Lista de AccesType</returns>
        public AccessRequestPagedData GetAccessRequest(AccessRequestFilter filter, string UserID, int perms = 0)
        {
            AccessRequestPagedData pagedData = new AccessRequestPagedData();
            pagedData.data = db.Database.SqlQuery<AccessRequestDTO>(
                                "EXEC AXSCONTROL.SP_ACCESS_REQUESTS_GET {0}, {1}, {2}, @PAGE_SIZE = {3}, @CURRENT_PAGE = {4}, " +
                                "@REQUEST_ID = {5}, @F_NUMBER = {6}, @F_TO_NAME = {7}, @F_TO_PROJECT = {8}, @F_TO_POSITION = {9},  @F_TYPE_DISPLAY = {10}, @F_PRIORITY_NAME = {11}, @F_STATUS_NAME = {12}, @F_DEPARTMENT_NAME = {13}, @F_DATE = {14}, @F_COMPLETE_DATE = {15}, @F_NUM_ITEMS = {16}, " +
                                "@F_DETAIL_RESOURCE = {17}, @F_DETAIL_CATEGORY = {18}",
                                this.sessionData.CompanyID,
                                filter.ViewType,
                                UserID,
                                filter.PageSize,
                                filter.CurrentPage,

                                filter.RequestID,
                                filter.RequestNumber.Trim(),
                                filter.RequestToName.Trim(),
                                filter.RequestToProject.Trim(),
                                filter.RequestToPosition.Trim(),
                                //filter.RequestToNumDoc.Trim(), // para visualizarse en la solicitud

                                filter.RequestTypeDisplay.Trim(),
                                filter.RequestPriorityName.Trim(),
                                filter.RequestStatusName.Trim(),
                                filter.RequestDepartmentName.Trim(),
                                filter.RequestDateDisplay.Trim(),
                                filter.RequestCompletedDateDisplay.Trim(),
                                filter.RequestNroItems.Trim(),
                                filter.RequestDetailResource.Trim(),
                                filter.RequestDetailCategory.Trim()
                             ).ToList();

            var requestIDs = pagedData.data.Select(r => r.RequestID).ToList();
            var details = (from resource in db.AccessResources
                           join cat in db.ResourceCategories on resource.ResourceCategory equals cat.CategoryID
                           join ard in db.AccessRequestDetails on resource.ResourceID equals ard.ResourceID
                           where
                                requestIDs.Contains((int)ard.RequestID)
                           select new AccessRequestDetailsDTO()
                           {
                               RequestID = ard.RequestID,
                               ResourceCategoryName = cat.CategoryName,
                               ResourceFullName = ard.ResourceFullName,
                               RequestDetType = ard.RequestDetType,
                               RequestDetDisplayValue = ard.CommonValuesType.CommonValueDisplay
                           }).ToList();

            foreach (var item in pagedData.data)
            {
                item.AccessRequestDetails = details.FindAll(x => x.RequestID == item.RequestID);
            }
            pagedData.totalServerItems = pagedData.data.FirstOrDefault()?.RequestTotalRows ?? 0;
            return pagedData;
        }


        public AccessRequestDTO GetAccessRequestById(int id, string ViewType = "", string UserID = "")
        {
            var approverId = (from wfep in db.WFExecutionParameters
                              join wfe in db.WorkflowExecution on wfep.WfExecID equals wfe.WfExecID
                              join p in db.People on wfep.WfExecParamIntValue equals p.PeopleID
                              where wfe.WfExecParentObjectID == id 
                                && wfep.WfExecParamName == "approver"
                                && wfe.WfExecStatus == 2 // Esperando respuesta en una consulta
                                && p.UserID == UserID
                              select wfep.WfExecParamIntValue).Distinct().FirstOrDefault();

            AccessRequestDTO request =
                (from ar in db.AccessRequests
                 join reqByData in db.People on ar.RequestBy equals reqByData.UserID into RequestBy
                 from reqBy in RequestBy.DefaultIfEmpty()
                 join reqTo in db.People on ar.RequestTo equals reqTo.PeopleID
                 join reqToCompany in db.Companies on ar.RequestToCompany equals reqToCompany.CompanyID
                 join type in db.CommonValues on ar.RequestType equals type.CommonValueID
                 join priority in db.CommonValues on ar.RequestPriority equals priority.CommonValueID
                 join status in db.CommonValues on ar.RequestStatus equals status.CommonValueID
                 join user in db.AccessUsers on ar.RequestBy equals user.UserInternalID
                 join department in db.CommonValues on ar.RequestDepartment equals department.CommonValueID into departments
                 from department in departments.DefaultIfEmpty()
                 where ar.RequestID == id
                 select new AccessRequestDTO()
                 {
                     RequestID = ar.RequestID,
                     RequestNumber = ar.RequestNumber,
                     FlagIsApprover = ar.RequestTo == approverId ? 1 : 0,

                     RequestBy = ar.RequestBy,
                     RequestByName = reqBy != null ? reqBy.PeopleLastName + " " + reqBy.PeopleLastName2 + ", " +
                                                 reqBy.PeopleFirstName + " " + reqBy.PeopleFirstName2
                                                 : ar.RequestBy,
                     RequestByProject = ar.RequestByProject,
                     RequestByDepartment = ar.RequestByDepartment,
                     RequestByPosition = ar.RequestByPosition,

                     RequestTo = ar.RequestTo,
                     RequestToName = reqTo.PeopleLastName + " " + reqTo.PeopleLastName2 + ", " + reqTo.PeopleFirstName + " " + reqTo.PeopleFirstName2,
                     RequestToEmail = reqTo.PeopleEmail,
                     RequestToCompany = ar.RequestToCompany,
                     RequestToCompanyName = reqToCompany.CompanyName,
                     RequestToProject = ar.RequestToProject,
                     RequestToDepartment = ar.RequestToDepartment,
                     RequestToPosition = ar.RequestToPosition,
                     RequestToNumDoc = reqTo.PeopleDocNum,

                     RequestType = ar.RequestType,
                     RequestTypeName = type.CommonValueName,
                     RequestTypeDisplay = type.CommonValueDisplay,
                     RequestPriority = ar.RequestPriority,
                     RequestPriorityName = priority.CommonValueDisplay,
                     RequestStatus = ar.RequestStatus,
                     RequestStatusName = status.CommonValueDisplay,
                     RequestDepartment = ar.RequestDepartment,
                     RequestDepartmentName = department.CommonValueDisplay,
                     RequestDate = ar.RequestDate,
                     RequestCompletedDate = ar.RequestCompletedDate,
                     RequestNote = ar.RequestNote,
                     FileAttachedString = ar.RequestAttached,
                     AttentionTicket = ar.AttentionTicket,
                     OracleUser = ar.OracleUser,
                     OracleMenu = ar.OracleMenu
                 }).FirstOrDefault();

            if (request != null)
            {
                request.AccessRequestDetails = db.Database.SqlQuery<AccessRequestDetailsDTO>("EXEC AXSCONTROL.SP_ACCESS_REQUEST_DETAILS_GET {0}, {1}, {2}", id, ViewType, UserID).ToList();
            }
            return request;
        }


        public List<AccessRequestDetailsDTO> GetAccessRequestDetails(int PeopleID, string RequestType, int? CompanyID = null)
        {
            int RequestTypeID = 0;
            if (RequestType == "ST_ALTA")
                RequestTypeID = (int)AccessRequestDTO.TYPE_ALTA;
            else if (RequestType == "ST_MODIFICACION")
                RequestTypeID = (int)AccessRequestDTO.TYPE_MODIFICACION;
            else if (RequestType == "ST_BAJA")
                RequestTypeID = (int)AccessRequestDTO.TYPE_BAJA;

            // Query ----------------------------------------------------------------------------------------------------------------- //
            //esto solo se utiliza cuando se esta creando una nueva solicitud
            List<AccessTypeValues> AccessTypeValues =
                (from at in db.AccessTypes
                 join atv in db.AccessTypeValues on at.AccessTypeID equals atv.AccessTypeID
                 //where acv.AccessTypeID == at.AccessTypeID
                 where
                 at.AccessTypeCompany == this.sessionData.CompanyID
                 orderby
                 atv.TypeValueDisplay
                 select atv).ToList();

            // Resources
            List<AccessRequestDetailsDTO> query = db.Database.SqlQuery<AccessRequestDetailsDTO>("EXEC AXSCONTROL.SP_GENERATE_REQUEST_DETAILS {0}, {1}, {2}", PeopleID, RequestTypeID, CompanyID.HasValue ? CompanyID : this.sessionData.CompanyID).ToList();
            foreach (var item in query)
            {
                item.ResourceAccessTypeValues = Mapper.Map<ICollection<AccessTypeValuesDTO>>(AccessTypeValues.Where(x => x.AccessTypeID == item.ResourceAccessTypeID));
            }
            return query;
        }


        public IQueryable<AccessRequestDetailHistoryDTO> GetDetailHistory(int RequestDetailID)
        {
            var query = from his in db.WorkflowExecutionHistory
                        where
                            his.WfExecObjectName == "ACCESS_REQUEST" && his.WfExecObjectID == RequestDetailID
                        orderby
                            his.WfExecHistoryID
                        select new AccessRequestDetailHistoryDTO
                        {
                            HistoryDate = DateTime.Now,
                            HistoryMessage = his.WfExecHistoryMessage
                        };
            return query;
        }


        public CommonValuesDTO GetHaveAccess(int PeopleID, string RequestType, AccessUsers user)
        {
            CommonValuesDTO commonvalue = null;
            var haveAccess = false;
            var query = GetAccessRequestDetails(PeopleID, RequestType);
            if (query != null)
                haveAccess = query.Any(x => x.RequestDetPrevData || x.RequestDetPending);

            if (haveAccess)
            {
                if (RequestType == "ST_ALTA")
                {
                    var commonvalues = new CommonValueSetsRepository(this.sessionData).GetCommonValuesBySet("TIPO_SOLICITUD", user);
                    if (commonvalues != null)
                    {
                        commonvalue = commonvalues.FirstOrDefault(x => x.CommonValueName == "ST_MODIFICACION");
                    }
                }
            }
            else
            {
                if (RequestType == "ST_MODIFICACION" || RequestType == "ST_BAJA")
                {
                    var commonvalues = new CommonValueSetsRepository(this.sessionData).GetCommonValuesBySet("TIPO_SOLICITUD", user);
                    if (commonvalues != null)
                    {
                        commonvalue = commonvalues.FirstOrDefault(x => x.CommonValueName == "ST_ALTA");
                    }
                }
            }
            return commonvalue;
        }

        public AccessRequestDTO InsertAccessRequest(AccessRequestDTO dto, string userId, int? companyID = null)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    AccessRequests model = null;
                    if (dto.RequestID == 0)
                        model = db.AccessRequests.Create();  // create new from context
                    else
                    {
                        throw new ModelException("Esta solicitud ya ha sido grabada.");
                    }

                    CommonValues cv;
                    Mapper.Map<AccessRequestDTO, AccessRequests>(dto, model);
                    model.RequestStatus = AccessRequestDTO.STATUS_PENDING;
                    model.EditUser = userId;
                    model.RequestBy = userId;
                    model.RequestCompany = companyID ?? this.sessionData.CompanyID;

                    // Datos del solicitante -----------------------------------------------------------------------------
                    People requestBy = db.People.FirstOrDefault(p => p.UserID == model.RequestBy);
                    if (requestBy != null)
                    {
                        cv = db.CommonValues.FirstOrDefault(c => c.CommonValueID == requestBy.PeopleProject);
                        model.RequestByProject = cv != null ? cv.CommonValueDisplay : null;

                        cv = db.CommonValues.FirstOrDefault(c => c.CommonValueID == requestBy.PeopleDepartment);
                        model.RequestByDepartment = cv != null ? cv.CommonValueDisplay : null;

                        model.RequestByPosition = requestBy.PeopleAttribute3;  // position
                    }
                    else
                    {
                        model.RequestByProject = null;
                        model.RequestByDepartment = null;
                        model.RequestByPosition = null;
                    }

                    // Datos del solicitado para -------------------------------------------------------------------------
                    People requestTo = db.People.FirstOrDefault(p => p.PeopleID == model.RequestTo);
                    if (requestTo != null)
                    {
                        model.RequestToCompany = requestTo.PeopleCompany;  // Empresa actual de la persona (se registra como histórico, se puede pedir permisos de una empresa A a una persona que este en la empresa B)

                        cv = db.CommonValues.FirstOrDefault(c => c.CommonValueID == requestTo.PeopleProject);
                        model.RequestToProject = cv != null ? cv.CommonValueDisplay : null;

                        cv = db.CommonValues.FirstOrDefault(c => c.CommonValueID == requestTo.PeopleDepartment);
                        model.RequestDepartment = cv != null ? (int?)cv.CommonValueID : null;
                        model.RequestToDepartment = cv != null ? cv.CommonValueDisplay : null;

                        model.RequestToPosition = requestTo.PeopleAttribute3;  // position
                    }
                    else
                    {
                        model.RequestToProject = null;
                        model.RequestToDepartment = null;
                        model.RequestToPosition = null;

                    }

                    PrepareDetail(model, dto);

                    if (Validate(model, dto, companyID))
                    {
                        var fileAttached = dto.FileAttached;
                        if (SaveEntity(true, model))
                        {
                            dto = GetAccessRequestById(model.RequestID);
                            if (fileAttached != null)
                            {
                                dto.FileAttached = fileAttached;
                                UpdateAccessRequest(dto, userId);
                                dto.FileAttached = null;
                            }
                            SaveWorkflowExecutionHistory(dto, companyID);
                        }
                    }

                    transaction.Commit();
                    return dto;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ModelException(ex.Message);
                }
            }
        }

        public void UpdateAccessRequest(AccessRequestDTO dto, string userId)
        {
            //using (transaction = transaction ?? db.Database.BeginTransaction())
            //{
            try
            {
                var model = db.AccessRequests.FirstOrDefault(x => x.RequestID == dto.RequestID);
                if (model == null)
                {
                    throw new ModelException("La solicitud a actualizar no existe");
                }

                model.RequestNote = dto.RequestNote;
                model.AttentionTicket = dto.AttentionTicket;
                model.OracleUser = dto.OracleUser;
                model.OracleMenu = dto.OracleMenu;
                model.EditUser = userId;
                if (dto.FileAttached != null)
                {
                    if (!ExisteDirectorio(model.RequestNumber.ToString("00000000")))
                    {

                        var result = CreateFolder(dto.FileAttached, model.RequestNumber.ToString("00000000"), String.IsNullOrWhiteSpace(model.RequestAttached));

                        if (result.StatusCode.ToString() == "PathnameCreated")
                        {
                            var urlSharepoint = SaveFileToFTP(dto.FileAttached, model.RequestNumber.ToString("00000000"));
                            model.RequestAttached = (!String.IsNullOrWhiteSpace(model.RequestAttached) ? model.RequestAttached + ";" + urlSharepoint : urlSharepoint);
                        }

                    }
                    else
                    {
                        var urlSharepoint = SaveFileToFTP(dto.FileAttached, model.RequestNumber.ToString("00000000"));
                        model.RequestAttached = (!String.IsNullOrWhiteSpace(model.RequestAttached) ? model.RequestAttached + ";" + urlSharepoint : urlSharepoint);

                    }
                }
                SaveEntity(false, model);
            }
            catch (Exception ex)
            {
                throw new ModelException("Error al grabar la actualización de la Solicitud", ex);
            }
            //}
        }

        public bool ApproveAccessRequest(AccessRequestApproveDTO dtoApprove)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    WorkflowExecution model = null;
                    foreach (var item in dtoApprove.ApproveDetail)
                    {
                        model = db.WorkflowExecution.FirstOrDefault(w => item.RequestID == (int)w.WfExecParentObjectID && item.RequestDetID == w.WfExecObjectID);
                        var param = db.WFExecutionParameters.FirstOrDefault(p => p.WfExecID == model.WfExecID
                                                                                 && p.WfExecParamName == "approver");
                        var people = db.People.FirstOrDefault(x => x.PeopleID == param.WfExecParamIntValue);
                        if (people.UserID.Trim().ToUpper() == this.sessionData.User.UserInternalID.Trim().ToUpper())
                        {
                            model.WfResponse = item.RequestResponse;
                            model.WfExecStatus = 3;         /// 3: Respuesta recibida
                            //Adicionado el 20191107 <-- DIANA CAMUS
                            new WorkflowExecutionRepository().SaveWFExecParam(model.WfExecID, "approver_by", people.PeopleID, people.GetFullName(), DateTime.Now); //Id del aprobador

                            db.WorkflowExecution.Attach(model);
                            db.Entry(model).State = EntityState.Modified;

                        }
                        else
                        {
                            var detail = db.AccessRequestDetails.FirstOrDefault(x => x.RequestDetID == item.RequestDetID);
                            throw new ModelException($"No tiene permiso para aprobar/rechazar este item {detail.AccessResources.ResourceFullName}");
                        }
                    }


                    db.SaveChanges();

                    //actualizar la observaciones y adjuntos
                    AccessRequestDTO dto = this.GetAccessRequestById(dtoApprove.RequestID);
                    PeopleDTO ppl = new PeopleRepository(this.sessionData).GetPeopleByUserId(this.sessionData.User.UserInternalID);

                    dto.AttentionTicket = dtoApprove.AttentionTicket;
                    dto.OracleUser = dtoApprove.OracleUser;
                    dto.OracleMenu = dtoApprove.OracleMenu;

                    if (!String.IsNullOrWhiteSpace(dtoApprove.RequestNote) || dtoApprove.FileAttached != null)
                    {
                        if (!String.IsNullOrWhiteSpace(dtoApprove.RequestNote))
                        {
                            string NoteAdd = $"[{System.DateTime.Now.ToString()} - {(!String.IsNullOrWhiteSpace(ppl.PeopleLastName) ? ppl.PeopleLastName : "")} {(!String.IsNullOrWhiteSpace(ppl.PeopleFirstName) ? ppl.PeopleFirstName : "")}]" +
                                             $" {dtoApprove.RequestNote}";
                            dto.RequestNote = String.IsNullOrWhiteSpace(dto.RequestNote) ? NoteAdd : $"{dto.RequestNote}\n{NoteAdd}";
                        }

                        //guardamos el archivo sharepoint
                        if (dtoApprove.FileAttached != null)
                        {
                            dto.FileAttached = dtoApprove.FileAttached;
                        }
                    }
                    this.UpdateAccessRequest(dto, this.sessionData.User.UserInternalID);
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ModelException(ex.Message);
                }
            }
        }


        private void PrepareDetail(AccessRequests model, AccessRequestDTO dto)
        {
            List<AccessRequestDetails> listToRemove = new List<AccessRequestDetails>();

            // Obtenemos la lista de los recursos asignados
            List<ResourcePeople> activeResourcePeople = db.ResourcePeople.Where(rp => rp.PeopleID == model.RequestTo
                                                                                      && rp.PresActive == 1)
                                                          .ToList();

            foreach (AccessRequestDetails item in model.AccessRequestDetails)
            {
                // Obtenemos el Recurso para el que se solicita el acceso
                var resource = db.AccessResources.FirstOrDefault(t => t.ResourceID == item.ResourceID);

                if (resource == null)
                    throw new ModelException(String.Format("El recurso con el id '{0}' no existe.", item.ResourceID));

                // Verificamos si este recurso tiene un Recurso Requerido --------------------------------------------------------------------- //
                if (resource.ResourceRequired != null)
                {
                    // Buscamos el Recurso Requerido
                    var requiredResource = db.AccessResources.First(r => r.ResourceID == resource.ResourceRequired);

                    // Verificamos si en esta solicitud se está pidiendo la ALTA del Recurso Requerido
                    var reqDetailRequiredResource = dto.AccessRequestDetails.FirstOrDefault(rr => rr.ResourceID == resource.ResourceRequired &&
                                                                                                  rr.RequestDetType == AccessRequestDTO.TYPE_ALTA);

                    // También verificamos si la persona ya tenía previamente el Recurso Requerido
                    var resourcePeopleRequired = activeResourcePeople.FirstOrDefault(rp => rp.ResourceID == resource.ResourceRequired);

                    // Si no tenemos Recurso Requerido ya sea en el detalle de solicitud como alta o en los recursos asignados para esta persona, devolvemos un error
                    if ((reqDetailRequiredResource == null && resourcePeopleRequired == null) && (item.RequestDetType == AccessRequestDTO.TYPE_ALTA || item.RequestDetType == 1))
                    {
                        throw new ModelException(String.Format("El recurso '{0}' requiere que se asigne también '{1}'.", resource.ResourceFullName, requiredResource.ResourceFullName));
                    }
                }
                // Verificamos si este recurso tiene un Recurso Requerido --------------------------------------------------------------------- //

                item.ReqDetSendAtEnd = resource.ResourceSendAtEnd;
                item.EditUser = model.EditUser;
                if (model.RequestType == AccessRequestDTO.TYPE_ALTA || model.RequestType == AccessRequestDTO.TYPE_BAJA)  // Si es ALTA o BAJA, se establecen a TODOS los detalles como el mismo tipo
                {
                    item.RequestDetType = model.RequestType;
                }
                else if (model.RequestType == AccessRequestDTO.TYPE_MODIFICACION)
                {
                    if (item.RequestDetType == 0 && resource.ResourceActive == 1)
                        item.RequestDetType = AccessRequestDTO.TYPE_BAJA;
                    else if (item.RequestDetType == 0 && resource.ResourceActive == 0)
                        item.RequestDetType = -1;//Evita la baja automatica del recurso inactivo
                    else if (item.RequestDetType == 1)
                        item.RequestDetType = AccessRequestDTO.TYPE_ALTA;
                }
            }

            //JLP 20180227 - Evita la baja automatica de recursos inactivos
            model.AccessRequestDetails = (from x in model.AccessRequestDetails
                                          where x.RequestDetType != -1
                                          select x).ToList();
        }

        private bool Validate(AccessRequests model, AccessRequestDTO dto, int? companyID = null)
        {
            AccessResources resource;

            // validate access request items
            int count = 0;
            count = dto.AccessRequestDetails.Count;
            if (count == 0)
            {
                throw new ModelException("No se puede grabar la solicitud sin detalles.");
            }

            // validate approver
            People requestBy = db.People.FirstOrDefault(p => p.UserID == model.RequestBy);
            foreach (var detail in dto.AccessRequestDetails)
            {
                if(detail.RequestDetType == AccessRequestDTO.TYPE_ALTA || detail.RequestDetType == AccessRequestDTO.TYPE_MODIFICACION) {
                    resource = db.AccessResources.FirstOrDefault(r => r.ResourceID == detail.ResourceID);
                    WorkflowApproveHierarchy hierarchy = resource.Workflows.WorkflowApproveHierarchy;
                    bool isHierarchyMember = hierarchy.WorkflowHierarchyMembers.Any(m => m.WfHierarchyMemberDepartment == requestBy.PeopleDepartment
                                                                                         && m.WfHierarchyMemberPosition == requestBy.PeoplePosition);
                    if (isHierarchyMember)
                    {
                        throw new ModelException($"No puede solicitar el recurso '{resource.ResourceFullName}' porque su usuario es parte de la jerarquía de aprobación.");
                    }
                }
            }

            // validate Oracle Resources
            var resourcesIDs = dto.AccessRequestDetails.Select(d => d.ResourceID);
            var hasOracleResources = db.AccessResources.Any(r => r.ResourceCategory == Constants.CATEGORY_ORACLE_ERP && resourcesIDs.Contains(r.ResourceID));
            if(hasOracleResources)
            {
                var people = db.People.FirstOrDefault(p => p.PeopleID == dto.RequestTo);
                if(!(people.PeopleTypeClasificacion == PeopleDTO.CLAS_TYPE_COLABORADOR || people.PeopleTypeClasificacion == PeopleDTO.CLAS_TYPE_COLABORADOR_EN_PROCESO))
                {
                    throw new ModelException($"No puede seleccionar recursos de Oracle si el destinatario NO es de Tipo Colaborador o Colaborador Manual.");
                }
                if (String.IsNullOrWhiteSpace(people.UserID))
                {
                    throw new ModelException($"No puede seleccionar recursos de Oracle si el destinatario NO tiene un usuario válido.");
                }
            }

            // validate resource and workflow
            resource = new AccessResources();
            foreach (AccessRequestDetailsDTO item in dto.AccessRequestDetails)
            {
                resource = db.AccessResources.FirstOrDefault(t => t.ResourceID == item.ResourceID
                                                                  && t.ResourceCompany == (companyID ?? this.sessionData.CompanyID));
                if (resource == null)
                {
                    throw new ModelException($"El recurso '{item.ResourceName}' no existe.");
                }
                if (resource.Workflows == null)
                {
                    throw new ModelException($"No se puede grabar porque el recurso '{resource.ResourceName}' no tiene workflow.");
                }
                count = resource.Workflows.WorkflowItems.Count;
                if (count == 0)
                {
                    throw new ModelException($"No se puede grabar porque el workflow '{resource.Workflows.WfName}' del recurso '{resource.ResourceName}' no tiene acciones.");
                }
            }

            //var allPeopleDetail = db.AccessRequestDetails.Where(ard => ard.AccessRequest.RequestTo == dto.RequestTo 
            //                                                        && ard.RequestDetStatus == AccessRequestDTO.STATUS_PENDING
            //                                                        && ard.RequestDetType == AccessRequestDTO.TYPE_BAJA
            //                                                    );
            //foreach (var item in dto.AccessRequestDetails)
            //{
            //    if (item.RequestDetType == AccessRequestDTO.TYPE_BAJA)
            //    {
            //        var havePendingResource = allPeopleDetail.Any(ard => ard.ResourceID == item.ResourceID);

            //        if (havePendingResource)
            //        {
            //            throw new ModelException($"No se puede grabar porque YA EXISTE una solicitud de baja en curso para el recurso '{item.ResourceFullName}' solicitado.");
            //        }
            //    }
            //}

            // validate approver access resource and resource pending to approve
            var resourceDetail = dto.AccessRequestDetails.Where(r => r.ResourceParamValue != null && r.ResourceParamValue.Contains(ROL_APROBADOR)).ToList();
            if (resourceDetail.Count > 0)
            {
                var resourcePeople = (from rp in db.ResourcePeople
                                      join rparam in db.AccessResourceParameters on rp.ResourceID equals rparam.ResourceID
                                      join ac in db.AccessResources on rparam.ResourceID equals ac.ResourceID
                                      join arc in db.ResourceCategories on ac.ResourceCategory equals arc.CategoryID
                                      where rp.PeopleID == model.RequestTo
                                      && arc.CategoryID == Constants.CATEGORY_SISTEMAS_INFORMACION_ALTAS_USUARIOS
                                      && rp.PresActive == 1
                                      select new
                                      {
                                          rp.ResourceID,
                                          rp.ResourceFullName,
                                          rparam.Value
                                      }).ToList();

                var resourcePeoplePending = (from ar in db.AccessRequests
                                             join ard in db.AccessRequestDetails on ar.RequestID equals ard.RequestID
                                             join rparam in db.AccessResourceParameters on ard.ResourceID equals rparam.ResourceID
                                             join ac in db.AccessResources on rparam.ResourceID equals ac.ResourceID
                                             join arc in db.ResourceCategories on ac.ResourceCategory equals arc.CategoryID
                                             where ar.RequestTo == dto.RequestTo
                                             && ard.RequestDetStatus == AccessRequestDTO.STATUS_PENDING
                                             && arc.CategoryID == Constants.CATEGORY_SISTEMAS_INFORMACION_ALTAS_USUARIOS
                                             select new
                                             {
                                                ard.ResourceID,
                                                ard.ResourceFullName,
                                                rparam.Value
                                             }).ToList();

                foreach (AccessRequestDetailsDTO item in resourceDetail)
                {
                    if (item.RequestDetType == AccessRequestDTO.TYPE_ALTA || item.RequestDetType == AccessRequestDTO.TYPE_MODIFICACION)
                    {
                        var paramValue = item.ResourceParamValue.Split('-')[0];
                        bool tieneAprobador = resourcePeople.Any(rp => rp.Value.Contains(paramValue));

                        if (tieneAprobador)
                        {
                            throw new ModelException($"Este usuario ya cuenta con un perfil de aprobador en otra empresa. No se puede grabar porque solo se puede tener rol de aprobador en una sola empresa.");
                        }

                        bool tieneRecursoPendiente = resourcePeoplePending.Any(rp => rp.Value.Contains(paramValue));

                        if (tieneRecursoPendiente)
                        {
                            throw new ModelException($"Este usuario ya cuenta con una solicitud de perfil de aprobador pendiente en otra empresa. No se puede grabar porque solo se puede tener rol de aprobador en una sola empresa.");
                        }
                    }
                }
            }
            return true;
        }

        public bool DeleteAccessRequest(int id)
        {
            AccessRequests model = db.AccessRequests.FirstOrDefault(a => a.RequestID == id);
            return DeleteEntity(model);
        }

        private void SaveWorkflowExecutionHistory(AccessRequestDTO accessRequest, int? companyID = null)
        {
            string createUserName = "";
            DateTime createDate = DateTime.Now;

            //People people = db.People.FirstOrDefault(p => p.PeopleID == approverID);

            WorkflowExecutionHistory wfExecHistory = db.WorkflowExecutionHistory.Create();
            wfExecHistory.WfExecWfID = null;        // La Solicitud no tiene Workflow
            wfExecHistory.WfExecCurrentStep = 0;    // La Solicitud no tiene Pasos
            wfExecHistory.WfResponse = -1;          // No hay respuesta para Workflow
            wfExecHistory.WfExecStartDate = createDate;
            wfExecHistory.WfExecStartedBy = accessRequest.RequestBy;
            wfExecHistory.WfExecParentObject = accessRequest.RequestID;   // La solicitud no tiene Padre, el mismo
            wfExecHistory.WfExecObjectID = 0;
            wfExecHistory.WfExecObjectName = "ACCESS_REQUEST";
            wfExecHistory.WfExecObjectStatus = 0;
            wfExecHistory.WfExecHistoryMessage = String.Format("[{0}] Solicitud de Acceso número [{1}] creada por [{2}].",
                                                               DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                                                               accessRequest.RequestNumber,
                                                               createUserName);
            wfExecHistory.WfExecCompany = companyID ?? this.sessionData.CompanyID;
            //wfExecHistory.
            db.WorkflowExecutionHistory.Add(wfExecHistory);

            foreach (AccessRequestDetailsDTO reqDetail in accessRequest.AccessRequestDetails)
            {
                WorkflowExecutionHistory wfExecHistoryDet = db.WorkflowExecutionHistory.Create();

                AccessResources resource = db.AccessResources.FirstOrDefault(r => r.ResourceID == reqDetail.ResourceID);

                wfExecHistoryDet.WfExecWfID = resource.ResourceWorkflow; //reqDetail.AccessResources.ResourceWorkflow;
                wfExecHistoryDet.WfExecCurrentStep = 0;
                wfExecHistoryDet.WfResponse = -1;          // No hay respuesta para Workflow
                wfExecHistoryDet.WfExecStartDate = createDate;
                wfExecHistoryDet.WfExecStartedBy = accessRequest.RequestBy;
                wfExecHistoryDet.WfExecParentObject = accessRequest.RequestID;
                //wfExecHistoryDet.WfExecObjectID = reqDetail.AddedRequestDetID;
                wfExecHistoryDet.WfExecObjectName = "ACCESS_REQUEST";
                wfExecHistoryDet.WfExecObjectStatus = 0;
                wfExecHistoryDet.WfExecHistoryMessage = String.Format("[{0}] Se solicitó acceso a [{1}].",
                                                                    DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                                                                    reqDetail.ResourceFullName);
                wfExecHistoryDet.WfExecCompany = companyID ?? this.sessionData.CompanyID;
                //reqDetail.AccessResources.ResourceFullName);
                db.WorkflowExecutionHistory.Add(wfExecHistoryDet);
            }
            db.SaveChanges();
        }

        public bool CancelRequest(int RequestID, string userID)
        {
            bool success = false;
            SisConAxsContext db = new SisConAxsContext();
            AccessRequests request = db.AccessRequests.FirstOrDefault(x => x.RequestID == RequestID);

            if ((this.sessionData.HavePermission(UserRole.ADMIN) || request.RequestBy == userID) && request.RequestCompletedDate == null)
            {
                var workflowExecution = db.WorkflowExecution.Where(t => t.WfExecStatus < 10 //&& t.WfExecStatus != (int)WfExecStatus.ToProcess
                                                                        && t.WfExecObjectName == "ACCESS_REQUEST"
                                                                        && t.WfExecParentObjectID == RequestID);
                foreach (WorkflowExecution wfExec in workflowExecution)
                {
                    wfExec.WfExecStatus = (int)WfExecStatus.CancelRequest;
                    AccessRequestDetails det = db.AccessRequestDetails.FirstOrDefault(x => x.RequestDetID == wfExec.WfExecObjectID);
                    if (det != null)
                    {
                        det.RequestDetParam01 = string.Format("Solicitud detalle {0} anulada por : {1} el {2}", det.RequestDetID, userID, DateTime.Now.ToString("dd/MM/yyy HH:mm:ss"));
                        det.EditUser = userID;
                    }
                    db.WorkflowExecution.Attach(wfExec);
                    db.Entry(wfExec).State = EntityState.Modified;
                }

                if (request.AccessRequestDetails.Count == 0)
                {
                    request.RequestStatus = AccessRequestDTO.STATUS_ANNUL; // ANULADO
                    request.RequestCompletedDate = DateTime.Now;
                }
                request.EditUser = userID;
                if (workflowExecution.Count() > 0 || request.AccessRequestDetails.Count == 0)
                    db.SaveChanges();
                success = true;
                return success;
            }
            else
            {
                throw new ModelException("No tiene permiso para anular esta solicitud");
            }
        }

        /**
         * summary Método que cancela las solicitudes pendientes de un destinatario
         * returns -1 con errores, 0 sin ejecuciones, 1 ejecutado correctamente
         */
        public int CancelAllRequestByPeople(int peopleID, string userID)
        {
            List<bool> results = new List<bool>();
            var listRequests = from req in db.AccessRequests
                               where
                                 req.RequestTo == peopleID && (req.RequestStatus == AccessRequestDTO.STATUS_PENDING || req.RequestStatus == AccessRequestDTO.STATUS_PARCIAL)
                               select req;
            foreach (var r in listRequests)
            {
                var resultCancel = CancelRequest(r.RequestID, userID);
                results.Add(resultCancel);
            }

            if (results.Count == 0) return 0;
            else if (results.Exists(r => !r)) return -1;
            else return 1;
        }

        private bool ValidateDeactivateExcelFormat(DataTable peopleDT)
        {
            if (peopleDT == null)
                throw new ModelException("El formato del archivo no es válido");

            bool isValid = true;
            string[] columNames = { "Nombre lugar trabajo", "ID CC", "ID Unidad org#", "ID RH", "Nombre", "Fecha Ingreso", "Fecha Cese", "Nombre de Puesto", "Nombre categoría puesto" };
            for (int idx = 0; idx < columNames.Length; idx++)
            {
                isValid = isValid && peopleDT.Columns.IndexOf(columNames[idx]) == idx;
            }
            if (!isValid)
                throw new ModelException("El formato del archivo no es válido");
            return true;
        }

        public bool MassiveDeactivateAccessFromExcel()
        {
            // Validation
            if (!(this.sessionData.UserRole3 > 0 || this.sessionData.UserRole5 > 0))  // rol "admnistrador" o "dar de baja"
            {
                throw new ModelException("No tiene permiso para realizar la baja.");
            }

            List<PeopleDTO> peopleList = (new PeopleRepository(this.sessionData)).getPeopleListFromExcel(
                AppDomain.CurrentDomain.BaseDirectory +
                ConfigurationManager.AppSettings["FilePath"],
                ConfigurationManager.AppSettings["FileName"],
                "Cesados$",
                ValidateDeactivateExcelFormat);
            if (peopleList.Count == 0)
                throw new ModelException("No hay personas en la lista.");

            string userID = this.sessionData.sessionUser;
            SisConAxsContext db2 = new SisConAxsContext();

            List<PeopleDTO> peopleList2 = new List<PeopleDTO>();//
            PeopleRepository pplRepo = new PeopleRepository(this.sessionData);
            IQueryable<AccessResources> list = db.AccessResources.Where(p => p.ResourceFlag == 1);

            foreach (PeopleDTO people in peopleList)//Llenamos peopleList2(donde todas la personas ya tienen un ID)
            {
                if (people.PeopleID == 0)
                {  //Lo creamos
                    //peopleList2.Add(pplRepo.SavePeople(people, user));
                }
                else
                {
                    //people.PeopleStatus = 0;
                    //peopleList2.Add(pplRepo.SavePeople(people, user));
                    peopleList2.Add(people);
                }

            }

            //foreach (PeopleDTO people in peopleList2)
            //{//verificar si tiene activos asignados o pendientes()si no tiene nada asignarle los recursos con flag
            //    if ((people.PendingApproveItems == null && people.AssignedItems==null) || (people.PendingApproveItems.Count == 0 && people.AssignedItems.Count == 0))
            //    {//No tiene nada asignado, asignarle los recursos con flag
            //        foreach (AccessResources resource in list)
            //        {
            //            ResourcePeople model = db2.ResourcePeople.Create();
            //            model.PeopleID = people.PeopleID;
            //            model.ResourceID = resource.ResourceID;
            //            model.AddedRequestID = null;
            //            model.AddedRequestDetID = null;
            //            db2.ResourcePeople.Add(model);
            //            db2.SaveChanges();
            //        }
            //    }
            //}

            List<int> peopleIds = peopleList2.Where(x => x.PeopleStatus == 1).Select(x => x.PeopleID).ToList();
            if (peopleIds == null || peopleIds.Count == 0)
            {
                throw new ModelException("Todas las personas ya fueron dadas de baja.");
            }

            MassiveDeactivateAccess(peopleIds);
            return true;
        }

        public bool MassiveDeactivateAccess(List<int> listPeopleID)
        {
            string userID = this.sessionData.sessionUser;

            // Validation
            //!(user.UserRole3 > 0 || user.UserRole5 > 0)
            if (this.sessionData.UserRole5 == 0)  // rol "dar de baja"
            {
                throw new ModelException("No tiene permiso para realizar la baja.");
            }
            if (listPeopleID == null || listPeopleID.Count == 0)
            {
                throw new ModelException("No ha seleccionado personas para la baja.");
            }

            bool success = false;
            AccessRequestDTO requestInfo = new AccessRequestDTO()
            {
                RequestBy = userID,
                RequestType = AccessRequestDTO.TYPE_BAJA, //ST_BAJA,
                RequestPriority = AccessRequestDTO.PRIORITY_NORMAL //SP_NORMAL
                //,RequestNote = "Realizado por Baja Masiva."
            };
            listPeopleID = listPeopleID.Distinct().ToList();

            PeopleRepository peopleRepository = new PeopleRepository(this.sessionData);

            foreach (int peopleID in listPeopleID)
            {
                // cancel current requests
                var listRequests = from req in db.AccessRequests
                                   where
                                     req.RequestTo == peopleID && (req.RequestStatus == AccessRequestDTO.STATUS_PENDING || req.RequestStatus == AccessRequestDTO.STATUS_PARCIAL)
                                   select req;
                foreach (AccessRequests request in listRequests)
                {
                    success = CancelRequest(request.RequestID, userID);
                    if (!success) break;
                }

                // save request
                requestInfo.RequestID = 0;
                requestInfo.RequestNumber = 0;
                requestInfo.RequestTo = peopleID;
                requestInfo.AccessRequestDetails = (from resourcePeople in db.ResourcePeople
                                                    where
                                                      resourcePeople.PeopleID == peopleID
                                                      && resourcePeople.PresCompany == this.sessionData.CompanyID
                                                      && resourcePeople.PresActive > 0
                                                    select new AccessRequestDetailsDTO()
                                                    {
                                                        ResourceID = resourcePeople.ResourceID,
                                                        RequestDetType = AccessRequestDTO.TYPE_BAJA, // ST_BAJA,
                                                        RequestDetIntValue = resourcePeople.PresIntValue,
                                                        RequestDetStrValue = resourcePeople.PresStrValue,
                                                        ReqDetTemporal = resourcePeople.PresTemporal,
                                                        ReqDetValidityFrom = resourcePeople.PresValidityFrom,
                                                        ReqDetValidityUntil = resourcePeople.PresValidityUntil,
                                                        RequestDetAdditional = resourcePeople.PresAdditional,
                                                        RequestDetAdditionalStrValue = resourcePeople.PresAdditionalStrValue,
                                                        RequestDetAdditionalIntValue = resourcePeople.PresAdditionalIntValue,
                                                    }).ToList();
                if (requestInfo.AccessRequestDetails.Count > 0)
                {
                    success = (InsertAccessRequest(requestInfo, userID) != null);
                }
                else
                {
                    success = true;
                }

                // desactivamos a la persona
                peopleRepository.UpdateActive(peopleID, false, this.sessionData.sessionUser);
                if (!success) break;
            }
            return success;
        }


        public bool DeactivateCompanyAccessByPeople(int peopleID, string requestNote, int companyID, bool skipIssues = false, Expression<Func<ResourcePeople, bool>> filter = null)
        {
            bool success = false;
            string userID = this.sessionData.sessionUser;

            // deactivate request ------------------------------------------------------------------
            AccessRequestDTO request = new AccessRequestDTO()
            {
                RequestBy = userID,
                RequestType = AccessRequestDTO.TYPE_BAJA,
                RequestPriority = AccessRequestDTO.PRIORITY_NORMAL,
                RequestNote = requestNote
            };
            request.RequestID = 0;
            request.RequestNumber = 0;
            request.RequestTo = peopleID;

            var queryDetails = from rp in db.ResourcePeople
                               where
                                   rp.PeopleID == peopleID
                                   && rp.PresCompany == companyID
                                   && rp.PresActive > 0
                                   && (
                                       !skipIssues ||
                                       (skipIssues &&
                                          !(rp.AccessResources == null || rp.AccessResources.Workflows == null || rp.AccessResources.Workflows.WorkflowItems.Count == 0
                                       //|| (
                                       //    // Si no tiene un item de execución
                                       //    (rp.AccessResources.Workflows.WorkflowItems.Count(x => x.WfItemType == WorkflowItemsDTO.TYPE_ACTION &&
                                       //                                                           x.WfItemActionProperty == WorkflowItemsDTO.ACTION_TYPE_EXECUTE_IN_SERVER) == 0) &&
                                       //    // Si no tiene notificación al ejecutor
                                       //    (rp.AccessResources.Workflows.WorkflowItems.Count(x => x.CommonValues.CommonValueName == "NOTIFICACION" &&
                                       //                                                           x.WfItemDestType == (int)WfItemDestType.Ejecutor) == 0)
                                       //)
                                       ))
                                   )
                               select rp;
            if(filter != null)
            {
                queryDetails = queryDetails.Where(filter);
            }
            request.AccessRequestDetails = queryDetails.Select(rp => new AccessRequestDetailsDTO()
            {
                ResourceID = rp.ResourceID,
                RequestDetType = AccessRequestDTO.TYPE_BAJA,
                RequestDetIntValue = rp.PresIntValue,
                RequestDetStrValue = rp.PresStrValue,
                ReqDetTemporal = rp.PresTemporal,
                ReqDetValidityFrom = rp.PresValidityFrom,
                ReqDetValidityUntil = rp.PresValidityUntil,
                RequestDetAdditional = rp.PresAdditional,
                RequestDetAdditionalStrValue = rp.PresAdditionalStrValue,
                RequestDetAdditionalIntValue = rp.PresAdditionalIntValue,
            }).ToList();
            
            if (request.AccessRequestDetails.Count > 0)
            {
                success = (InsertAccessRequest(request, userID, companyID) != null);
            }
            else
            {
                success = true;
            }
            //if (skipIssues)
            //{
            //    var allResourcePeople = from rp in db.ResourcePeople
            //                            where
            //                                 rp.PeopleID == peopleID
            //                                 && rp.PresCompany == companyID
            //                                 && rp.PresActive > 0
            //                            select rp;
            //    success = success && (request.AccessRequestDetails.Count == allResourcePeople.Count());
            //}

            return success;
        }
        public int DeactivateAllAccessByPeople(int peopleID, string requestNote, bool skipIssues = false, Expression<Func<ResourcePeople, bool>> filter = null)
        {
            SisConAxsContext db = new SisConAxsContext();
            List<bool> results = new List<bool> ();
            var activeRP = db.ResourcePeople.Where(rp => rp.PeopleID == peopleID && rp.PresActive == 1).ToList();
            var peopleCompanies = activeRP.Select(l => l.PresCompany).Distinct().ToList();
            foreach (var companyID in peopleCompanies)
            {
                var resultDeactivate = this.DeactivateCompanyAccessByPeople(
                    peopleID,
                    requestNote,
                    companyID,
                    skipIssues,
                    filter
                );
                results.Add(resultDeactivate);
            }

            if (results.Count == 0) return 0;
            else if (results.Exists(r => !r)) return -1;
            else return 1;
        }
        public int DeactivateAllAccessByPeople(int peopleID, string requestNote, Expression<Func<ResourcePeople, bool>> filter)
        {
            return DeactivateAllAccessByPeople(peopleID, requestNote, false, filter);
        }
            


        public void CancelWfExecutionByRequestDetail(int RequestDetailID)
        {
            var WfExecItems = db.WorkflowExecution.Where(x => x.WfExecObjectID == RequestDetailID);
            foreach (var item in WfExecItems)
            {
                item.WfExecStatus = 4;/// 4: Cancelación de solicitud
                SaveEntity(item.WfExecID == 0, item);
            }
        }

        public object GetRequestApprovePending(string user)
        {
            var query = (
                from peop in db.People
                join executionParams in db.WFExecutionParameters on peop.PeopleID equals executionParams.WfExecParamIntValue
                join execution in db.WorkflowExecution on executionParams.WfExecID equals execution.WfExecID
                join request in db.AccessRequests on execution.WfExecParentObjectID equals request.RequestID
                join requestTo in db.People on request.RequestTo equals requestTo.PeopleID
                join comp in db.Companies on request.RequestCompany equals comp.CompanyID //new line - angel
                where
                    execution.WfExecStatus == 2
                    && executionParams.WfExecParamName == "approver"
                    && peop.UserID == user
                select new AccessRequestDTO()
                {
                    RequestID = request.RequestID,
                    RequestNumber = request.RequestNumber,
                    RequestDate = request.RequestDate,
                    RequestToName = requestTo.PeopleFirstName + " " + requestTo.PeopleFirstName2 + ", " + requestTo.PeopleLastName + " " + requestTo.PeopleLastName2,
                    RequestToCompanyName = comp.CompanyName // new line - angel
                }
            ).Distinct();
            return new
            {
                count = query.Count(),
                lastRequests = query.Take(3)
            };
        }

        public static AccessRequestDetails GetDetailByID(int requestDetailID)
        {
            SisConAxsContext db = new SisConAxsContext();
            return db.AccessRequestDetails.FirstOrDefault(x => x.RequestDetID == requestDetailID);
        }

        public static void SaveRequestDetailStatus(WorkflowExecution wfExec, WfExecStatus status)
        {
            SisConAxsContext db = new SisConAxsContext();
            //SaveRequestDetailStatusTransaction(db, wfExec, status);
            //db.SaveChanges();

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    SaveRequestDetailStatusTransaction(db, wfExec, status);
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (ModelException ex)  // Si es un error provocado por el usuario (al grabar resource people) se graba y se manda la excepción para generar el LOG de ERROR
                {
                    db.SaveChanges();
                    transaction.Commit();
                    throw ex;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
        public static void SaveRequestDetailStatusTransaction(SisConAxsContext db, WorkflowExecution wfExec, WfExecStatus status)
        {
            // graba estado de detalle y el maestro de la solicitud
            AccessRequests Request = db.AccessRequests.FirstOrDefault(t => t.RequestID == wfExec.WfExecParentObjectID && wfExec.WfExecObjectName == "ACCESS_REQUEST");
            AccessRequestDetails RequestDetail = Request.AccessRequestDetails.FirstOrDefault(t => t.RequestDetID == wfExec.WfExecObjectID);

            // detalle
            if (RequestDetail != null)
            {
                switch (status)
                {
                    case WfExecStatus.FinishedApproved:
                        RequestDetail.RequestDetStatus = AccessRequestDTO.STATUS_APPROVE;
                        break;
                    case WfExecStatus.FinishedRejected:
                        RequestDetail.RequestDetStatus = AccessRequestDTO.STATUS_REJECT;
                        break;
                    case WfExecStatus.FinishedTimeout:
                        RequestDetail.RequestDetStatus = AccessRequestDTO.STATUS_TIMEOUT;
                        break;
                    default:
                        RequestDetail.RequestDetStatus = AccessRequestDTO.STATUS_UNDEFINIED;
                        break;
                }
            }

            // maestro
            if (Request.AccessRequestDetails.All(t => t.RequestDetStatus == AccessRequestDTO.STATUS_PENDING))
            {
                Request.RequestStatus = AccessRequestDTO.STATUS_PENDING;
            }
            else
            {
                if (Request.AccessRequestDetails.All(t => t.RequestDetStatus == AccessRequestDTO.STATUS_APPROVE))
                {
                    Request.RequestStatus = AccessRequestDTO.STATUS_APPROVE;
                    Request.RequestCompletedDate = DateTime.Now;
                }
                else if (Request.AccessRequestDetails.All(t => t.RequestDetStatus == AccessRequestDTO.STATUS_REJECT))
                {
                    Request.RequestStatus = AccessRequestDTO.STATUS_REJECT;
                    Request.RequestCompletedDate = DateTime.Now;
                }
                else if (Request.AccessRequestDetails.All(t => t.RequestDetStatus == AccessRequestDTO.STATUS_TIMEOUT))
                {
                    Request.RequestStatus = AccessRequestDTO.STATUS_TIMEOUT;
                    Request.RequestCompletedDate = DateTime.Now;
                }
                else if (Request.AccessRequestDetails.Count(t => t.RequestDetStatus == AccessRequestDTO.STATUS_PENDING) > 0)
                {
                    Request.RequestStatus = AccessRequestDTO.STATUS_PARCIAL;
                }
                else
                {
                    Request.RequestStatus = AccessRequestDTO.STATUS_ATTENDED;
                    Request.RequestCompletedDate = DateTime.Now;
                }
            }

            db.AccessRequests.Attach(Request);
            db.Entry(Request).State = EntityState.Modified;
            //db.SaveChanges();

            // guarda los permisos del usuario
            if (status == WfExecStatus.FinishedApproved)
            {
                try
                {
                    new ResourcePeopleRepository().SaveResourcePeople(db, RequestDetail);
                }
                catch (ModelException ex)
                {
                    new WorkflowExecutionRepository().SaveWFExecutionTransaction(
                        db,
                        wfExec,
                        null,
                        status,
                        $"<span style='color:#555'>[{GetCurrentDateStr()}]&nbsp;&nbsp;</span> {ex.Message}"
                    );
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public FtpWebResponse CreateFolder(HttpPostedFile filename, string requestNumber, bool createFolder)
        {
            var url = $"{FTP_URL}/{requestNumber}";
            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(url);
            request.Credentials = new NetworkCredential(FTP_USER, FTP_PASSWORD);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;
            Console.WriteLine("Getting the response");

            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            //AQUI CREA LA CARPETA EN EL SERVIDOR FTP
            var resp = (FtpWebResponse)request.GetResponse();
            return resp;
        }

        public bool ExisteDirectorio(string requestNumber)
        {

            bool bExiste = true;

            try
            {
                var url = $"{FTP_URL}/{requestNumber}";
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Credentials = new NetworkCredential(FTP_USER, FTP_PASSWORD);
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                FtpWebResponse respuesta = (FtpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    FtpWebResponse respuesta = (FtpWebResponse)ex.Response;
                    if (respuesta.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    {
                        bExiste = false;
                    }
                }
            }
            return bExiste;
        }
        public string SaveFileToFTP(HttpPostedFile filename, string requestNumber)
        {
            try
            {
                var url = $"{FTP_URL}/{requestNumber}/{filename.FileName}";
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Ftp.UploadFile;

                request.Credentials = new NetworkCredential(FTP_USER, FTP_PASSWORD);
                
                // File generation in memory 
                byte[] fileBytes = null;
                using (var binaryReader = new BinaryReader(filename.InputStream))
                {
                    fileBytes = binaryReader.ReadBytes(filename.ContentLength);
                }

                request.ContentLength = fileBytes.Length;
                request.Timeout = -1;

                //AQUI SUBE EL ARCHIVO AL SERVIDOR FTP
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileBytes, 0, fileBytes.Length);
                }

                return url;
            }
            catch (Exception ex)
            {
                throw new ModelException("Error al grabar archivo adjunto.", ex);
            }
        }

        public string SaveAttachedToSharepoint(HttpPostedFile filename, string requestNumber, bool createFolder)
        {
            // SharePoint Site URL 
            var urlDomain = ConfigurationManager.AppSettings["Url"];
            var siteUrl = ConfigurationManager.AppSettings["SiteUrl"];
            var userLogin = ConfigurationManager.AppSettings["UserLogin"];
            var userPassword = ConfigurationManager.AppSettings["UserPassword"];
            var docLibTitle = ConfigurationManager.AppSettings["DocLibTitle"];
            var folder = ConfigurationManager.AppSettings["Folder"];

            var url = string.Empty;

            var pwd = new SecureString();
            foreach (var c in userPassword) pwd.AppendChar(c);

            // Credentials for SharePoint Online 
            var SPOCredentials = new SH.SharePointOnlineCredentials(userLogin, pwd);

            // Credentials for SharePoint on-premise 
            var SPCredentials = new NetworkCredential(userLogin, pwd);

            using (var ctx = new SH.ClientContext(siteUrl))
            {
                try
                {
                    // try to use SharePoint Online Credentials 
                    ctx.Credentials = SPOCredentials;
                    ctx.ExecuteQuery();  // aqui se cae por las credenciales
                    Console.WriteLine("SharePoint Online");
                }
                catch (SH.ClientRequestException)
                {
                    // switch to NetworkCredential 
                    ctx.Credentials = SPCredentials;
                    ctx.ExecuteQuery();
                    Console.WriteLine("SharePoint On-premise");
                }
                catch (NotSupportedException)
                {
                    // switch to NetworkCredential 
                    ctx.Credentials = SPCredentials;
                    ctx.ExecuteQuery();
                    Console.WriteLine("SharePoint On-premise");
                }

                var library = ctx.Web.Lists.GetByTitle(docLibTitle);
                ctx.Load(library, x => x.RootFolder.ServerRelativeUrl);
                ctx.ExecuteQuery();

                //validamos si la carpeta con numero de solicitud existe
                //if (!FolderExists(ctx, $"{library.RootFolder.ServerRelativeUrl}/{requestNumber}"))
                //{
                //    var folders = library.RootFolder.Folders;
                //    ctx.Load(folders);
                //    ctx.ExecuteQuery();
                //    var newFolder = folders.Add(requestNumber);
                //    ctx.ExecuteQuery();
                //}
                if (createFolder)
                {
                    var folders = library.RootFolder.Folders;
                    ctx.Load(folders);
                    ctx.ExecuteQuery();
                    var newFolder = folders.Add(requestNumber);
                    ctx.ExecuteQuery();
                }

                folder = requestNumber;


                // File generation in memory 
                byte[] fileBytes = null;
                using (var binaryReader = new BinaryReader(filename.InputStream))
                {
                    fileBytes = binaryReader.ReadBytes(filename.ContentLength);
                }

                Console.WriteLine("Uploading file...");

                // Server relative url of the document
                url = string.Format("{0}/{1}/{2}_{3}{4}", library.RootFolder.ServerRelativeUrl, folder, Path.GetFileNameWithoutExtension(filename.FileName), DateTime.Now.ToString("yyyyMMdd_HHmmss",
                                            CultureInfo.InvariantCulture), Path.GetExtension(filename.FileName));

                Microsoft.SharePoint.Client.File.SaveBinaryDirect(
                    //Client Context 
                    ctx,
                    // Server relative url of the document 
                    url,
                    // Content of the file 
                    new MemoryStream(fileBytes),
                    // Overwrite file if it's already exist 
                    false);
            }

            return string.Format("{0}{1}", urlDomain, url);
        }

        public byte[] DownloadFile(string filename, string requestNumber)
        {
            try
            {
                var url = $"{FTP_URL}/{requestNumber}/{filename}";
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(FTP_USER, FTP_PASSWORD);
                
                FtpWebResponse response = (FtpWebResponse)request.GetResponse(); //AQUI DESCARGA EL SERVIDOR FTP
                using (MemoryStream stream = new MemoryStream())
                {
                    response.GetResponseStream().CopyTo(stream);
                    HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + filename);
                    HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    HttpContext.Current.Response.BinaryWrite(stream.ToArray());
                    HttpContext.Current.Response.End();

                    byte[] data = stream.ToArray();
                    //byte[] data = new byte[111111111];
                    return data;
                }
            }
            catch (HttpException ex)
            {
                throw new ModelException("Error al descargar el archivo adjunto.", ex);
            }
        }

        private static bool FolderExists(SH.ClientContext context, string url)
        {
            var folder = context.Web.GetFolderByServerRelativeUrl(url);
            context.Load(folder, f => f.Exists);
            try
            {
                context.ExecuteQuery();

                if (folder.Exists)
                {
                    return true;
                }
                return false;
            }
#pragma warning disable CS0168 // La variable 'e' se ha declarado pero nunca se usa
            catch (SH.ServerUnauthorizedAccessException e)
#pragma warning restore CS0168 // La variable 'e' se ha declarado pero nunca se usa
            {
                //Trace.WriteLine($"You are not allowed to access this folder");
                throw;
            }
#pragma warning disable CS0168 // La variable 'ex' se ha declarado pero nunca se usa
            catch (Exception ex)
#pragma warning restore CS0168 // La variable 'ex' se ha declarado pero nunca se usa
            {
                //Trace.WriteLine($"Could not find folder {url}");
                return false;
            }
        }
    }
}
