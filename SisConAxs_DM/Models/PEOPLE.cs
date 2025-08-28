using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SisConAxs_DM.Models
{
    public partial class People
    {
        public People()
        {
            //this.DepartmentPositions = new List<DepartmentPositions>();
            //this.PeopleDepartments  = new List<PeopleDepartments>();
            this.AccessRequests = new List<AccessRequests>();
            this.ResourcePeople = new List<ResourcePeople>();
        }

        public int PeopleID { get; set; }
        public int PeopleOrgID { get; set; }
        public string PeopleInternalID { get; set; }
        public string PeopleLastName { get; set; }
        public string PeopleFirstName { get; set; }
        public string PeopleLastName2{ get; set; }
        public string PeopleFirstName2 { get; set; }
        public Nullable<int> PeopleDocType { get; set; }
        public Nullable<int> PeopleTypeClasificacion { get; set; }
        public Nullable<int> PeopleEmployeeType { get; set; }
        public string PeopleDocNum { get; set; }
        public string PeopleAddress1 { get; set; }
        public string PeopleAddress2 { get; set; }
        public string PeoplePhone1 { get; set; }
        public string PeoplePhone2 { get; set; }
        public string PeopleEmail { get; set; }
        public Nullable<DateTime> PeopleBirthday { get; set; }
        public string PeopleGender { get; set; }
        public System.Data.Entity.Spatial.DbGeography PeopleAddressGeolocation { get; set; }
        public byte[] PeopleImage { get; set; }
        public Nullable<int> PeopleDepartment { get; set; }
        public Nullable<int> PeoplePosition { get; set; }
        public string PeopleAttribute1 { get; set; }
        public string PeopleAttribute2 { get; set; }
        public int? PeopleProject { get; set; }
        public string PeopleAttribute3 { get; set; }
        public string PeopleAttribute4 { get; set; }
        public string PeopleAttribute5 { get; set; }
        public string PeopleAttribute6 { get; set; }
        public string PeopleAttribute7 { get; set; }
        public string PeopleAttribute8 { get; set; }
        public string PeopleAttribute9 { get; set; }
        public string PeopleAttribute10 { get; set; }
        public int PeopleStatus { get; set; }
        //public string CreateUser { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public Nullable<System.DateTime> CreateDate { get; set; }

        public string EditUser { get; set; }
        //public Nullable<System.DateTime> EditDate { get; set; }
        public string UserID { get; set; }

        public int PeopleCompany { get; set; }
        public bool PeopleIsSourceSAP { get; set; }
        public DateTime? PeopleStartDate { get; set; }

        //public virtual ICollection<DepartmentPositions> DepartmentPositions { get; set; }
        //public virtual ICollection<PeopleDepartments> PeopleDepartments { get; set; }
        public virtual ICollection<AccessRequests> AccessRequests { get; set; }
        public virtual ICollection<ResourcePeople> ResourcePeople { get; set; }

        public virtual Company Company { get; set; }
        public virtual CommonValues DocType { get; set; }

        public virtual CommonValues TypeClasificacion { get; set; }
        public virtual CommonValues EmployeeType { get; set; }
        public virtual CommonValues Department { get; set; }
        public virtual CommonValues Position { get; set; }


        public string GetFullName()
        {
            return  (this.PeopleLastName + " " +
                     this.PeopleLastName2 + ", " +
                     this.PeopleFirstName + " " +
                     this.PeopleFirstName2).Trim();
        }
    }
}
