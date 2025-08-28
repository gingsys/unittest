using SisConAxs_DM.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SisConAxs.Session
{
    public class AuthenticationProviderLocal : IAuthenticationProvider
    {
        public string GetLoginUsername(string username)
        {
            int position = username.Trim().IndexOf("@");
            return username.Trim().Substring(0, position);
        }

        public bool Validate(string username, string credentials)
        {
            bool response = false;
            #if RELEASE
                throw new Exception("Login no válido, NO puede acceder con un usuario local.");
            #else
            var loginUsername = GetLoginUsername(username);
                response = new AccessUsersRepository().GetAccessUserQuery(u => u.UserInternalID == loginUsername).Count() > 0;
            #endif
            return response;
        }
    }
}