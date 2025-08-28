using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs.Integration.Helpers.AzureAuthentication
{
    public interface IAzureAuthentication
    {
        GraphServiceClient GetClient();
    }
}
