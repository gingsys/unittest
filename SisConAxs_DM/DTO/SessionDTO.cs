using SisConAxs_DM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.DTO
{
    public class SessionData
    {
        public string sessionToken = "";
        public string sessionUser = "";
        public string sessionUserFullName = "";
        public int UserStatus = 0;

        public int CompanyID = 0;
        public string CompanyDisplay;
        public string CompanyName;
        public int UserRole1 = 0;
        public int UserRole2 = 0;
        public int UserRole3 = 0;
        public int UserRole4 = 0;
        public int UserRole5 = 0;
        public int UserRole6 = 0;
        public int UserRoleSysAdmin = 0;
        public int UserRole7 = 0;

        public AccessUserDTO User;

        public bool HavePermission(int roles)
        {
            return (GetPermissions() & roles) > 0;
        }
        public bool HavePermission(int[] roles)
        {
            var acumRol = 0;
            foreach (var item in roles)
                acumRol |= item;
            return (GetPermissions() & acumRol) > 0;
        }

        public int GetPermissions()
        {
            int userRole = UserRole1
                            | (UserRole2 * UserRole.APROBADOR)
                            | (UserRole3 * UserRole.ADMIN)
                            | (UserRole4 * UserRole.CREA_PERSONAS)
                            | (UserRole5 * UserRole.DAR_BAJA)
                            | (UserRole6 * UserRole.SEARCH_ALL_REQ)
                            | (UserRoleSysAdmin * UserRole.SYSADMIN)
                            | (UserRole7 * UserRole.REPORTES);
            return userRole;
        }
    }
}
