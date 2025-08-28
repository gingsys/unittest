using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SisConAxs;
using SisConAxs.Session;

namespace SisConAxs.Controllers
{
    public class HomeController : ApiController
    {
        [Route("api/Home")]
        [HttpGet]
        public string GetHomeConnected (HttpRequestMessage msg)
        {
            string authToken = "";
            IEnumerable<string> values;
            msg.Headers.TryGetValues("X-Auth-Token", out values);
            if (values != null) authToken = values.First();

            if (authToken != null && authToken !="")
            {
                SessionInfo session = SessionManager.GetSessionByToken(authToken);
                if (session != null)
                {                    
                    return "connected";
                }
            }

            HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized); // Unauthorized
            message.Content = new StringContent("No ha iniciado sesión");
            throw new HttpResponseException(message);
        }

        [Route("api/Home/logout")]
        [HttpGet]
        public string LogOutHome (HttpRequestMessage msg)
        {
            string authToken = "";
            IEnumerable<string> values;
            msg.Headers.TryGetValues("X-Auth-Token", out values);
            if (values != null) authToken = values.First();

            if (authToken != null && authToken != "")
            {
                SessionManager.LogOut(authToken);
            }

            HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized); // Unauthorized
            message.Content = new StringContent("No ha iniciado sesión");
            throw new HttpResponseException(message);
        }

        [Route("api/Home/validateUser/{id}")]
        [HttpGet]
        public void validate(string id, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add(Int32.Parse(id));
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
        }

        [Route("api/Home/validateUserProfile")]
        [HttpGet]
        public void validate([FromUri]int[] roles, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            foreach(int rol in roles){
                approvedProfiles.Add(rol);
            }
            SessionManager.ValidateSession(msg, approvedProfiles);
        }

        // Se usa para mantener activa la aplicación
        [Route("api/Home/server-wakeup")]
        [HttpGet]
        public void WakeUp([FromUri]int[] roles, HttpRequestMessage msg)
        {
            //var serverURL = $"{msg.RequestUri.Scheme}://{msg.RequestUri.Authority}";
            ServerWakeUp.RegisterCacheEntry();
        }
    }
}
