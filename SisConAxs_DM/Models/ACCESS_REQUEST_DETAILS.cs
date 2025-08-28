using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SisConAxs_DM.Models
{
    public partial class AccessRequestDetails
    {
        public AccessRequestDetails()
        {
            this.AddedResourcePeople = new List<ResourcePeople>();
            this.RemovedResourcePeople = new List<ResourcePeople>();
        }

        public int RequestID { get; set; }
        public int RequestDetID { get; set; }
        public int ResourceID { get; set; }
        public Nullable<int> RequestDetStatus { get; set; }
        public int RequestDetStep { get; set; }
        public Nullable<int> RequestDetType { get; set; }
        public string RequestDetStrValue { get; set; }
        public Nullable<int> RequestDetIntValue { get; set; }
        public int ReqDetTemporal { get; set; }
        public Nullable<DateTime> ReqDetValidityFrom { get; set; }
        public Nullable<DateTime> ReqDetValidityUntil { get; set; }
        public int ReqDetSendAtEnd { get; set; }
        public int RequestDetAdditional { get; set; }
        public string RequestDetAdditionalStrValue { get; set; }
        public int RequestDetAdditionalIntValue { get; set; }
        public string RequestDetParam01 { get; set; }
        public string ResourceFullName { get; set; }
        public string RequestDetDisplayValue { get; set; }
        //public string CreateUser { get; set; }
        //public Nullable<System.DateTime> CreateDate { get; set; }
        public string EditUser { get; set; }
        //public Nullable<System.DateTime> EditDate { get; set; }

        public virtual AccessRequests AccessRequest { get; set; }
        public virtual AccessResources AccessResources { get; set; }
        public virtual CommonValues CommonValuesType { get; set; }
        public virtual CommonValues CommonValuesStatus { get; set; }
        public virtual ICollection<ResourcePeople> AddedResourcePeople { get; set; }
        public virtual ICollection<ResourcePeople> RemovedResourcePeople { get; set; }
    }
}