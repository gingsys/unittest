using System;
using System.Collections.Generic;

namespace SisConAxs_DM.Models
{
    public partial class WorkflowHierarchyMembers
    {
        public WorkflowHierarchyMembers()
        {
        }

        public int WfHierarchyMemberID { get; set; }
        public int WfApproveHierarchyID { get; set; }
        public int WfHierarchyMemberCompany { get; set; }
        public int WfHierarchyMemberDepartment { get; set; }
        public int WfHierarchyMemberPosition { get; set; }
        public int WfHierarchyMemberOrder { get; set; }
        public string WfHierarchyMemberDescription { get; set; }
        //public string CreateUser { get; set; }
        //public Nullable<System.DateTime> CreateDate { get; set; }
        public string EditUser { get; set; }
        //public Nullable<System.DateTime> EditDate { get; set; }

        public virtual WorkflowApproveHierarchy WorkflowApproveHierarchy { get; set; }
        public virtual CommonValues CommonValues { get; set; }
        public virtual CommonValues CommonValues1 { get; set; }
    }
}
