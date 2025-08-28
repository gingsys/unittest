using System;
using System.Collections.Generic;

namespace SisConAxs_DM.Models
{
    public partial class Workflow
    {
        public Workflow()
        {
            this.WorkflowExecution = new List<WorkflowExecution>();
            this.WorkflowExecutionHistory = new List<WorkflowExecutionHistory>();
            this.WorkflowItems = new List<WorkflowItems>();
            this.AccessResources = new List<AccessResources>();
        }

        public int WfID { get; set; }
        public Nullable<int> WfApproveHierarchyID { get; set; }
        public string WfName { get; set; }
        public string WfDescription { get; set; }
        public int WfActivo { get; set; }
        public System.DateTime WfStartDate { get; set; }
        public Nullable<System.DateTime> WfEndDate { get; set; }
        //public string CreateUser { get; set; }
        //public Nullable<System.DateTime> CreateDate { get; set; }
        public string EditUser { get; set; }
        //public Nullable<System.DateTime> EditDate { get; set; }

        public int WfCompany { get; set; }

        public virtual ICollection<AccessResources> AccessResources { get; set; }
        public virtual ICollection<WorkflowExecution> WorkflowExecution { get; set; }
        public virtual ICollection<WorkflowExecutionHistory> WorkflowExecutionHistory { get; set; }
        public virtual ICollection<WorkflowItems> WorkflowItems { get; set; }
        public virtual WorkflowApproveHierarchy WorkflowApproveHierarchy { get; set; }
    }
}
