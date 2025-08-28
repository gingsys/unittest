using System;
using System.Collections.Generic;

namespace SisConAxs_DM.Models
{
    public partial class CommonValues
    {
        public CommonValues()
        {
            this.AccessRequestDetailsType = new List<AccessRequestDetails>();
            this.AccessRequestDetailsStatus = new List<AccessRequestDetails>();

            this.AccessRequests = new List<AccessRequests>();
            this.AccessRequests1 = new List<AccessRequests>();
            this.AccessRequests2 = new List<AccessRequests>();
            
            this.People = new List<People>();
            this.People1 = new List<People>();
            this.People2 = new List<People>();
            this.People3 = new List<People>();
            this.PeopleEmployeeType = new List<People>();

            this.WorkflowApproveHierarchy = new List<WorkflowApproveHierarchy>();
            this.WorkflowHierarchyMembers = new List<WorkflowHierarchyMembers>();
            this.WorkflowHierarchyMembers1 = new List<WorkflowHierarchyMembers>();
            this.WorkflowItems = new List<WorkflowItems>();

            this.AccessResourceParameters = new List<AccessResourceParameter>();
            this.CompanyParameters = new List<CompanyParameter>();
        }

        public int CommonValueID { get; set; }
        public int CommonValueSetID { get; set; }
        public string CommonValueName { get; set; }
        public string CommonValueDisplay { get; set; }
        public string CommonValueDesc { get; set; }
        public Nullable<int> CommonValueDefault { get; set; }
        //public string CreateUser { get; set; }
        //public Nullable<System.DateTime> CreateDate { get; set; }
        public string EditUser { get; set; }
        //public Nullable<System.DateTime> EditDate { get; set; }

        public Nullable<int> CommonValueCompany { get; set; }


        public virtual ICollection<RequestTemplate> ReqTemplateType { get; set; }
        public virtual ICollection<RequestTemplate> ReqTemplateEmployeeType { get; set; }

        public virtual ICollection<AccessRequestDetails> AccessRequestDetailsType { get; set; }
        public virtual ICollection<AccessRequestDetails> AccessRequestDetailsStatus { get; set; }
        public virtual ICollection<AccessRequests> AccessRequests { get; set; }
        public virtual ICollection<AccessRequests> AccessRequests1 { get; set; }
        public virtual ICollection<AccessRequests> AccessRequests2 { get; set; }
        
        public virtual ICollection<People> People { get; set; }              // People Department
        public virtual ICollection<People> People1 { get; set; }             // People DocType
        public virtual ICollection<People> People2 { get; set; }             // People Position
        public virtual ICollection<People> People3 { get; set; }             // People TypeClasificacion
        public virtual ICollection<People> PeopleEmployeeType { get; set; }  // People EmployeeType


        public virtual ICollection<WorkflowApproveHierarchy> WorkflowApproveHierarchy { get; set; }   // WorkflowApproveHierarchy Department
        public virtual ICollection<WorkflowHierarchyMembers> WorkflowHierarchyMembers { get; set; }
        public virtual ICollection<WorkflowHierarchyMembers> WorkflowHierarchyMembers1 { get; set; }
        public virtual ICollection<WorkflowItems> WorkflowItems { get; set; }

        public virtual ICollection<AccessResourceParameter> AccessResourceParameters { get; set; }
        public virtual ICollection<CompanyParameter> CompanyParameters { get; set; }

        public virtual CommonValueSets CommonValueSets { get; set; }
    }
}
