using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Repository
{
    public class WorkflowExecutionRepository : AxsBaseRepository
    {
        public WorkflowExecutionRepository ()
        {
            dbSet = db.WorkflowExecution;
        }


        private string GetStatusName(WfExecStatus wfStatus)
        {
            switch (wfStatus)
            {
                case WfExecStatus.Pending:
                    return "Pendiente";
                case WfExecStatus.ToProcess:
                    return "Para procesar";
                case WfExecStatus.WaitingResponse:
                    return "Esperando Respuesta";
                case WfExecStatus.Responded:
                    return "Respuesta recibida";
                case WfExecStatus.FinishedApproved:
                    return "Aprobado";
                case WfExecStatus.FinishedRejected:
                    return "Rechazado";
                case WfExecStatus.FinishedTimeout:
                    return "Tiempo de espera vencido";
                case WfExecStatus.FinishedUndefinied:
                    return "Indefinido";
                default:
                    return "Desconocido";
            }
        }

        /// <summary>
        /// Actualiza un Workflow Execution con nueva información. El registro ya debe existir
        /// previo a la actualización.
        /// </summary>
        /// <param name="db">Contexto de base de datos para grabar el registro.</param>
        /// <param name="WfExec">Workflow Execution a actualizar.</param>
        /// <param name="newItemID">Nuevo Workflow Item ID.</param>
        /// <param name="newStatus">Nuevo Status de ejecución.</param>
        /// <param name="saveImmediately">Indica si el registro se debe grabar inmediatamente (true) o si es responsabilidad del
        ///                               procedimiento que hizo la llamada (false). Por defecto es true</param>
        ///                               
        public void SaveWFExecution(WorkflowExecution WfExec,
                                    Nullable<int> newItemID,
                                    WfExecStatus newStatus,
                                    string message)
        {
            SisConAxsContext db = new SisConAxsContext();
            //SaveWFExecutionTransaction(db, WfExecID, newItemID, newStatus, message);
            //db.SaveChanges();

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    SaveWFExecutionTransaction(db, WfExec, newItemID, newStatus, message);
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public void SaveWFExecutionTransaction(
            SisConAxsContext db,
            WorkflowExecution WfExec,
            Nullable<int> newItemID,
            WfExecStatus newStatus,
            string message
        )
        {
            var model = db.WorkflowExecution.FirstOrDefault(m => m.WfExecID == WfExec.WfExecID);
            var currentItemID = model.WfExecCurrentStep;
            model.WfExecCurrentStep = newItemID;
            model.WfExecStatus = (int)newStatus;
            model.WfExecHistoryMessage = message;
            model.WfExecStartDate = DateTime.Now;
            if (newStatus == WfExecStatus.WaitingResponse) // resetea la respuesta si es consulta
                model.WfResponse = -1;
            else
            {
                if (WfExec.WfResponse != -1)
                    model.WfResponse = WfExec.WfResponse;
            }

            db.WorkflowExecution.Attach(model);
            db.Entry(model).State = EntityState.Modified;
            //db.SaveChanges();

            SaveWFExecHistory(db, model, currentItemID);
        }

        public void SaveWFExecutionEnd(WorkflowExecution wfExec, WfExecStatus status = WfExecStatus.FinishedUndefinied, string message = "")
        {
            string statusColor = "inherited";

            // establece estado de termino del workflow
            if (wfExec.WfResponse == (int)WfExecResponse.Rejected)
            {
                status = WfExecStatus.FinishedRejected;
                statusColor = "#E77272";
            }
            else if (wfExec.WfResponse == (int)WfExecResponse.Approved)
            {
                status = WfExecStatus.FinishedApproved;
                statusColor = "#6CD865";
            }
            if (status == WfExecStatus.FinishedTimeout)
            {
                statusColor = "#E77272";
            }

            // graba ejecucion
            SisConAxsContext db = new SisConAxsContext();
            //this.SaveWFExecutionTransaction(
            //    db,
            //    wfExec.WfExecID,
            //    null,
            //    status,
            //    String.Format("<span style='color:#555'>[{0}]&nbsp;&nbsp;</span> Se terminó el proceso de aprobación con resultado <strong><span style='background-color:{2}'>[{1}]</span></strong>. ",
            //                    GetCurrentDateStr(),
            //                    GetStatusName(status).ToUpper(),
            //                    statusColor) + message);

            //AccessRequestRepository.SaveRequestDetailStatusTransaction(db, wfExec, status);  // graba estado de detalle de solicitud y actualiza el maestro
            //db.SaveChanges();
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    this.SaveWFExecutionTransaction(
                        db,
                        wfExec,
                        null,
                        status,
                        $"<span style='color:#555'>[{GetCurrentDateStr()}]&nbsp;&nbsp;</span> Se terminó el proceso de aprobación con resultado <strong><span style='background-color:{statusColor}'>[{GetStatusName(status).ToUpper()}]</span></strong>. {message}"
                    );
                    AccessRequestRepository.SaveRequestDetailStatusTransaction(db, wfExec, status);  // graba estado de detalle de solicitud y actualiza el maestro
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

        // Guarda el historico del Workflow Execution
        public void SaveWFExecHistory(SisConAxsContext db, WorkflowExecution wfExec, int? currentItemID)
        {
            WorkflowExecutionHistory wfExecHistoryDet = db.WorkflowExecutionHistory.Create();
            wfExecHistoryDet.WfExecID = wfExec.WfExecID;
            wfExecHistoryDet.WfExecWfID = wfExec.WfExecWfID;
            wfExecHistoryDet.WfExecCurrentStep = currentItemID; //wfExec.WfExecCurrentStep;
            wfExecHistoryDet.WfResponse = wfExec.WfResponse;
            wfExecHistoryDet.WfExecStartDate = wfExec.WfExecStartDate;
            wfExecHistoryDet.WfExecStartedBy = wfExec.WfExecStartedBy;
            wfExecHistoryDet.WfExecParentObject = wfExec.WfExecParentObjectID;
            wfExecHistoryDet.WfExecObjectID = wfExec.WfExecObjectID;
            wfExecHistoryDet.WfExecObjectName = wfExec.WfExecObjectName;
            wfExecHistoryDet.WfExecObjectStatus = wfExec.WfExecStatus;
            wfExecHistoryDet.WfExecHistoryMessage = wfExec.WfExecHistoryMessage;
            wfExecHistoryDet.WfExecCompany = wfExec.WfExecCompany;
            //INICIO Adicionado el 20191107 <-- DIANA CAMUS
            var approver = db.WFExecutionParameters.FirstOrDefault(p => p.WfExecID == wfExec.WfExecID
                                                                         && p.WfExecParamName == "approver_by");
            if(approver != null)
            {
                var people = (from p in db.People
                              join dep in db.CommonValues on p.PeopleDepartment equals dep.CommonValueID into department
                              from dep in department.DefaultIfEmpty()
                              join pos in db.CommonValues on p.PeoplePosition equals pos.CommonValueID into position
                              from pos in position.DefaultIfEmpty()
                              where
                                  p.PeopleID == approver.WfExecParamIntValue.Value
                              select new PeopleDTO()
                              {
                                  PeopleID = p.PeopleID,
                                  PeopleInternalID = p.PeopleInternalID,
                                  PeopleDepartment = p.PeopleDepartment,
                                  PeopleDepartmentName = dep.CommonValueDisplay,
                                  PeoplePosition = p.PeoplePosition,
                                  PeoplePositionName = pos.CommonValueDisplay,
                                  UserID = p.UserID,
                              }
                        ).FirstOrDefault();

                wfExecHistoryDet.WfExecApproverName = approver.WfExecParamStrValue;
                wfExecHistoryDet.wfExecApproverID = approver.WfExecParamIntValue;
                wfExecHistoryDet.WfExecApproverArea = people.PeopleDepartmentName;
                wfExecHistoryDet.WfExecApproverPosition = people.PeoplePositionName;
            }

            //if (wfExec.WfResponse == (int)WfExecResponse.Approved)
            //{
            //    wfExecHistoryDet.WfExecApproverID = wfExec.WFExecutionParameters.FirstOrDefault(p => p.WfExecParamName == "approver").WfExecParamIntValue;
            //}
            var itemCurrent = db.WorkflowItems.FirstOrDefault(x => x.WfItemId == currentItemID);
            if (itemCurrent != null && itemCurrent.WfItemType == WorkflowItemsDTO.TYPE_NOTIFICATION && itemCurrent.WfItemDestType == (int)WfItemDestType.Ejecutor)
            {
                wfExecHistoryDet.WfExecExecutorMail = itemCurrent.WfItemDestMail;
            }
            //FIN Adicionado el 20191107 <-- DIANA CAMUS
            db.WorkflowExecutionHistory.Add(wfExecHistoryDet);
        }


        /// <summary>
        ///  Graba un Parametro de Ejecucion de Workflow según los valores pasados.
        /// Si el parametro no existia para esta Ejecución entonces se crea uno nuevo,
        /// si el parametro ya existia, se sobreescriben los valores con los nuevos.
        /// </summary>
        /// <param name="wfExecID">ID del Workflow Execution para el que se graba el parametro.</param>
        /// <param name="paramName">Nombre del parametro a grabar</param>
        /// <param name="paramIntValue">Valor entero a grabar para este parametro.</param>
        /// <param name="paramStrValue">Valor de cadena de texto a grabar para este parametro</param>
        /// <param name="paramDateValue">Valor fecha a grabar para este parametro</param>
        public void SaveWFExecParam(int wfExecID,
                                    string paramName,
                                    Nullable<int> paramIntValue,
                                    string paramStrValue,
                                    Nullable<DateTime> paramDateValue)
        {
            SisConAxsContext db = new SisConAxsContext();
            WFExecutionParameters wfExecParams = db.WFExecutionParameters.FirstOrDefault(p => p.WfExecID == wfExecID && p.WfExecParamName == paramName);
            if (wfExecParams == null)
            {
                wfExecParams = db.WFExecutionParameters.Create();
                wfExecParams.WfExecID = wfExecID;
                wfExecParams.WfExecParamName = paramName;
                wfExecParams.WfExecParamIntValue = paramIntValue;
                wfExecParams.WfExecParamStrValue = paramStrValue;
                wfExecParams.WfExecParamDateValue = paramDateValue;
                wfExecParams.CreateUser = "WORKFLOW_ENGINE";
                wfExecParams.CreateDate = DateTime.Now;
                wfExecParams.EditUser = "WORKFLOW_ENGINE";
                wfExecParams.EditDate = DateTime.Now;
                db.WFExecutionParameters.Add(wfExecParams);
            }
            else
            {
                wfExecParams.WfExecParamIntValue = paramIntValue;
                wfExecParams.WfExecParamStrValue = paramStrValue;
                wfExecParams.WfExecParamDateValue = paramDateValue;
                wfExecParams.EditDate = DateTime.Now;
                db.WFExecutionParameters.Attach(wfExecParams);
                db.Entry(wfExecParams).State = EntityState.Modified;
            }
            db.SaveChanges();
        }

        public void DeleteWFExecParam(int wfExecID,
                                    string paramName)
        {
            WFExecutionParameters wfExecParams = db.WFExecutionParameters.FirstOrDefault(p => p.WfExecID == wfExecID && p.WfExecParamName == paramName);
            AxsBaseRepository.DeleteEntity(wfExecParams, db, db.WFExecutionParameters);
        }

        // Parameters INIT -------------------------------------------------------------------------------------------- //

        // Graba los Parametros iniciales de un Workflow Execution, estos valores se obtienen
        // de la Solicitud y del Detalle.
        public bool SaveWFExecInitParams(SisConAxsContext db, WorkflowExecution wfExec) //, out string errMessage)
        {
            AccessRequestDetails detail = db.AccessRequestDetails.FirstOrDefault(t => t.RequestID == wfExec.WfExecParentObjectID
                                                                                      && t.RequestDetID == wfExec.WfExecObjectID
                                                                                      && wfExec.WfExecObjectName == "ACCESS_REQUEST");

            // Primero busca si tiene la execución en el servicio
            WorkflowItems itemExecution = wfExec.Workflow.WorkflowItems.FirstOrDefault(x => x.WfItemType == WorkflowItemsDTO.TYPE_ACTION
                                                                                            && x.WfItemActionProperty == WorkflowItemsDTO.ACTION_TYPE_EXECUTE_IN_SERVER);
            // Si no encuentra la execución en el servicio, busca la notificación al ejecutor
            if (itemExecution == null)
            {
                itemExecution = wfExec.Workflow.WorkflowItems.FirstOrDefault(x => x.CommonValues.CommonValueName == "NOTIFICACION"
                                                                                  && x.WfItemDestType == (int)WfItemDestType.Ejecutor);
            }

            int generalRequestType = db.AccessRequests.Where(ar => ar.RequestID == detail.RequestID).Select(ar => ar.RequestType).FirstOrDefault();

            // Si el TIPO DE SOLUCITUD GENERAL es ALTA o MODIFICACION, se verifica la jerarquía y si tiene items de consulta ==========================================================================================================
            if (generalRequestType == AccessRequestDTO.TYPE_ALTA || generalRequestType == AccessRequestDTO.TYPE_MODIFICACION) 
            {
                //errMessage = "";
                string defaulApprovetMsj = $"<span style='color:#555'>[{GetCurrentDateStr()}]&nbsp;&nbsp;</span> <strong><span style='background-color:#6CD865'>APROBADO</span></strong> automáticamente.";

                // si no tiene jerarquía se aprueba inmediatamente ----------------------------------------------------------- //
                if (wfExec.Workflow.WfApproveHierarchyID == null || wfExec.Workflow.WfApproveHierarchyID == 0)
                {
                    wfExec.WfResponse = (int)WfExecResponse.Approved;
                    wfExec.WfExecHistoryMessage = defaulApprovetMsj;

                    if (itemExecution != null)
                    {
                        SaveWFExecution(wfExec, itemExecution.WfItemId, WfExecStatus.ToProcess, defaulApprovetMsj);
                    } else
                    {
                        SaveWFExecutionEnd(wfExec, WfExecStatus.FinishedApproved);
                    }
                    return false;
                }

                // si no tiene items de consulta se aprueba inmediatamente --------------------------------------------------- //
                if (!wfExec.Workflow.WorkflowItems.Any(x => x.WfItemType == WorkflowItemsDTO.TYPE_ASK))
                {
                    wfExec.WfResponse = (int)WfExecResponse.Approved;
                    wfExec.WfExecHistoryMessage = defaulApprovetMsj;

                    if (itemExecution != null)
                    {
                        SaveWFExecution(wfExec, itemExecution.WfItemId, WfExecStatus.ToProcess, defaulApprovetMsj);
                    } else
                    {
                        SaveWFExecutionEnd(wfExec, WfExecStatus.FinishedApproved);
                    }
                    return false;
                }
            }
            // Si el TIPO DE SOLICITUD GENERAL es BAJA se APRUEBA y envía directamente al ejecutor ==================================================================================================================
            else if (generalRequestType == AccessRequestDTO.TYPE_BAJA) 
            {
                if (itemExecution != null)
                {
                    wfExec.WfResponse = (int)WfExecResponse.Approved;  // Aprobado
                    SaveWFExecution(
                        wfExec,
                        itemExecution.WfItemId,
                        WfExecStatus.ToProcess,
                        $"<span style='color:#555'>[{GetCurrentDateStr()}]&nbsp;&nbsp;</span> Se inicia el proceso de <strong><span style='color:#C80000'>BAJA</span></strong> del permiso, se establece como <strong><span style='background-color:#6CD865'>APROBADO</span></strong>."
                    );
                }
                else
                {
                    wfExec.WfResponse = (int)WfExecResponse.Rejected;  // Rechazado
                    SaveWFExecutionEnd(wfExec, WfExecStatus.FinishedRejected, "No se encontró un item en el workflow que sea el envío al ejecutor o la ejecución en un servicio.");
                }
                return false;
            }


            // Se obtiene el ID y Nombre de la jerarquía de aprobación, se hace una búsqueda en el caso sea la jerarquía es del área de la persona
            int? workflowHierarchyID = wfExec.Workflow.WfApproveHierarchyID;
            string workflowHierarchyname = wfExec.Workflow.WorkflowApproveHierarchy.WfApproveHierarchyName;
            if (workflowHierarchyID == -1)  // si la jerarquía es del área de la persona
            {
                AccessRequests request = db.AccessRequests.FirstOrDefault(t => t.RequestID == wfExec.WfExecParentObjectID);
                People people = db.People.FirstOrDefault(t => t.PeopleID == request.RequestTo);
                if (people.PeopleDepartment == null || people.PeopleDepartment == 0)
                {
                    throw new ModelException("No se encontró a la jerarquía para el área porque la persona no pertenece a alguna área.");
                    //errMessage = "No se encontró a la jerarquía para el área porque la persona no pertenece a alguna área.";
                    //LogManager.Error(String.Format("[{0}] " + errMessage, GetCurrentDateStr()));
                    //return false;
                }

                WorkflowApproveHierarchy wfApproveHierarchy = db.WorkflowApproveHierarchy.FirstOrDefault(t => t.WfApproveHierarchyDepartment == people.Department.CommonValueID);
                if (wfApproveHierarchy == null)
                {
                    throw new ModelException($"No se encontró a la jerarquía para el área [{people.Department.CommonValueDisplay}]");
                    //errMessage = String.Format("No se encontró a la jerarquía para el área [{0}]", people.Department.CommonValueDisplay);
                    //LogManager.Error(String.Format("[{0}] " + errMessage, GetCurrentDateStr()));
                    //return false;
                }

                workflowHierarchyID = wfApproveHierarchy.WfApproveHierarchyID;
                workflowHierarchyname = wfApproveHierarchy.WfApproveHierarchyName;
            }

            // Obtenemos el Parametro [approver] del primer aprobador de la Jeraquia de Aprobacion del Workflow de esta ejecución.
            WorkflowHierarchyMembers wfHMember = db.WorkflowHierarchyMembers.FirstOrDefault(m => m.WfApproveHierarchyID == workflowHierarchyID
                                                                                                 && m.WfHierarchyMemberOrder == 1);
            if (wfHMember == null)
            {
                throw new ModelException($"No se encontró a un miembro para la jerarquía [{workflowHierarchyname}].");
                //errMessage = String.Format("No se encontró a un miembro para la jerarquía [{0}]. Solicitud N: {1}", workflowHierarchyname, wfExec.WfExecParentObjectID);
                //LogManager.Error(String.Format("[{0}] " + errMessage, GetCurrentDateStr()));
                //return false;
            }

            People wfApprover = db.People.FirstOrDefault(p => p.PeopleDepartment == wfHMember.WfHierarchyMemberDepartment
                                                              && p.PeoplePosition == wfHMember.WfHierarchyMemberPosition
                                                              && p.PeopleStatus == 1); //VALIDA QUE EL APROBADOR ESTE ACTIVO 2017-04-12
            if (wfApprover == null)
            {
                throw new ModelException($"No se encontró a una persona que sea miembro de la jerarquía <strong>[{wfExec.Workflow.WorkflowApproveHierarchy.WfApproveHierarchyName}]</strong>");
                //errMessage = String.Format("No se encontró a una persona que sea miembro de la jerarquía <strong>[{0}]</strong>",
                //                           wfExec.Workflow.WorkflowApproveHierarchy.WfApproveHierarchyName);
                //LogManager.Error(String.Format("[{0}] " + errMessage, GetCurrentDateStr()));
                //return false;
            }
            else
            {
                // si la persona no tiene un registro en la tabla usuarios(No va a poder aprobar la solicitud)
                AccessUsers user = db.AccessUsers.FirstOrDefault(p => p.UserInternalID == wfApprover.UserID);
                if (user == null)
                {
                    throw new ModelException($"No se ejecutó la consulta porque la persona '{wfApprover.GetFullName()}' no está registrado como usuario del sistema.");
                    //errMessage = String.Format("No se ejecutó la consulta porque la persona '{0}' no está registrado como usuario del sistema.",
                    //                           wfApprover.GetFullName());
                    //LogManager.Error(String.Format("[{0}] " + errMessage, GetCurrentDateStr()));
                    //return false;
                }

                string peopleFullName = wfApprover.GetFullName();
                SaveWFExecParam(wfExec.WfExecID, "approver", wfApprover.PeopleID, peopleFullName, DateTime.Now);
                SaveWFExecParam(wfExec.WfExecID, "approver_order", wfHMember.WfHierarchyMemberOrder, "", DateTime.Now);

                String HierarchyMemberDescription = "";
                if (!String.IsNullOrWhiteSpace(wfHMember.WfHierarchyMemberDescription))
                {
                    HierarchyMemberDescription = " - " + wfHMember.WfHierarchyMemberDescription;
                }

                wfExec.WfExecHistoryMessage = $"<span style='color:#555'>[{GetCurrentDateStr()}]&nbsp;&nbsp;</span> Se cambió el aprobador a <strong>[{peopleFullName}]</strong> {HierarchyMemberDescription}.";
            }

            return true;
        }

        // Parameters -------------------------------------------------------------------------------------------- //
        public string GetWfExecStringParam(int wfExecID, string paramName)
        {
            WFExecutionParameters wfExecParams = GetWfExecParam(wfExecID, paramName);
            if (wfExecParams != null)
                return wfExecParams.WfExecParamStrValue;
            else
                return null;
        }

        public Nullable<int> GetWfExecIntParam(int wfExecID, string paramName)
        {
            WFExecutionParameters wfExecParams = GetWfExecParam(wfExecID, paramName);
            if (wfExecParams != null)
                return wfExecParams.WfExecParamIntValue;
            else
                return null;
        }

        public Nullable<DateTime> GetWfExecDateParam(int wfExecID, string paramName)
        {
            WFExecutionParameters wfExecParams = GetWfExecParam(wfExecID, paramName);
            if (wfExecParams != null)
                return wfExecParams.WfExecParamDateValue;
            else
                return null;
        }

        public WFExecutionParameters GetWfExecParam(int wfExecID, string paramName)
        {
            SisConAxsContext db = new SisConAxsContext();
            return db.WFExecutionParameters.FirstOrDefault(p => p.WfExecID == wfExecID && p.WfExecParamName == paramName);
        }        
    }
}
