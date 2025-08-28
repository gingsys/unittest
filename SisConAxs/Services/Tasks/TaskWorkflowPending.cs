using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using SisConAxs_DM.Repository;
using System;
using System.Linq;
using System.Threading;

namespace SisConAxs.Services
{
    // Procesa los Workflow Execution que estan en estado Pendiente,
    // es decir los que aun NO se han ejecutado por primera vez
    public class TaskWorkflowPending : AbstractTask
    {
        public TaskWorkflowPending(TaskManager manager) : base(manager)
        {

        }

        public override void Execute(object state)
        {
            try
            {
                SisConAxsContext db = new SisConAxsContext();
                IQueryable<WorkflowExecution> workflows_execs = from wfx in db.WorkflowExecution
                                                                where wfx.WfExecStatus == (int)WfExecStatus.Pending
                                                                orderby
                                                                    wfx.WfExecStartDate
                                                                select wfx;
                LogManager.Debug($"> TaskWorkflowPending ({workflows_execs.Count()})");
                foreach (WorkflowExecution wfExec in workflows_execs)
                {
                    try
                    {
                        // No debe procesar Workflows que no tengan acciones!!
                        if (wfExec.Workflow.WorkflowItems != null && wfExec.Workflow.WorkflowItems.Count > 0)
                        {
                            try
                            {
                                if (this.WFExecuteRepository.SaveWFExecInitParams(db, wfExec))
                                {
                                    int firstWfItemId = wfExec.Workflow.WorkflowItems.FirstOrDefault(i => i.WfItemStep == 1).WfItemId;        // Primer item de Workflow
                                    this.WFExecuteRepository.SaveWFExecution(wfExec, firstWfItemId, WfExecStatus.ToProcess, wfExec.WfExecHistoryMessage);
                                }
                            }
                            catch (Exception ex)
                            {
                                LogManager.Error($"> TaskWorkflowPending [{this.GetCurrentDateStr()}] >> {ex.Message} - Solicitud ID: {wfExec.WfExecParentObjectID}");
                                if (ex is ModelException) // Si es un error manejado
                                {
                                    wfExec.WfResponse = (int)WfExecResponse.Rejected;   // Se establece la respuesta a rechazado
                                    this.WFExecuteRepository.SaveWFExecutionEnd(wfExec, WfExecStatus.FinishedRejected, ex.Message); // Termina la ejecución
                                }       
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManager.Error($"> TaskWorkflowPending [{System.DateTime.Now}] >> ", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"> TaskWorkflowPending [{System.DateTime.Now}] >> ", ex);
            }
            finally
            {
                Timer.Change(this.Lapse, Timeout.Infinite);
            }
        }
    }
}