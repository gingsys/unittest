using System;
using System.Collections.Generic;

namespace SisConAxs_DM.Models
{
    public partial class WorkflowApproveHierarchy
    {
        public WorkflowApproveHierarchy()
        {
            this.WorkflowHierarchyMembers = new List<WorkflowHierarchyMembers>();
            this.Workflow = new List<Workflow>();
        }

        public int WfApproveHierarchyID { get; set; }
        public string WfApproveHierarchyName { get; set; }
        public int? WfApproveHierarchyDepartment { get; set; }
        //public string CreateUser { get; set; }
        //public Nullable<System.DateTime> CreateDate { get; set; }
        public string EditUser { get; set; }
        //public Nullable<System.DateTime> EditDate { get; set; }

        public int WfApproveHierarchyCompany { get; set; }

        //public virtual Workflow Workflow { get; set; }
        public virtual ICollection<WorkflowHierarchyMembers> WorkflowHierarchyMembers { get; set; }
        public virtual ICollection<Workflow> Workflow { get; set; }
        public virtual CommonValues Department { get; set; }
    }
}
