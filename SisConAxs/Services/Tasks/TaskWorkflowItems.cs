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
    // tienen un item listo para procesar.
    public class TaskWorkflowItems : AbstractTask
    {
        private TaskWorkflowItemsAsk ItemAsk;
        private TaskWokflowItemsNotify ItemNotification;

        public TaskWorkflowItems(TaskManager manager) : base(manager)
        {
            ItemAsk = new TaskWorkflowItemsAsk(this.Manager);
            ItemNotification = new TaskWokflowItemsNotify(this.Manager);
        }

        public override void Execute(object state)
        {
            try
            {
                // 1ro procesa los correos que son de envío al finalizar las aprobaciones
                try
                {
                    ItemNotification.ProcessNotifyItemForSendAtEnd();    
                }
                catch (Exception ex)
                {
                    LogManager.Error("> TaskWorkflowItems.ProcessNotifyItemForSendAtEnd >> ", ex);
                }


                // Luego los items a procesar
                SisConAxsContext db = new SisConAxsContext();
                IQueryable<WorkflowExecution> workflows_execs = from wfx in db.WorkflowExecution
                                                                where
                                                                    wfx.WfExecStatus == (int)WfExecStatus.ToProcess &&
                                                                    (wfx.WorkflowItem.WfItemType == WorkflowItemsDTO.TYPE_NOTIFICATION || wfx.WorkflowItem.WfItemType == WorkflowItemsDTO.TYPE_ASK)
                                                                orderby
                                                                    wfx.WfExecStartDate
                                                                select wfx;
                LogManager.Debug($"> TaskWorkflowItems ({workflows_execs.Count()})");
                foreach (WorkflowExecution wfExec in workflows_execs)
                {
                    try
                    {
                        switch (wfExec.WorkflowItem.WfItemType)
                        {
                            case (WorkflowItemsDTO.TYPE_NOTIFICATION):  // NOTIFICACION
                                ItemNotification.Process(wfExec);
                                break;
                            case (WorkflowItemsDTO.TYPE_ASK):           // CONSULTA
                                ItemAsk.Process(wfExec);
                                break;
                            //case ("EVALUACION"):
                            //    ProcessEvaluationItem(wfExec);
                            //    break;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManager.Error("> TaskWorkflowItems >> ", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("> TaskWorkflowItems >> ", ex);
            }
            finally
            {
                Timer.Change(this.Lapse, Timeout.Infinite);
            }
        }
        
        //private void ProcessEvaluationItem(WorkflowExecution wfExec)
        //{
        //    // uso futuro
        //}
    }
}