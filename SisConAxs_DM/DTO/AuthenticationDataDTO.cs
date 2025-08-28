using SisConAxs_DM.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.DTO
{
    public class AuthenticationDataDTO
    {
        public string grant_type { get; set; }
        public string username { get; set; }
        public string credentials { get; set; }
        public string userAlias
        {
            get
            {
                int position = this.username.Trim().IndexOf("@");
                return this.username.Trim().Substring(0, position);
            }
        }
        public string name { get; set; }

        public string empresa { get; set; }
        public string numeroDocumento { get; set; }
        public string givenName { get; set; }
        public string surname { get; set; }

        public string jobTitle { get; set; }

        public AccessUserDTO ToAccessUserDTO()
        {
            AccessUsersRepository userRepository = new AccessUsersRepository();

            AccessUserDTO user = new AccessUserDTO();
            user.UserFirstName = this.givenName.Trim();
            user.UserLastName = this.surname.Trim();
            user.UserID = 0;
            user.UserInternalID = this.userAlias.Trim();
            user.UserPassword = "";
            //user.UserDocNum = this.numeroDocumento.Trim();
            user.UserEMail = this.username.Trim();

            user.UserRole1 = 0;
            user.UserRole2 = 0;
            user.UserRole3 = 0;
            user.UserRole4 = 0;
            user.UserRole5 = 0;
            user.UserRole6 = 0;
            user.UserRoleSysAdmin = 0;
            user.UserStatus = 1;
            // campos de auditoria
            //newUser.CreateDate = DateTime.Now;
            //newUser.CreateUser = "user.automatico";
            //newUser.EditDate = DateTime.Now;

            // Último acceso LOGIN usuarios:
            user.LastAccessUser = DateTime.Now;

            // Llenamos los datos de la compañia a la cual tendra acceso y asignamos el rol de "Solicitante".
            var companyID = 4;   // Aenza
            var company = new CompanyRepository().GetCompanyByAD(this.empresa.Trim()).FirstOrDefault();
            if (company != null)
            {
                companyID = company.CompanyID;
            }

            ICollection<AccessUserCompanyDTO> userCompanies = new List<AccessUserCompanyDTO>();
            userCompanies.Add(new AccessUserCompanyDTO()
            {
                CompanyID = companyID,
                UserRole1 = 1,
                UserRole2 = 0,
                UserRole3 = 0,
                UserRole4 = 0,
                UserRole5 = 0,
                UserRole6 = 0
            });
            user.UserCompanies = userCompanies;
            return user;
        }
    }
}
