using System;
using System.Collections.Generic;

namespace SisConAxs_DM.Models
{
    public partial class WorkflowItems
    {
        public WorkflowItems()
        {
            this.WorkflowExecution = new List<WorkflowExecution>();
            this.WorkflowExecutionHistory = new List<WorkflowExecutionHistory>();
            this.WorkflowItemNext = new List<WorkflowItemNext>();
            this.WorkflowItemNextParents = new List<WorkflowItemNext>();            
        }

        public int WfItemId { get; set; }
        public int WfItemWfID { get; set; }
        public string WfItemName { get; set; }
        public int WfItemType { get; set; }
        public string WfItemSubject { get; set; }
        public string WfItemMessage { get; set; }
        public int WfItemStep { get; set; }
        public string WfItemEnterCondition { get; set; }
        public string WfItemEnterParams { get; set; }
        public Nullable<int> WfItemPrevSibling { get; set; }
        public string WfItemExitValues { get; set; }
        public Nullable<int> WfItemDestType { get; set; }
        public string WfItemDestMail { get; set; }
        public Nullable<int> WfItemCcType { get; set; }
        public string WfItemCcMail { get; set; }
        //public Nullable<int> WfItemNextItem { get; set; }
        //public Nullable<int> WfItemApproveItem { get; set; }
        //public Nullable<int> WfItemRejectItem { get; set; }
        //public Nullable<int> WfItemTimeoutItem { get; set; }
        public Nullable<int> WfItemTimeoutDueTime { get; set; }
        public Nullable<int> WfItemTimeoutDueUnits { get; set; }
        public Nullable<int> WfItemActionProperty { get; set; }
        public Nullable<int> WfItemActionValue { get; set; }
        //public string CreateUser { get; set; }
        //public Nullable<System.DateTime> CreateDate { get; set; }
        public string EditUser { get; set; }
        //public Nullable<System.DateTime> EditDate { get; set; }

        public virtual ICollection<WorkflowExecution> WorkflowExecution { get; set; }
        public virtual ICollection<WorkflowExecutionHistory> WorkflowExecutionHistory { get; set; }
        public virtual ICollection<WorkflowItemNext> WorkflowItemNext { get; set; }
        public virtual ICollection<WorkflowItemNext> WorkflowItemNextParents { get; set; }
        public virtual Workflow Workflow { get; set; }
        public virtual CommonValues CommonValues { get; set; }
    }
}
