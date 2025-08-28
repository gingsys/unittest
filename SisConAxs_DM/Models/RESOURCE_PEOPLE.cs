using System;
using System.Collections.Generic;

namespace SisConAxs_DM.Models
{
    public partial class ResourcePeople
    {
        public ResourcePeople()
        {
            this.ResourcePeopleLog = new List<ResourcePeopleLog>();
        }

        public int PresID { get; set; }
        public int PeopleID { get; set; }
        public int ResourceID { get; set; }
        public Nullable<int> AddedRequestID { get; set; }
        public Nullable<int> AddedRequestDetID { get; set; }
        public Nullable<int> RemovedRequestID { get; set; }
        public Nullable<int> RemovedRequestDetID { get; set; }
        public Nullable<int> PresIntValue { get; set; }
        public string PresStrValue { get; set; }
        public Nullable<System.DateTime> PresDateValue { get; set; }
        public Nullable<System.DateTime> PresDateStart { get; set; }
        public Nullable<System.DateTime> PresDateEnd { get; set; }
        public int PresActive { get; set; }
        public int PresTemporal { get; set; }
        public Nullable<System.DateTime> PresValidityFrom { get; set; }
        public Nullable<System.DateTime> PresValidityUntil { get; set; }
        public Nullable<int> PeopleDepartment { get; set; }

        public int PresAdditional { get; set; }
        public string PresAdditionalStrValue { get; set; }
        public int PresAdditionalIntValue { get; set; }

        public string ResourceFullName { get; set; }
        public string PresDisplayValue { get; set; }

        public int PresCompany { get; set; }

        public string PresAttribute1 { get; set; }
        public string PresAttribute2 { get; set; }
        public string PresAttribute3 { get; set; }
        public string PresAttribute4 { get; set; }
        public string PresAttribute5 { get; set; }
        public string PresAttribute6 { get; set; }
        public string PresAttribute7 { get; set; }
        public string PresAttribute8 { get; set; }
        public string PresAttribute9 { get; set; }
        public string PresAttribute10 { get; set; }

        //public string CreateUser { get; set; }
        //public Nullable<System.DateTime> CreateDate { get; set; }
        public string EditUser { get; set; }
        //public Nullable<System.DateTime> EditDate { get; set; }

        public virtual AccessResources AccessResources { get; set; }
        public virtual People People { get; set; }
        public virtual AccessRequestDetails AddedAccessRequestDetails { get; set; }
        public virtual AccessRequestDetails RemovedAccessRequestDetails { get; set; }
        //public virtual AccessRequests AccessRequest { get; set; }
        public virtual ICollection<ResourcePeopleLog> ResourcePeopleLog { get; set; }
    }
}
