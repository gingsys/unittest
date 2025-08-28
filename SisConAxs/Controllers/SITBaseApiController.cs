using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SisConAxs.Controllers
{
    public abstract class SITBaseApiController : System.Web.Http.ApiController
    {
        public HttpResponseMessage GetHttpMessageNoRecordFound()
        {
            HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
            message.Content = new StringContent("No se encontró ningún registro");
            return message;
        }
    }
}
