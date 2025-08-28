using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs.Integration.Helpers.AzureAuthentication
{
    internal enum AzureAuthenticationType
    {
        ClientSecret,
        UsernamePassword
    }

    internal class AuthFactory
    {
        public static IAzureAuthentication Get(AzureAuthenticationType type)
        {
            if(type == AzureAuthenticationType.ClientSecret)
            {
                return new AuthClientSecret();
            }
            if(type == AzureAuthenticationType.UsernamePassword)
            {
                return new AuthUsernamePassword();
            }
            throw new NotImplementedException();
        }
    }
}
