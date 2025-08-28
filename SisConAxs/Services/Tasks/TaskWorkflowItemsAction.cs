using SisConAxs.Services.Tasks;
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
    // Procesa los Workflow Execution que estan en estado por Procesar (1), es decir los que ya
    // tienen un item listo para procesar y sean de tipo ACCION.
    public class TaskWorkflowItemsAction : AbstractTask
    {
        public TaskWorkflowItemsAction(TaskManager manager) : base(manager)
        {
            this.Lapse = 30 * 1000; // 30 seg
        }

        public override void Execute(object state)
        {
            try
            {
                SisConAxsContext db = new SisConAxsContext();
                IQueryable<WorkflowExecution> workflows_execs = from wfx in db.WorkflowExecution
                                                                where wfx.WfExecStatus == (int)WfExecStatus.ToProcess &&
                                                                      wfx.WorkflowItem.WfItemType == WorkflowItemsDTO.TYPE_ACTION
                                                                orderby
                                                                    wfx.WfExecStartDate
                                                                select wfx;
                LogManager.Debug($"> TaskWorkflowItemsAction ({workflows_execs.Count()})");
                foreach (WorkflowExecution wfExec in workflows_execs)
                {
                    try
                    {
                        var WFItem = wfExec.WorkflowItem;
                        if (WFItem.WfItemActionProperty == WorkflowItemsDTO.ACTION_TYPE_CHANGE_APPROVER)
                        {
                            new TaskWorkflowItemsActionChangeApprover(Manager).Process(wfExec);
                        }
                        else if (WFItem.WfItemActionProperty == WorkflowItemsDTO.ACTION_TYPE_EXECUTE_IN_SERVER)
                        {
                            new TaskWorkflowItemsActionExecuteService(Manager).Process(wfExec, WFItem);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManager.Error("> TaskWorkflowItemsAction >> ", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("> TaskWorkflowItemsAction >> ", ex);
            }
            finally
            {
                Timer.Change(this.Lapse, Timeout.Infinite);
            }
        }
    }
}