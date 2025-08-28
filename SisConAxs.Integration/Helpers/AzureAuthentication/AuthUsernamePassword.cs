using Azure.Identity;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs.Integration.Helpers.AzureAuthentication
{
    internal class AuthUsernamePassword : IAzureAuthentication
    {
        public GraphServiceClient GetClient()
        {
            var options = new UsernamePasswordCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
            };
            // https://learn.microsoft.com/dotnet/api/azure.identity.usernamepasswordcredential
            var authCredential = new UsernamePasswordCredential(Settings.Username, Settings.Password, Settings.TenantId, Settings.ClientId, options);
            var graphClient = new GraphServiceClient(authCredential, Settings.Scopes);
            return graphClient;
        }
    }
}