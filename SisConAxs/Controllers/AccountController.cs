using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SisConAxs.Session;
using SisConAxs_DM.DTO;
using SisConAxs_DM.Repository;

namespace SisConAxs.Controllers
{
    public class AccountController : ApiController
    {
        [Route("api/Account/")]
        [HttpPost]
        public SessionData Post([FromBody]AuthenticationDataDTO data)
        {
            try
            {
                SessionInfo session = SessionManager.LogIn(data);
                HttpResponseMessage message;
                if (session != null)
                {
                    //if (session.getSessionData().UserStatus == 0)
                    //{
                    //    message = new HttpResponseMessage(HttpStatusCode.Unauthorized);  // Unauthorized
                    //    message.Content = new StringContent("Usuario inactivo");
                    //    throw new HttpResponseException(message);
                    //}
                    //else
                    //{
                        return session.getSessionData();
                    //}
                }
                message = new HttpResponseMessage(HttpStatusCode.Unauthorized);         // Unauthorized
                message.Content = new StringContent("Usuario o password inválido");
                throw new HttpResponseException(message);
            }
            catch (Exception ex)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                
                var company = new CompanyRepository().GetCompanyByAD(data.empresa.Trim()).FirstOrDefault();
                if (company == null)
                {
                    message.Content = new StringContent("El nombre de la empresa asociada a su usuario no se encuentra registrada en ICARUS, comuníquese con Mesa de ayuda");
                    throw new HttpResponseException(message);
                }
                 // Unauthorized
                message.Content = new StringContent("Usuario o password inválido");
                throw new HttpResponseException(message);
            }
        }

        [Route("api/Account/changecompany/{companyID}")]
        [HttpPost]
        public SessionData ChangeCompany(int companyID, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            session.SetCompany(companyID);
            return session.getSessionData();
        }
    }
}
