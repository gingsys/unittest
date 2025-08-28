using System;
using System.Collections.Generic;

namespace SisConAxs_DM.Models
{
    public partial class WFExecutionParameters
    {
        public int WfExecParamID { get; set; }
        public int WfExecID { get; set; }
        public string WfExecParamName { get; set; }
        public Nullable<int> WfExecParamIntValue { get; set; }
        public string WfExecParamStrValue { get; set; }
        public Nullable<System.DateTime> WfExecParamDateValue { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string EditUser { get; set; }
        public System.DateTime EditDate { get; set; }

        public virtual WorkflowExecution WorkflowExecution { get; set; }
    }
}
