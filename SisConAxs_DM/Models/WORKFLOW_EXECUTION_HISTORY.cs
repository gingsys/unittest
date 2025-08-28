using System;
using System.Collections.Generic;

namespace SisConAxs_DM.Models
{
    public partial class WorkflowExecutionHistory
    {
        public int WfExecHistoryID { get; set; }
        public Nullable<int> WfExecID { get; set; }
        public Nullable<int> WfExecWfID { get; set; }
        public Nullable<int> WfExecCurrentStep { get; set; }
        public int WfResponse { get; set; }
        public System.DateTime WfExecStartDate { get; set; }
        public string WfExecStartedBy { get; set; }
        public Nullable<int> WfExecParentObject { get; set; }
        public int WfExecObjectID { get; set; }
        public string WfExecObjectName { get; set; }
        public int WfExecObjectStatus { get; set; }
        public string WfExecHistoryMessage { get; set; }
        public int WfExecCompany { get; set; }
        public string WfExecApproverName{ get; set; }
        public string WfExecExecutorMail { get; set; }
        public int? wfExecApproverID { get; set; }
        public string WfExecApproverArea { get; set; }
        public string WfExecApproverPosition { get; set; }
        public virtual Workflow Workflow { get; set; }
        public virtual WorkflowItems WorkflowItems { get; set; }
    }
}
