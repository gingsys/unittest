using Azure.Identity;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs.Integration.Helpers.AzureAuthentication
{
    internal class AuthClientSecret : IAzureAuthentication
    {
        public GraphServiceClient GetClient()
        {
            var options = new ClientSecretCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
            };
            // https://learn.microsoft.com/dotnet/api/azure.identity.clientsecretcredential
            var clientSecretCredential = new ClientSecretCredential(Settings.TenantId, Settings.ClientId, Settings.ClientSecret, options);
            var graphClient = new GraphServiceClient(clientSecretCredential, Settings.Scopes);
            return graphClient;
        }
    }
}