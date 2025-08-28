using System;
using System.Collections.Generic;

namespace SisConAxs_DM.DTO
{
    public class WorkflowHierarchyMembersDTO
    {
        public int WfHierarchyMemberID { get; set; }
        public int WfApproveHierarchyID { get; set; }
        public int WfHierarchyMemberCompany { get; set; }
        public string WfHierarchyMemberCompanyName { get; set; }
        public string WfHierarchyMemberCompanyDisplay { get; set; }
        public int WfHierarchyMemberDepartment { get; set; }
        public string WfHierarchyMemberDepartmentName { get; set; }
        public int WfHierarchyMemberPosition { get; set; }
        public string WfHierarchyMemberPositionName { get; set; }
        public int WfHierarchyMemberOrder { get; set; }
        public string WfHierarchyMemberDescription { get; set; }
        public string WfHierarchyMemberPerson { get; set; }
    }
}
