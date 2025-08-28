using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using SisConAxs_DM.Repository;
using SisConAxs_DM.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SisConAxs.Services
{
    public class TaskWokflowItemsNotify : AbstractTask
    {
        public TaskWokflowItemsNotify(TaskManager manager) : base(manager)
        {
        }

        public bool Process(WorkflowExecution wfExec)
        {
            SisConAxsContext db = new SisConAxsContext();
            AccessRequestDetails requestDetail = db.AccessRequestDetails.FirstOrDefault(t => t.RequestDetID == wfExec.WfExecObjectID);
            if (requestDetail != null)
            {
                int sendAtEnd = requestDetail.AccessResources.ResourceSendAtEnd;
                if (!(sendAtEnd == 1 && wfExec.WorkflowItem.WfItemDestType == (int)WfItemDestType.Ejecutor))  // Si no es de envío al finalizar y el destinatario es el Ejecutor
                {
                    ProcessNotifyItem(wfExec);
                }
            }
            else
            {
                LogManager.Error($"> TaskWorkflowItems.ProcessNotifyItem >> No existe un detalle de solicitud para este WorkflowExecution '{wfExec.WfExecID}'");
            }
            return true;
        }

        private bool ProcessNotifyItem(WorkflowExecution wfExec)
        {
            SisConAxsContext db = new SisConAxsContext();
            People people = null;
            string peopleFullName = "";
            string destEmail = "";
            string destEmailCc = ""; //Adicionado el 20191004 <-- DIANA CAMUS
            string errorMessage = "";
            bool emailConfiguredCorrectly = true;
            bool sendAccountMailer = false;

            // Destinatario ----------------------------------------------------------------------------------------------------- 
            AccessRequests request = db.AccessRequests.FirstOrDefault(r => r.RequestID == wfExec.WfExecParentObjectID);
            switch (wfExec.WorkflowItem.WfItemDestType)
            {
                case (int)WfItemDestType.Aprobador:
                    Nullable<int> approverID = this.WFExecuteRepository.GetWfExecIntParam(wfExec.WfExecID, "approver");
                    people = db.People.FirstOrDefault(p => p.PeopleID == approverID);
                    if (people != null)
                    {
                        peopleFullName = people.GetFullName();
                        destEmail = people.PeopleEmail;
                    }
                    break;
                case (int)WfItemDestType.Solicitante:
                    people = db.People.FirstOrDefault(p => p.UserID == request.RequestBy);
                    if (people != null)
                    {
                        peopleFullName = people.GetFullName();
                        destEmail = people.PeopleEmail;
                    }
                    break;
                case (int)WfItemDestType.SolicitadoPara:
                    people = db.People.FirstOrDefault(p => p.PeopleID == request.RequestTo);
                    if (people != null)
                    {
                        peopleFullName = people.GetFullName();
                        destEmail = people.PeopleEmail;
                    }
                    break;
                case (int)WfItemDestType.Otro:
                    peopleFullName = "Otro";
                    destEmail = wfExec.WorkflowItem.WfItemDestMail;
                    break;
                case (int)WfItemDestType.Ejecutor:
                    peopleFullName = "Ejecutor";
                    destEmail = wfExec.WorkflowItem.WfItemDestMail;
                    sendAccountMailer = true;
                    break;
            }

            //INICIO Adicionado el 20191004 <-- DIANA CAMUS
            // Con Copia ----------------------------------------------------------------------------------------------------- 
            if (wfExec.WorkflowItem.WfItemCcType.HasValue && wfExec.WorkflowItem.WfItemCcType.Value > 0)
            {
                People peopleCc = null;
                switch (wfExec.WorkflowItem.WfItemCcType)
                {
                    case (int)WfItemDestType.Aprobador:
                        Nullable<int> approverID = this.WFExecuteRepository.GetWfExecIntParam(wfExec.WfExecID, "approver");
                        peopleCc = db.People.FirstOrDefault(p => p.PeopleID == approverID);
                        if (peopleCc != null)
                        {
                            destEmailCc = peopleCc.PeopleEmail;
                        }
                        break;
                    case (int)WfItemDestType.Solicitante:
                        peopleCc = db.People.FirstOrDefault(p => p.UserID == request.RequestBy);
                        if (peopleCc != null)
                        {
                            destEmailCc = peopleCc.PeopleEmail;
                        }
                        break;
                    case (int)WfItemDestType.SolicitadoPara:
                        peopleCc = db.People.FirstOrDefault(p => p.PeopleID == request.RequestTo);
                        if (peopleCc != null)
                        {
                            destEmailCc = peopleCc.PeopleEmail;
                        }
                        break;
                    case (int)WfItemDestType.Otro:
                        destEmailCc = wfExec.WorkflowItem.WfItemCcMail;
                        break;
                    case (int)WfItemDestType.Ejecutor:
                        destEmailCc = wfExec.WorkflowItem.WfItemDestMail;
                        //sendAccountMailer = true;
                        break;
                }
            }
            //FIN Adicionado el 20191004 <-- DIANA CAMUS

            // en caso se requiera tener la historia del correo de rechazo
            //if (wfExec.WfResponse == (int)WfExecResponse.Rejected)
            //{ sendAccountMailer = true; }

            // VALIDACIONES ================================================================================================================================= //
            if ((wfExec.WorkflowItem.WfItemDestType == (int)WfItemDestType.Aprobador ||
                 wfExec.WorkflowItem.WfItemDestType == (int)WfItemDestType.Solicitante ||
                 wfExec.WorkflowItem.WfItemDestType == (int)WfItemDestType.SolicitadoPara)
               && String.IsNullOrEmpty(peopleFullName)
            )
            {
                switch (wfExec.WorkflowItem.WfItemDestType)
                {
                    case (int)WfItemDestType.Aprobador:
                        errorMessage = "No se puede envíar el correo porque no se encontró el registro de persona para el aprobador";
                        LogManager.Error($"> TaskWorkflowItems.ProcessNotifyItem >> {errorMessage}.\nSolicitud N° {request.RequestNumber}.");
                        break;
                    case (int)WfItemDestType.Solicitante:
                        errorMessage = "No se puede envíar el correo porque no se encontró el registro de persona para el solicitante";
                        LogManager.Error($"> TaskWorkflowItems.ProcessNotifyItem >> {errorMessage}.\nSolicitud N° {request.RequestNumber}\nUsuario: {request.RequestBy}.");
                        break;
                    case (int)WfItemDestType.SolicitadoPara:
                        errorMessage = "No se puede envíar el correo porque no se encontró el registro para la persona a la que se le solicita los accesos";
                        LogManager.Error($"> TaskWorkflowItems.ProcessNotifyItem >> {errorMessage}.\n Solicitud N° {request.RequestNumber}.");
                        break;
                    //case 4: // otro
                    //    LogManager.Error(String.Format("> TaskWorkflowItems.ProcessNotifyItem2 >> No se puede preparar el correo porque no hay una dirección de destino [{0}]",
                    //                                   peopleFullName));
                    //    break;
                }
                emailConfiguredCorrectly = false;
            }
            if (emailConfiguredCorrectly && String.IsNullOrEmpty(destEmail))
            {
                errorMessage = $"No se puede envíar el correo porque no hay una dirección de destino [{peopleFullName}]";
                LogManager.Error($"> TaskWorkflowItems.ProcessNotifyItem >> {errorMessage}\n Solicitud N° {request.RequestNumber}.");
                emailConfiguredCorrectly = false;
            }
            // VALIDACIONES ================================================================================================================================= //

            //INICIO Adicionado el 20191004 <-- DIANA CAMUS
            // VALIDACIONES CC ================================================================================================================================= //
            if (wfExec.WorkflowItem.WfItemCcType.HasValue && emailConfiguredCorrectly && String.IsNullOrEmpty(destEmailCc))
            {
                var errorMessageCc = $"No se puede envíar el correo copia porque no se encontró una dirección de destino válido.";
                LogManager.Error($"> TaskWorkflowItems.ProcessNotifyItem >> {errorMessageCc}\n Solicitud N° {request.RequestNumber}.");
            }
            // VALIDACIONES ================================================================================================================================= //
            //FIN Adicionado el 20191004 <-- DIANA CAMUS

            // Si llega a esta parte es porque se llegó a aprobar la solicitud
            //SaveRequestDetailStatus(db, wfExec, WfExecStatus.FinishedApproved, false);
            if (emailConfiguredCorrectly)
            {
                // Prepara el Correo                
                RequestEmailDataStorage.PrepareEmailData(wfExec, "",
                                                         destEmail,  //wfExec.WorkflowItem.WfItemDestType <= 3 ? "" : destEmail
                                                         sendAccountMailer,
                                                         destEmailCc);  //Modificado el 20191004 <-- DIANA CAMUS
            }

            string message = emailConfiguredCorrectly ? $"Se Notificó a <strong>[{peopleFullName} - {destEmail}]</strong>" : $"{errorMessage}.";

            WorkflowItems nextItem = WorkflowRepository.GetWfNextItem(wfExec);
            if (nextItem == null)
            {
                //SaveWorkflowEnd(db, wfExec);
                // JONATAN LOBATO P.
                // 2016-02-01 Finaliza el workflow cuando cuando el tiempo Expiro
                this.WFExecuteRepository.SaveWFExecutionEnd(
                    wfExec, WfExecStatus.FinishedTimeout,
                    message
                );
                // JONATAN LOBATO P.
            }
            else
            {
                //grabar el workflow_execute con el nuevo next item
                this.WFExecuteRepository.SaveWFExecution(
                    wfExec, nextItem.WfItemId, WfExecStatus.ToProcess,
                    $"<span style='color:#555'>[{GetCurrentDateStr()}]&nbsp;&nbsp;</span> {message}"
                );
            }

            return true;
        }

        // Procesa los items que se envían al finalizar (los itemas se agrupan en un sólo correo al ejecutor)
        public bool ProcessNotifyItemForSendAtEnd()
        {
            SisConAxsContext db = new SisConAxsContext();

            var requestsToProcess = (from R in
                                         (
                                             from reqdet in db.AccessRequestDetails
                                             join wfexec in db.WorkflowExecution on new { objectName = "ACCESS_REQUEST", objectID = reqdet.RequestDetID } equals
                                                                                    new { objectName = wfexec.WfExecObjectName, objectID = wfexec.WfExecObjectID }
                                             where
                                               //reqdet.CommonValuesStatus.CommonValueName == "S_PENDIENTE" &&  // pendientes
                                               reqdet.ReqDetSendAtEnd == 1 &&  // Enviar al finalizar
                                               wfexec.WfExecStatus < 10        // Workflows que no han terminado
                                             select new
                                             {
                                                 requestID = reqdet.RequestID,
                                                 reqDet = reqdet,
                                                 wfExec = wfexec
                                             }
                                           )
                                     group R by R.requestID
                                    ).Where(x =>
                                               x.All(y => y.wfExec.WfResponse != -1) // si todos los items ya tienen respuesta
                                           );

            foreach (var p in requestsToProcess)
            {
                var query = p.Where(t => t.wfExec.WorkflowItem.WfItemType == WorkflowItemsDTO.TYPE_NOTIFICATION
                                         && t.wfExec.WorkflowItem.WfItemDestType == (int)WfItemDestType.Ejecutor
                                         && t.wfExec.WfResponse == (int)WfExecResponse.Approved
                            ).GroupBy(
                                t => new { t.requestID, t.wfExec.WfExecWfID, t.wfExec.WfExecCurrentStep, t.wfExec.WorkflowItem.WfItemDestMail }
                            );

                if (query.Count() > 0)  // si encuentra items que cumplan con las condiciones
                {
                    foreach (var group in query)
                    {
                        if (group.Count() == p.Count(t => t.requestID == group.Key.requestID && t.wfExec.WfExecWfID == group.Key.WfExecWfID))
                        {
                            foreach (var item in group)
                            {
                                if (!ProcessNotifyItem(item.wfExec))
                                    break;
                            }
                        }
                    }
                }
            }


            //var request = from reqdet in db.AccessRequestDetails
            //              join wfexec in db.WorkflowExecution on new { objectName = "ACCESS_REQUEST", objectID = reqdet.RequestDetID } equals new { objectName = wfexec.WfExecObjectName, objectID = wfexec.WfExecObjectID }
            //              where
            //                  reqdet.CommonValuesStatus.CommonValueName == "S_PENDIENTE"  // pendientes
            //                  && reqdet.ReqDetSendAtEnd == 1  // enviar al finalizar
            //                  && wfexec.WfExecStatus < 10     // workflows que no han terminado
            //                  && wfexec.WfResponse == -1      // sin respuesta
            //              group reqdet by reqdet.RequestID into grp
            //              select grp.Key;

            //var details = from reqdet in db.AccessRequestDetails
            //              join wfexec in db.WorkflowExecution on new { objectName = "ACCESS_REQUEST", objectID = reqdet.RequestDetID } equals new { objectName = wfexec.WfExecObjectName, objectID = wfexec.WfExecObjectID }
            //              where
            //                  !request.Contains(reqdet.RequestID)
            //                  && reqdet.CommonValuesStatus.CommonValueName == "S_PENDIENTE"
            //                  && reqdet.ReqDetSendAtEnd == 1    // enviar al finalizar
            //                  && wfexec.WfResponse == 1     // aprobado
            //              select new
            //              {
            //                  reqID = reqdet.RequestID,
            //                  reqdetID = reqdet.RequestDetID,
            //                  wfID = reqdet.AccessResources.ResourceWorkflow
            //              };

            //if (details.Count() > 0)    // si encuentra detalles
            //{
            //    var query = db.WorkflowExecution.Where(
            //        t => t.WfExecStatus == (int)WfExecStatus.ToProcess
            //                                && t.WfExecObjectName == "ACCESS_REQUEST"
            //                                && details.Any(u => u.reqdetID == t.WfExecObjectID)
            //                                && t.WorkflowItems.CommonValues.CommonValueName == "NOTIFICACION"
            //                                && t.WorkflowItems.WfItemDestType == 5 // ejecutor
            //                                && t.WfResponse == 1 // aprobado
            //    ).GroupBy(
            //        t => new { t.WfExecParentObjectID, t.WfExecWfID, t.WfExecCurrentStep, t.WorkflowItems.WfItemDestMail }
            //    );

            //    if (query.Count() > 0)  // si encuentra wfexecutions que cumplan con las condiciones
            //    {
            //        foreach (var group in query)
            //        {
            //            if (group.Count() == details.Count(t => t.reqID == group.Key.WfExecParentObjectID && t.wfID == group.Key.WfExecWfID))
            //            {
            //                foreach (var item in group)
            //                {
            //                    if (!ProcessNotifyItem2(item))
            //                        break;
            //                }
            //            }
            //        }
            //    }
            //}
            return true;
        }


        public override void Execute(object state)
        {
            throw new NotImplementedException();
        }
    }
}