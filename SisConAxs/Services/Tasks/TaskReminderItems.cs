using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using SisConAxs_DM.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;

namespace SisConAxs.Services
{
    // Envio de Recordatorios de Aprobacion
    public class TaskReminderItems : AbstractTask
    {
        private List<int?> wfExcesRemidereds = new List<int?>();

        public TaskReminderItems(TaskManager manager) : base(manager)
        {
            //this.Lapse = 5 * 60 * 1000; // 5 Minutos
        }

        public override void Execute(object state)
        {
            try
            {
                SisConAxsContext db = new SisConAxsContext();
                IQueryable<WorkflowExecution> workflows_execs = from wfx in db.WorkflowExecution
                                                                where
                                                                    wfx.WfExecStatus == (int)WfExecStatus.WaitingResponse
                                                                    && wfx.WorkflowItem.WfItemTimeoutDueTime > 0
                                                                orderby
                                                                    wfx.WfExecStartDate
                                                                select wfx;
                LogManager.Debug($"> TaskReminderItems ({workflows_execs.Count()})");
                foreach (WorkflowExecution wfExec in workflows_execs)
                {
                    try
                    {
                        if (wfExec.WorkflowItem.WfItemTimeoutDueTime > 0)
                        {
                            //Considerar solo dias habiles (Lunes a Viernes) - se excluyen feriados(consulta al sistema Indigo)
                            DateTime wfExecExpireDate = this.GetExpireDate(
                                wfExec.WfExecStartDate,
                                wfExec.WfExecCompany,
                                wfExec.WorkflowItem.WfItemTimeoutDueTime,
                                wfExec.WorkflowItem.WfItemTimeoutDueUnits
                            );

                            DateTime Now = DateTime.Now;
                            DateTime WfDatetime = Convert.ToDateTime(wfExec.WfExecStartDate.ToString("HH:mm:ss"));

                            if (WfDatetime <= Now && Now <= WfDatetime.AddMinutes(5)
                                && Now >= wfExec.WfExecStartDate.AddDays(1)
                                && Now < wfExecExpireDate
                                && !this.wfExcesRemidereds.Exists(f => f == wfExec.WfExecID))
                            {
                                RequestEmailDataStorage.PrepareEmailData(wfExec, "[Recordatorio]: ");
                                this.wfExcesRemidereds.Add(wfExec.WfExecID);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManager.Error("> TaskReminderItems >> ", ex);
                    }
                }

                DateTime reset = Convert.ToDateTime(ConfigurationManager.AppSettings["ClearDatetimeReminders"]);
                if (DateTime.Now >= reset && DateTime.Now <= reset.AddSeconds(10))
                    wfExcesRemidereds = new List<int?>();
            }
            catch (Exception ex)
            {
                LogManager.Error("> TaskReminderItems >> ", ex);
            }
            finally
            {
                Timer.Change(this.Lapse, Timeout.Infinite);
            }
        }
    }
}