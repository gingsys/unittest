using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using SisConAxs.Integration.DTO;
using SisConAxs.Integration.Helpers.AzureAuthentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs.Integration.IntegrationUser
{
    public class IntegrationUserAzure
    {
        static public List<AzureUserDTO> GetOrganizationUsers(string filter = null)
        {
            List<AzureUserDTO> users = new List<AzureUserDTO>();
            try
            {
                IAzureAuthentication authenticationHelper = AuthFactory.Get(AzureAuthenticationType.ClientSecret);
                GraphServiceClient client = authenticationHelper.GetClient();
                var result = client.Users.GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Top = 999;
                    requestConfiguration.QueryParameters.Select = new string[] { "onPremisesSamAccountName", "userPrincipalName", "givenName", "surName", "postalCode", "companyName", "mail", "createdDateTime" };
                    //if (!String.IsNullOrWhiteSpace(filter))
                    //    requestConfiguration.QueryParameters.Filter = filter; //"startsWith(companyName,'AENZA SERVICIOS CORPORATIVOS S.A.C.')";
                    requestConfiguration.QueryParameters.Orderby = new string[] { "userPrincipalName" };
                    requestConfiguration.QueryParameters.Count = true;
                    requestConfiguration.Headers.Add("ConsistencyLevel", "eventual");
                }).Result;

                // https://learn.microsoft.com/en-us/graph/sdks/paging?tabs=csharp
                var pageIterator = PageIterator<User, UserCollectionResponse>.CreatePageIterator(
                    client,
                    result,
                    userAD =>
                    {
                        var user = new AzureUserDTO()
                        {
                            Username = userAD.OnPremisesSamAccountName?.Trim(),
                            Firtsname = userAD.GivenName?.Trim(),
                            Lastsname = userAD.Surname?.Trim(),
                            DocNumber = userAD.PostalCode?.Trim(),
                            CompanyName = userAD.CompanyName?.Trim(),
                            Email = userAD.Mail?.Trim(),
                            CreatedDateTime = userAD.CreatedDateTime
                        };
                        users.Add(user);
                        return true;
                    });
                Task.WaitAll(pageIterator.IterateAsync());

                users = users.FindAll(user =>
                {
                    return !String.IsNullOrWhiteSpace(user.Username) && user.DocNumber != "GENERICA" && user.DocNumber != "EXTERNO" && user.DocNumber != "SERVICIO" && user.DocNumber != "BUZON" && user.DocNumber != "SALA" && user.DocNumber != "SISTEMA";
                });
                users = users
                             //.OrderBy(u => u.DocNumber).ThenByDescending(u => u.CreatedDateTime)
                             //.GroupBy(u => u.DocNumber)
                             //.Select(u => u.First(u2 => u2.CreatedDateTime == u.Max(u1 => u1.CreatedDateTime)))
                             //.ToList()
                             .OrderBy(u => u.DocNumber).ThenByDescending(u => u.CreatedDateTime)
                             .GroupBy(u => u.DocNumber)
                             .Select(u => u.First())
                             .OrderBy(u => u.Username)
                             .ToList()
                             ;

                //users.ForEach(user =>
                //{
                //    Console.WriteLine($"user: {user.OnPremisesSamAccountName} | DNI: {user.PostalCode} | mail: {user.Mail} | company: {user.CompanyName}");
                //});
                //Console.WriteLine($"count: {users.Count}");

                return users;
            }
            catch (ODataError odataError)
            {
                throw new Exception($"Error en los datos de la consulta a Graph API > Code: {odataError.Error.Code}, Message: {odataError.Error.Message}", odataError);
            }
            catch (ServiceException ex)
            {
                throw new Exception($"Error accedientdo a Graph API > Message: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al realizar la consulta > Message: {ex.Message}", ex);
            }
        }
    }
}
