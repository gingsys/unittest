using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using SisConAxs_DM.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace SisConAxs.Services
{
    // Procesa la respuesta del aprobador
    public class TaskResponseItems : AbstractTask
    {
        public TaskResponseItems(TaskManager manager) : base(manager)
        {

        }

        public override void Execute(object state)
        {
            try
            {
                WorkflowItems nextItem = null;
                SisConAxsContext db = new SisConAxsContext();
                IQueryable<WorkflowExecution> workflows_execs = from wfx in db.WorkflowExecution
                                                                where wfx.WfExecStatus == (int)WfExecStatus.Responded
                                                                orderby
                                                                    wfx.WfExecStartDate
                                                                select wfx;
                AccessRequestDetails requestDetail = null;
                LogManager.Debug($"> TaskResponseItems ({workflows_execs.Count()})");
                foreach (WorkflowExecution wfExec in workflows_execs)
                {
                    try
                    {
                        requestDetail = db.AccessRequestDetails.FirstOrDefault(t => t.RequestDetID == wfExec.WfExecObjectID && wfExec.WfExecObjectName == "ACCESS_REQUEST");

                        string approver = this.WFExecuteRepository.GetWfExecStringParam(wfExec.WfExecID, "approver");
                        string typeDisplay = requestDetail.CommonValuesType.CommonValueDisplay.ToUpper();
                        string typeColor = "";
                        if (requestDetail.RequestDetType == AccessRequestDTO.TYPE_ALTA)
                        {
                            typeColor = "#009900";
                        }
                        else if(requestDetail.RequestDetType == AccessRequestDTO.TYPE_MODIFICACION)
                        {
                            typeColor = "#FFA500";
                        }
                        else if (requestDetail.RequestDetType == AccessRequestDTO.TYPE_BAJA)
                        {
                            typeColor = "#C80000";
                        }

                        switch (wfExec.WfResponse)
                        {
                            case (int)WfExecResponse.Approved:
                                nextItem = WorkflowRepository.GetWfNextItem(wfExec, WfExecNextItem.Approved);
                                if (nextItem == null)
                                {
                                    this.WFExecuteRepository.SaveWFExecutionEnd(wfExec, WfExecStatus.FinishedApproved);
                                }
                                else
                                {
                                    this.WFExecuteRepository.SaveWFExecution(
                                        wfExec, nextItem.WfItemId, WfExecStatus.ToProcess,
                                        $"<span style='color:#555'>[{GetCurrentDateStr()}]&nbsp;&nbsp;</span> Se <strong><span style='background-color:#6CD865'>APROBÓ</span></strong> la " +
                                        $"<strong><span style='color:{typeColor}'>'{typeDisplay}'</span></strong> del acceso por <strong>'{approver}'</strong>."
                                    );
                                    //if (WorkflowRepository.SearchNextWorkflowItemByType(wfExec, "CONSULTA") == null)
                                    //{
                                        //AccessRequestRepository.SaveRequestDetailStatus(wfExec, WfExecStatus.FinishedApproved);
                                    //}
                                }
                                this.WFExecuteRepository.DeleteWFExecParam(wfExec.WfExecID, "approver_by");//INICIO Adicionado el 20191107 <-- DIANA CAMUS
                                break;
                            case (int)WfExecResponse.Rejected:
                                nextItem = WorkflowRepository.GetWfNextItem(wfExec, WfExecNextItem.Rejected);
                                if (nextItem == null)
                                {
                                    this.WFExecuteRepository.SaveWFExecutionEnd(wfExec, WfExecStatus.FinishedRejected);
                                }
                                else
                                {
                                    string message = "";
                                    if (!String.IsNullOrWhiteSpace(approver))
                                    {
                                        message = $"<span style='color:#555'>[{GetCurrentDateStr()}]&nbsp;&nbsp;</span> Se <strong><span style='background-color:#E77272'>RECHAZÓ</span></strong> la " +
                                                  $"<strong><span style='color:{typeColor}'>'{typeDisplay}'</span></strong> del acceso por <strong>'{approver}'</strong>.";
                                    }
                                    else
                                    {   // Si no tiene aprobador es porque viene de un tiempo expirado, no se ha encontrado al aprobador o por validaciones anteriores.
                                        message = $"<span style='color:#555'>[{GetCurrentDateStr()}]&nbsp;&nbsp;</span> Se <strong><span style='background-color:#E77272'>RECHAZÓ</span></strong> la " +
                                                  $"<strong><span style='color:{typeColor}'>'{typeDisplay}'</span></strong> del acceso porque el tiempo de espera para la respuesta ha expirado o por validaciones anteriores.";
                                    }
                                    this.WFExecuteRepository.SaveWFExecution(wfExec, nextItem.WfItemId, WfExecStatus.ToProcess, message);
                                }
                                //aqui borrar
                                this.WFExecuteRepository.DeleteWFExecParam(wfExec.WfExecID, "approver_by");
                                break;
                            default:
                                nextItem = null;
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManager.Error("> TaskResponseItems >> ", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("> TaskResponseItems >> ", ex);
            }
            finally
            {
                Timer.Change(this.Lapse, Timeout.Infinite);
            }
        }
    }
}