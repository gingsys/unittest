using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using System;
using System.Linq;
using System.Threading;

namespace SisConAxs.Services
{
    public class TaskCancelRequest : AbstractTask
    {
        public TaskCancelRequest(TaskManager manager) : base(manager)
        {
            
        }

        public override void Execute(object state)
        {
            try
            {
                SisConAxsContext db = new SisConAxsContext();
                IQueryable<WorkflowExecution> workflows_execs = from wfx in db.WorkflowExecution
                                                                where wfx.WfExecStatus == (int)WfExecStatus.CancelRequest
                                                                orderby
                                                                    wfx.WfExecStartDate
                                                                select wfx;

                if (workflows_execs != null && workflows_execs.Count() > 0)
                {
                    LogManager.Debug($"> TaskCancelRequest ({workflows_execs.Count()})");
                    AccessRequests request = null;
                    foreach (WorkflowExecution wfExec in workflows_execs)
                    {
                        try
                        {
                            request = db.AccessRequests.FirstOrDefault(t => t.RequestID == wfExec.WfExecParentObjectID && wfExec.WfExecObjectName == "ACCESS_REQUEST");
                            wfExec.WfResponse = (int)WfExecResponse.Rejected;
                            this.WFExecuteRepository.SaveWFExecutionEnd(
                                wfExec,
                                WfExecStatus.FinishedRejected,
                                $"Se ha cancelado la solicitud '{request.RequestNumber}' para '{request.PeopleRequestTo.GetFullName()}'."
                            );
                        }
                        catch (Exception ex)
                        {
                            LogManager.Error("> TaskCancelRequest >> ", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("> TaskCancelRequest >> ", ex);
            }
            finally
            {
                Timer.Change(this.Lapse, Timeout.Infinite);
            }
        }
    }
}