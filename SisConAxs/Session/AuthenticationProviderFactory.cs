using SisConAxs.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SisConAxs.Session
{
    public class AuthenticationProviderFactory
    {
        public const string PROVIDER_LOCAL = "local";
        public const string PROVIDER_AZUREAD = "azureAD";

        static public IAuthenticationProvider Get(string name)
        {
            if (name == PROVIDER_LOCAL)
            {
                return new AuthenticationProviderLocal();
            }
            if (name == PROVIDER_AZUREAD)
            {
                return new AuthenticationProviderAzureAD();
            }
            return null;
        }
    }
}