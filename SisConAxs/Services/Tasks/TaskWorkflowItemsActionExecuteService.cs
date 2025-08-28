using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using SisConAxs_DM.Repository;
using SisConAxs.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SisConAxs.Services.Tasks
{
    public class TaskWorkflowItemsActionExecuteService : AbstractTask
    {
        public TaskWorkflowItemsActionExecuteService(TaskManager manager) : base(manager)
        {
        }

        public bool Process(WorkflowExecution wfExec, WorkflowItems wfItem)
        {
            if ((int)wfItem.WfItemActionValue == 1)
            {
                var integrationService = IntegrationResourceFactory.Get((int)wfItem.WfItemActionValue);
                if (integrationService != null)
                {
                    var requestDetail = AccessRequestRepository.GetDetailByID(wfExec.WfExecObjectID);
                    if (requestDetail.RequestDetType == AccessRequestDTO.TYPE_ALTA || requestDetail.RequestDetType == AccessRequestDTO.TYPE_MODIFICACION)
                    {
                        integrationService.SavePeopleAccess(
                            wfExec,
                            requestDetail,
                            OnSucess(integrationService, wfExec),
                            OnError(integrationService, wfExec, requestDetail)
                        );
                    }
                    else if (requestDetail.RequestDetType == AccessRequestDTO.TYPE_BAJA)
                    {
                        integrationService.DeletePeopleAccess(
                            wfExec,
                            requestDetail,
                            OnSucess(integrationService, wfExec),
                            OnError(integrationService, wfExec, requestDetail)
                        );
                    }
                    return true;
                }
                else
                {
                    wfExec.WfResponse = (int)WfExecResponse.Rejected;  // se establece como rechazado
                    this.WFExecuteRepository.SaveWFExecutionEnd(wfExec, WfExecStatus.FinishedRejected, $"Servicio no válido o no implementado. Rechazado por el sistema.");
                    return false;
                }
            }
            else if ((int)wfItem.WfItemActionValue == 3)
            {
                var integrationService = IntegrationResourceFactory.GetIcarusAccess((int)wfItem.WfItemActionValue);
                if (integrationService != null)
                {
                    var requestDetail = AccessRequestRepository.GetDetailByID(wfExec.WfExecObjectID);
                    if (requestDetail.RequestDetType == AccessRequestDTO.TYPE_ALTA || requestDetail.RequestDetType == AccessRequestDTO.TYPE_MODIFICACION)
                    {
                        integrationService.SavePeopleAccess(
                            wfExec,
                            requestDetail,
                            OnSucess(integrationService, wfExec),
                            OnError(integrationService, wfExec, requestDetail)
                        );
                    }
                    else if (requestDetail.RequestDetType == AccessRequestDTO.TYPE_BAJA)
                    {
                        integrationService.DeletePeopleAccess(
                            wfExec,
                            requestDetail,
                            OnSucess(integrationService, wfExec),
                            OnError(integrationService, wfExec, requestDetail)
                        );
                    }
                    return true;
                }
                else
                {
                    wfExec.WfResponse = (int)WfExecResponse.Rejected;  // se establece como rechazado
                    this.WFExecuteRepository.SaveWFExecutionEnd(wfExec, WfExecStatus.FinishedRejected, $"Servicio no válido o no implementado. Rechazado por el sistema.");
                    return false;
                }
            }
            else
            {
                wfExec.WfResponse = (int)WfExecResponse.Rejected;  // se establece como rechazado
                this.WFExecuteRepository.SaveWFExecutionEnd(wfExec, WfExecStatus.FinishedRejected, $"Servicio no válido o no implementado. Rechazado por el sistema.");
                return false;
            }
            
        }

        public override void Execute(object state)
        {
            throw new NotImplementedException();
        }

        private Action<string, AccessRequestDetails> OnSucess(IIntegrationResource integrationService, WorkflowExecution wfExec)
        {
            return (string response, AccessRequestDetails requestDetail) =>
            {
                string action = "";
                if (requestDetail.RequestDetType == AccessRequestDTO.TYPE_ALTA)
                    action = "alta";
                else if (requestDetail.RequestDetType == AccessRequestDTO.TYPE_MODIFICACION)
                    action = "modificación";
                else if (requestDetail.RequestDetType == AccessRequestDTO.TYPE_BAJA)
                    action = "baja";

                var nextItem = WorkflowRepository.GetWfNextItem(wfExec);
                if (nextItem == null)
                {
                    this.WFExecuteRepository.SaveWFExecutionEnd(wfExec, WfExecStatus.FinishedUndefinied, $"Servicio {integrationService.GetName()} ejecutado. Se realizó la {action} del acceso solicitado.");
                }
                else
                {
                    this.WFExecuteRepository.SaveWFExecution(
                        wfExec, nextItem.WfItemId, WfExecStatus.ToProcess,
                        $"<span style='color:#555'>[{GetCurrentDateStr()}]&nbsp;&nbsp;</span> Servicio {integrationService.GetName()} ejecutado. Se realizó la {action} del acceso solicitado."
                    );
                }
            };
        }
        private Action<string, AccessRequestDetails> OnSucess(IIntegrationIcarusAccess integrationService, WorkflowExecution wfExec)
        {
            return (string response, AccessRequestDetails requestDetail) =>
            {
                string action = "";
                if (requestDetail.RequestDetType == AccessRequestDTO.TYPE_ALTA)
                    action = "alta";
                else if (requestDetail.RequestDetType == AccessRequestDTO.TYPE_MODIFICACION)
                    action = "modificación";
                else if (requestDetail.RequestDetType == AccessRequestDTO.TYPE_BAJA)
                    action = "baja";

                var nextItem = WorkflowRepository.GetWfNextItem(wfExec);
                if (nextItem == null)
                {
                    this.WFExecuteRepository.SaveWFExecutionEnd(wfExec, WfExecStatus.FinishedUndefinied, $"Servicio {integrationService.GetName()} ejecutado. Se realizó la {action} del acceso solicitado.");
                }
                else
                {
                    this.WFExecuteRepository.SaveWFExecution(
                        wfExec, nextItem.WfItemId, WfExecStatus.ToProcess,
                        $"<span style='color:#555'>[{GetCurrentDateStr()}]&nbsp;&nbsp;</span> Servicio {integrationService.GetName()} ejecutado. Se realizó la {action} del acceso solicitado."
                    );
                }
            };
        }
        private Action<IntegrationResourceException> OnError(IIntegrationResource integrationService, WorkflowExecution wfExec, AccessRequestDetails requestDetail)
        {
            return (IntegrationResourceException ex) =>
            {
                string action = "";
                if (requestDetail.RequestDetType == AccessRequestDTO.TYPE_ALTA)
                    action = "ALTA";
                else if (requestDetail.RequestDetType == AccessRequestDTO.TYPE_MODIFICACION)
                    action = "MODIFICACION";
                else if (requestDetail.RequestDetType == AccessRequestDTO.TYPE_BAJA)
                    action = "BAJA";

                LogManager.Error($"> TaskWorkflowItemsActionExecuteService >> Error al ejecutar acción de {action} en el Servicio {integrationService.GetName()}", ex);

                // si es un error en las validaciones tanto del cliente o servidor, se rechaza
                if (ex.Code == IntegrationResourceException.ERROR_RESOURCE_WITHOUT_EXTERNAL_ID || ex.Code == IntegrationResourceException.ERROR_CLIENT_VALIDATION || ex.Code == IntegrationResourceException.ERROR_SERVER_VALIDATION)
                {
                    wfExec.WfResponse = (int)WfExecResponse.Rejected;  // se establece como rechazado
                    this.WFExecuteRepository.SaveWFExecutionEnd(wfExec, WfExecStatus.FinishedRejected, $"{ex.Message} Rechazado por el sistema.");
                }
            };
        }
        private Action<IntegrationResourceException> OnError(IIntegrationIcarusAccess integrationService, WorkflowExecution wfExec, AccessRequestDetails requestDetail)
        {
            return (IntegrationResourceException ex) =>
            {
                string action = "";
                if (requestDetail.RequestDetType == AccessRequestDTO.TYPE_ALTA)
                    action = "ALTA";
                else if (requestDetail.RequestDetType == AccessRequestDTO.TYPE_MODIFICACION)
                    action = "MODIFICACION";
                else if (requestDetail.RequestDetType == AccessRequestDTO.TYPE_BAJA)
                    action = "BAJA";

                LogManager.Error($"> TaskWorkflowItemsActionExecuteService >> Error al ejecutar acción de {action} en el Servicio {integrationService.GetName()}", ex);

                // si es un error en las validaciones tanto del cliente o servidor, se rechaza
                if (ex.Code == IntegrationResourceException.ERROR_RESOURCE_WITHOUT_EXTERNAL_ID || ex.Code == IntegrationResourceException.ERROR_CLIENT_VALIDATION || ex.Code == IntegrationResourceException.ERROR_SERVER_VALIDATION)
                {
                    wfExec.WfResponse = (int)WfExecResponse.Rejected;  // se establece como rechazado
                    this.WFExecuteRepository.SaveWFExecutionEnd(wfExec, WfExecStatus.FinishedRejected, $"{ex.Message} Rechazado por el sistema.");
                }
            };
        }
    }
}