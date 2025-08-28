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
    // Procesa los workflow execution que cuyo tiempo de espera haya expirado
    public class TaskWorkflowTimeout : AbstractTask
    {
        public TaskWorkflowTimeout(TaskManager manager) : base(manager)
        {
            this.Lapse = 60 * 1000; // 60 seg
        }

        public override void Execute(object state)
        {
            try
            {
                WorkflowItems nextItem = null;
                Nullable<int> dueTime = 0;
                Nullable<int> dueUnits = 0;
                SisConAxsContext db = new SisConAxsContext();
                IQueryable<WorkflowExecution> workflows_execs = from wfx in db.WorkflowExecution
                                                                where wfx.WfExecStatus == (int)WfExecStatus.WaitingResponse
                                                                orderby
                                                                    wfx.WfExecStartDate
                                                                select wfx;
                LogManager.Debug($">> TaskWorkflowTimeout ({workflows_execs.Count()})");
                foreach (WorkflowExecution wfExec in workflows_execs)
                {
                    try
                    {
                        var workflowItem = wfExec.WorkflowItem;
                        if (workflowItem.WfItemTimeoutDueTime > 0) // si el tiempo es mayor a 0 entonces realiza el chequeo de timeout
                        {
                            // Obtenemos el vencimiento y la unidad de tiempo
                            dueTime = workflowItem.WfItemTimeoutDueTime;
                            dueUnits = workflowItem.WfItemTimeoutDueUnits;

                            //Considerar solo dias habiles (Lunes a Viernes) - se excluyen feriados(consulta al sistema Indigo)
                            DateTime wfExecExpireDate = this.GetExpireDate(wfExec.WfExecStartDate, wfExec.WfExecCompany, dueTime, dueUnits, true);

                            // Si el plazo ha expirado va al siguiente paso                    
                            //if(DateTime.Now > wfExec.WfExecStartDate.AddSeconds((int)dueHours)) //2016-01-15 SOLO PRUEBAS!
                            if (DateTime.Now > wfExecExpireDate)
                            {
                                nextItem = WorkflowRepository.GetWfNextItem(wfExec, WfExecNextItem.Timeout);
                                if (nextItem == null)  // si no hay mas WorkflowItems se establece como rechazado y respondido para que sea procesado la proxima vez
                                {
                                    // se actualiza el ultimo aprobador a ninguno, ya que no se realizarán mas consultas
                                    this.WFExecuteRepository.SaveWFExecParam(wfExec.WfExecID, "approver", 0, "", DateTime.Now);
                                    this.WFExecuteRepository.SaveWFExecParam(wfExec.WfExecID, "approver_order", -1, "", DateTime.Now);

                                    wfExec.WfResponse = (int)WfExecResponse.Rejected;
                                    this.WFExecuteRepository.SaveWFExecution(
                                        wfExec, wfExec.WfExecCurrentStep, WfExecStatus.Responded,
                                        $"<span style='color:#555'>[{GetCurrentDateStr()}]&nbsp;&nbsp;</span> Tiempo expirado." // la respuesta se establece como <strong><span style='background-color:#E77272'>RECHAZADO</span></strong>.",
                                    );
                                    //SaveWorkflowEnd(db, wfExec, WfExecStatus.FinishedRejected, false, "Expiró el tiempo de espera para la aprobación.");
                                }
                                else
                                {
                                    this.WFExecuteRepository.SaveWFExecution(
                                        wfExec, nextItem.WfItemId, WfExecStatus.ToProcess,
                                        $"<span style='color:#555'>[{GetCurrentDateStr()}]&nbsp;&nbsp;</span> Expiró el tiempo de espera para la aprobación."
                                    );
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManager.Error("> TaskWorkflowTimeout >> ", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("> TaskWorkflowTimeout >> ", ex);
            }
            finally
            {
                Timer.Change(this.Lapse, Timeout.Infinite);
            }
        }
    }
}