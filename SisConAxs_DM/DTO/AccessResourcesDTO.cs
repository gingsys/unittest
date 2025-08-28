using System;
using System.Collections.Generic;

namespace SisConAxs_DM.DTO
{
    public class AccessResourcesDTO
    {
        public const int TEMPORAL_NO = 0;
        public const int TEMPORAL_REQUIRED = 1;
        public const int TEMPORAL_OPTIONAL = 2;

        public int ResourceID { get; set; }
        public int ResourceCategory { get; set; }
        public string ResourceCategoryName { get; set; }
        public string ResourceName { get; set; }
        public string ResourceFullName { get; set;  }
        public string ResourceDescription { get; set; }
        public int ResourceAccessType { get; set; }
        public string ResourceAccessTypeName { get; set; }
        public int ResourceTemporal { get; set; }
        public int ResourceSendAtEnd { get; set; }
        public Nullable<int> ResourceParent { get; set; }
        public string ResourceParentName { get; set; }
        public Nullable<int> ResourceWorkflow { get; set; }
        public string ResourceWorkflowName { get; set; }
        public Nullable<int> ResourceDepartment { get; set; }
        public string ResourceDepartmentName { get; set; }
        public int ResourceLevel { get; set; }
        public int ResourceActive { get; set; }
        public int ResourceOnlyAssignable { get; set; }
        public Nullable<int> ResourceRequired { get; set; }
        public string ResourceRequiredName { get; set; }
        public int ResourceFlag { get; set; }

        public int ResourceCompany { get; set; }

        public ICollection<AccessResourceParameterDTO> ResourceParameters { get; set; } = new List<AccessResourceParameterDTO>();
    }
}