using SisConAxs_DM.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Utils
{
    public class ConstantsDM
    {
        static public string USER_SYSTEM_ICARUS = "icarus";
        static public SessionData USER_SYSTEM_ICARUS_SESSIONDATA = new SessionData()
        {
            User = new AccessUserDTO() { UserFirstName = "ICARUS", UserLastName = "ICARUS" },
            sessionUser = USER_SYSTEM_ICARUS,
            //UserRole3 = 1,        // ADMIN
            UserRoleSysAdmin = 1  // SYSADMIN
        };
    }
}
