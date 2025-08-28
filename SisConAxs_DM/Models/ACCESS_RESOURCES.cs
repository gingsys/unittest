using System;
using System.Collections.Generic;

namespace SisConAxs_DM.Models
{
    public partial class AccessResources
    {
        public AccessResources()
        {
            this.ResourceParameters = new List<AccessResourceParameter>();

            this.AccessRequestDetails = new List<AccessRequestDetails>();
            this.ResourcePeople = new List<ResourcePeople>();
            //this.AccessResourcesChildren = new List<AccessResources>();
            //this.USER_OBJECT_ACCESS = new List<USER_OBJECT_ACCESS>();
        }

        public int ResourceID { get; set; }
        public int ResourceCategory { get; set; }
        public string ResourceName { get; set; }
        public string ResourceFullName { get; set; }
        public string ResourceDescription { get; set; }
        public Nullable<int> ResourceParent { get; set; }
        public Nullable<int> ResourceDepartment { get; set; }
        public int ResourceTemporal { get; set; }
        public int ResourceSendAtEnd { get; set; }
        public string ResourceParam1 { get; set; }
        public string ResourceParam2 { get; set; }
        public string ResourceParam3 { get; set; }
        public string ResourceParam4 { get; set; }
        public string ResourceParam5 { get; set; }
        public string ResourceParam6 { get; set; }
        public string ResourceParam7 { get; set; }
        public string ResourceParam8 { get; set; }
        public string ResourceParam9 { get; set; }
        public string ResourceParam10 { get; set; }
        public int ResourceAccessType { get; set; }

        public int ResourceFlag { get; set; }

        //public string CreateUser { get; set; }
        //public Nullable<System.DateTime> CreateDate { get; set; }
        public string EditUser { get; set; }
        //public Nullable<System.DateTime> EditDate { get; set; }
        public Nullable<int> ResourceWorkflow { get; set; }
        public int ResourceActive { get; set; }
        public int ResourceOnlyAssignable { get; set; }

        public Nullable<int> ResourceRequired { get; set; }

        public int ResourceCompany { get; set; }

        public virtual ICollection<AccessResourceParameter> ResourceParameters { get; set; }


        public virtual ICollection<RequestTemplateDetail> RequestTemplateDetail { get; set; }
        public virtual ICollection<AccessRequestDetails> AccessRequestDetails { get; set; }
        public virtual ICollection<ResourcePeople> ResourcePeople { get; set; }

        //public virtual ICollection<AccessResources> AccessResourcesChildren { get; set; }      // AccessResources1
        //public virtual AccessResources AccessResourcesParent { get; set; }                     // AccessResources2
        public virtual AccessTypes AccessTypes { get; set; }
        public virtual ResourceCategories ResourceCategories { get; set; }
        public virtual Workflow Workflows { get; set; }
        //public virtual ICollection<USER_OBJECT_ACCESS> USER_OBJECT_ACCESS { get; set; }
    }
}
