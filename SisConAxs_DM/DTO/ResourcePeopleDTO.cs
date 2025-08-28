using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.DTO
{
    public class ResourcePeopleDTO
    {
        public ResourcePeopleDTO()
        {
        }

        public int PresID { get; set; }
        public int PeopleID { get; set; }
        public int ResourceID { get; set; }
        public Nullable<int> AddedRequestID { get; set; }
        public Nullable<int> AddedRequestNum { get; set; }
        public Nullable<int> AddedRequestDetID { get; set; }
        public Nullable<int> RemovedRequestID { get; set; }
        public Nullable<int> RemovedRequestNum { get; set; }
        public Nullable<int> RemovedRequestDetID { get; set; }
        //public Nullable<int> PresIntValue { get; set; }
        //public string PresStrValue { get; set; }
        //public Nullable<System.DateTime> PresDateValue { get; set; }
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

        public string CategoryName { get; set; }
        public string ResourceFullName { get; set; }
        public string PresDisplayValue { get; set; }

        public int PresCompany { get; set; }
        public string PresCompanyName { get; set; }

        public ICollection<ResourcePeopleLogDTO> ResourcePeopleLog { get; set; } = new List<ResourcePeopleLogDTO>();
    }

    public class ResourcePeopleLogDTO
    {
        public int ResourcePeopleLogID { get; set; }
        public int ResourcePeopleID { get; set; }
        public int Action { get; set; }
        public string ActionDisplay { get; set; }
        public string Source { get; set; }
        public string Description { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string EditUser { get; set; }
        //public Nullable<System.DateTime> EditDate { get; set; }
    }
}
