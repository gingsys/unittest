using System;
using System.Collections.Generic;

namespace SisConAxs_DM.DTO
{
    public class AccessRequestDetailsDTO
    {
        public Nullable<int> RequestID { get; set; }
        public Nullable<int> RequestDetID { get; set; }
        public int ResourceID { get; set; }
        public string ResourceParamValue { get; set; }
        public Nullable<int> ResourceParent { get; set; }
        public int ResourceCategoryID { get; set; }
        public string ResourceCategoryName { get; set; }
        public string ResourceCategoryDisplay { get; set; }
        public string ResourceName { get; set; }
        public string ResourceFullName { get; set; }
        public Nullable<int> RequestDetStatus { get; set; }
        public string RequestDetStatusName { get; set; }
        public int RequestDetStep { get; set; }
        public Nullable<int> RequestDetType { get; set; }
        public string RequestDetTypeName { get; set; }
        public string RequestDetTypeDisplay { get; set; }
        public string RequestDetStrValue { get; set; }
        public Nullable<int> RequestDetIntValue { get; set; }
        public string RequestDetDisplayValue { get; set; }

        public int ReqDetTemporal { get; set; }
        public int ResourceTemporal { get; set; }
        public Nullable<DateTime> ReqDetValidityFrom { get; set; }
        public Nullable<DateTime> ReqDetValidityUntil { get; set; }

        public Nullable<int> RequestDetAdditional { get; set; }
        public string RequestDetAdditionalStrValue { get; set; }
        public Nullable<int> RequestDetAdditionalIntValue { get; set; }

        public bool RequestDetPrevData { get; set; }
        public bool RequestDetPending { get; set; }
        public bool ResourceOnlyAssignable { get; set; }

        public int ResourceAccessTypeID { get; set; }
        public int ResourceAccessTypeType { get; set; }
        public ICollection<AccessTypeValuesDTO> ResourceAccessTypeValues { get; set; }

        public int ResourceActive { get; set; }
        public int FlagIsApprover { get; set; }
    }
}