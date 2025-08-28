using System;
using System.Collections.Generic;

namespace SisConAxs_DM.Models
{
    public partial class AccessRequests
    {
        public AccessRequests()
        {
            this.AccessRequestDetails = new List<AccessRequestDetails>();
            //this.AddedResourcePeople = new List<AddedResourcePeople>();
        }

        public int RequestID { get; set; }
        public int RequestNumber { get; set; }

        public string RequestBy { get; set; }
        public string RequestByProject { get; set; }
        public string RequestByDepartment { get; set; }
        public string RequestByPosition { get; set; }

        public int RequestTo { get; set; }
        public int RequestToCompany { get; set; }
        public string RequestToProject { get; set; }
        public string RequestToDepartment { get; set; }
        public string RequestToPosition { get; set; }

        //public string RequestToNumDoc { get; set; }
        public int RequestType { get; set; }
        public int RequestPriority { get; set; }
        public int RequestStatus { get; set; }
        public Nullable<int> RequestDepartment { get; set; }
		
        public Nullable<System.DateTime> RequestDate { get; set; }
        public Nullable<System.DateTime> RequestCompletedDate { get; set; }
        public string RequestNote { get; set; }
        //public string CreateUser { get; set; }
        //public Nullable<System.DateTime> CreateDate { get; set; }
        public string EditUser { get; set; }
        //public Nullable<System.DateTime> EditDate { get; set; }

        public virtual CommonValues ReqPriority { get; set; }
        public virtual CommonValues ReqStatus { get; set; }
        public virtual CommonValues ReqType { get; set; }
        public virtual People PeopleRequestTo { get; set; }

        public int RequestCompany { get; set; }

        public virtual ICollection<AccessRequestDetails> AccessRequestDetails { get; set; }
        //public virtual ICollection<AddedResourcePeople> AddedResourcePeople { get; set; }
        public string RequestAttached { get; set; }
        public string AttentionTicket { get; set; }
        public string OracleUser { get; set; }
        public string OracleMenu { get; set; }
    }
}
