using System;
using System.Collections.Generic;

namespace SisConAxs_DM.DTO
{
    public class WorkflowApproveHierarchyDTO
    {
        public int WfApproveHierarchyID { get; set; }
        public string WfApproveHierarchyName { get; set; }
        public int? WfApproveHierarchyDepartment { get; set; }
        public string WfApproveHierarchyDepartmentName { get; set; }
        //public int WfApproveHierarchyCompany { get; set; }

        public ICollection<WorkflowHierarchyMembersDTO> WorkflowHierarchyMembers { get; set; }
    }
}
