using SisConAxs_DM.DTO;
using System;
using System.Collections.Generic;

namespace SisConAxs_DM.Models
{
    public partial class AccessUsers
    {
        public AccessUsers()
        {
            this.UserCompanies = new List<AccessUserCompanies>();
            //this.USER_OBJECT_ACCESS = new List<USER_OBJECT_ACCESS>();
        }

        public int UserID { get; set; }
        public string UserInternalID { get; set; }
        public string UserLastName { get; set; }
        public string UserFirstName { get; set; }
        public string UserName3 { get; set; }
        public string UserName4 { get; set; }
        public string UserDocNum { get; set; }
        public string UserAddress1 { get; set; }
        public string UserAddress2 { get; set; }
        public string UserPhone1 { get; set; }
        public string UserPhone2 { get; set; }
        public string UserEMail { get; set; }
        public System.Data.Entity.Spatial.DbGeography UserAddressGeoLocation { get; set; }
        public string UserPassword { get; set; }
        public int UserRole1 { get; set; }
        public int UserRole2 { get; set; }
        public int UserRole3 { get; set; }
        public int UserRole4 { get; set; }
        public int UserRole5 { get; set; }
        public int UserRole6 { get; set; }
        public int UserRoleSysAdmin { get; set; }
        public int UserStatus { get; set; }

        //public string CreateUser { get; set; }
        //public System.DateTime CreateDate { get; set; }
        public string EditUser { get; set; }
        //public System.DateTime EditDate { get; set; }

        //public DateTime LastAccessUser { get; set; }
        public Nullable<System.DateTime> LastAccessUser { get; set; }

        public virtual ICollection<AccessUserCompanies> UserCompanies { get; set; }
        //public virtual ICollection<USER_OBJECT_ACCESS> USER_OBJECT_ACCESS { get; set; }
        

        public int GetPermissions()
        {
            int userRole = UserRole1
                            | (UserRole2 * UserRole.APROBADOR)
                            | (UserRole3 * UserRole.ADMIN)
                            | (UserRole4 * UserRole.CREA_PERSONAS)
                            | (UserRole5 * UserRole.DAR_BAJA)
                            | (UserRole6 * UserRole.SEARCH_ALL_REQ)
                            | (UserRoleSysAdmin * (int)UserRole.SYSADMIN);
            return userRole;
        }
    }

    public partial class AccessUserCompanies {
        public AccessUserCompanies()
        {

        }

        public int UserID { get; set; }
        public int CompanyID { get; set; }
        //public string CompanyName { get; set; }
        //public string CompanyDisplay { get; set; }
        //public int CompanyActive { get; set; }
        public int UserRole1 { get; set; }
        public int UserRole2 { get; set; }
        public int UserRole3 { get; set; }
        public int UserRole4 { get; set; }
        public int UserRole5 { get; set; }
        public int UserRole6 { get; set; }
        public int UserRole7 { get; set; }
        //public string CreateUser { get; set; }
        //public System.DateTime CreateDate { get; set; }
        public string EditUser { get; set; }
        //public System.DateTime EditDate { get; set; }

        public virtual AccessUsers User { get; set; }

        public int GetPermissions()
        {
            int userRole = UserRole1
                            | (UserRole2 * UserRole.APROBADOR)
                            | (UserRole3 * UserRole.ADMIN)
                            | (UserRole4 * UserRole.CREA_PERSONAS)
                            | (UserRole5 * UserRole.DAR_BAJA)
                            | (UserRole6 * UserRole.SEARCH_ALL_REQ);
            return userRole;
        }

        public bool ValidatePerm(int profiles)
        {
            return (GetPermissions() & profiles) > 0;
        }
    }
}
