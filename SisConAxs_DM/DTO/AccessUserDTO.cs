using System;
using System.Collections.Generic;

namespace SisConAxs_DM.DTO
{
    public static class UserRole
    {
        public const int SOLICITANTE = 1;
        public const int APROBADOR = 2;
        public const int ADMIN = 4;
        public const int CREA_PERSONAS = 8;
        public const int DAR_BAJA = 16;
        public const int SEARCH_ALL_REQ = 32;
        public const int REPORTES = 64;
        public const int SYSADMIN = 1024;
    }

    public partial class AccessUserDTO
    {
        public int UserID { get; set; }
        public string UserInternalID { get; set; }
        public string UserCode { get; set; }        
        public string UserLastName { get; set; }
        public string UserFirstName { get; set; }
        public string UserName3 { get; set; }
        public string UserName4 { get; set; }
        public int UserDocType { get; set; }
        public string UserDocTypeDisplay { get; set; }
        public string UserDocNum { get; set; }
        public string UserCompany { get; set; }
        public string UserAddress1 { get; set; }
        public string UserAddress2 { get; set; }
        public string UserPhone1 { get; set; }
        public string UserPhone2 { get; set; }
        public string UserEMail { get; set; }
        public string UserPassword { get; set; }
        public int UserRole1 { get; set; }
        public int UserRole2 { get; set; }
        public int UserRole3 { get; set; }
        public int UserRole4 { get; set; }
        public int UserRole5 { get; set; }
        public int UserRole6 { get; set; }
        public int UserRoleSysAdmin { get; set; }
        //public string CreateUser { get; set; }
        //public System.DateTime CreateDate { get; set; }
        public string EditUser { get; set; }
        //public System.DateTime EditDate { get; set; }

        public int UserStatus { get; set; }
        public string UserStatusDesc { get; set; }
        //public System.Data.Entity.Spatial.DbGeography UserAddressGeoLocation { get; set; }}}

        //public DateTime LastAccessUser { get; set; }
        public Nullable<System.DateTime> LastAccessUser { get; set; }
        public ICollection<AccessUserCompanyDTO> UserCompanies { get; set; } = new List<AccessUserCompanyDTO>();
        public string UserPrincipalName { get; set; }

        public string GetADFilter()
        {
            string filter = "";
            if (!String.IsNullOrWhiteSpace(UserInternalID))
                filter += String.Format("(samAccountName=*{0}*)", UserInternalID);
            if (!String.IsNullOrWhiteSpace(UserCode))                 // <----- @NOTE: for CONCAR
                filter += String.Format("(postalCode=*{0}*)", UserCode);
            if (!String.IsNullOrWhiteSpace(UserLastName))
                filter += String.Format("(sn=*{0}*)", UserLastName);
            if (!String.IsNullOrWhiteSpace(UserFirstName))
                filter += String.Format("(givenName=*{0}*)", UserFirstName);
            if (!String.IsNullOrWhiteSpace(UserEMail))
                filter += String.Format("(mail=*{0}*)", UserEMail);
            if (!String.IsNullOrWhiteSpace(UserDocNum))                 // <----- @NOTE: for CONCAR
                filter += String.Format("(postalCode=*{0}*)", UserDocNum);
            if (!String.IsNullOrWhiteSpace(UserPhone1))
                filter += String.Format("(mobile=*{0}*)", UserPhone1);
            if (!String.IsNullOrWhiteSpace(UserCompany))
                filter += String.Format("(company=*{0}*)", UserCompany);
            return filter;
        }
    }

    public partial class AccessUserCompanyDTO
    {
        public AccessUserCompanyDTO()
        {

        }

        public int UserID { get; set; }
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyDisplay { get; set; }
        public int CompanyActive { get; set; }
        public int UserRole1 { get; set; }
        public int UserRole2 { get; set; }
        public int UserRole3 { get; set; }
        public int UserRole4 { get; set; }
        public int UserRole5 { get; set; }
        public int UserRole6 { get; set; }
        public int UserRole7 { get; set; }

        public int GetPermissions()
        {
            return UserRole1 +
                UserRole2 * 2 +
                UserRole3 * 4 +
                UserRole4 * 8 +
                UserRole5 * 16 +
                UserRole6 * 32 +
                UserRole7 * 64;
        }
    }
}
