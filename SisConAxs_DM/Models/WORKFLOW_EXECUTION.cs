using System;
using System.Collections.Generic;


namespace SisConAxs_DM.Models
{
    public partial class WorkflowExecution
    {
        public int WfExecID { get; set; }
        public int WfExecWfID { get; set; }
        public Nullable<int> WfExecCurrentStep { get; set; }
        public int WfResponse { get; set; }
        public System.DateTime WfExecStartDate { get; set; }        
        public string WfExecStartedBy { get; set; }
        public Nullable<int> WfExecParentObjectID { get; set; }
        public int WfExecObjectID { get; set; }
        public string WfExecObjectName { get; set; }
        public int WfExecStatus { get; set; }
        public string WfExecHistoryMessage { get; set; }
        public int WfExecCompany { get; set; }
        public virtual ICollection<WFExecutionParameters> WFExecutionParameters { get; set; }

        public virtual Workflow Workflow { get; set; }
        public virtual WorkflowItems WorkflowItem { get; set; }
    }
}
