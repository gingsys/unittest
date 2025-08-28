using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SisConAxs.Session;
using SisConAxs_DM.Repository;
using SisConAxs_DM.DTO;
using Microsoft.Reporting.WebForms;
using System.IO;
using System.Configuration;
using System.Net.Http.Headers;


namespace SisConAxs.Controllers
{
    public class ReportsController : ApiController
    {
        public string GetReports(HttpRequestMessage msg)
        {           
            return "OK";
        }

        


    }
}