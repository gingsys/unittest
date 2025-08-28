using SisConAxs.Session;
using SisConAxs_DM.DTO;
using SisConAxs_DM.DTO.Filters;
using SisConAxs_DM.DTO.Response;
using SisConAxs_DM.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SisConAxs.Controllers
{
    public class RequestTemplateController : ApiController
    {
        [Route("api/request-template")]
        public IQueryable<RequestTemplateDTO> GetReqTemplates([FromUri] RequestTemplateFilter filter, HttpRequestMessage msg)
        {
            //List<int> approvedProfiles = new List<int>();
            //approvedProfiles.Add(UserRole.ADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg); //, approvedProfiles);

            filter.ReqTemplateCompany = session.getSessionData().CompanyID;
            return new RequestTemplateRepository(session.getSessionData()).GetReqTemplates(filter);
        }

        //[Route("api/request-template/paginate")]
        //[HttpGet]
        //public PaginationResponse<RequestTemplateDTO> GetRequestTemplatePaginate([FromUri] RequestTemplateFilter filter, HttpRequestMessage msg)
        //{
            //List<int> approvedProfiles = new List<int>();
            //approvedProfiles.Add(UserRole.ADMIN);
            //SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            //return new RequestTemplateRepository(session.getSessionData()).GetCompaniesPaginate(filter);
        //}

        [Route("api/request-template/{id}")]
        [HttpGet]
        public RequestTemplateDTO GetReqTemplateByID(int id, HttpRequestMessage msg)
        {
            //List<int> approvedProfiles = new List<int>();
            //approvedProfiles.Add(UserRole.ADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg); //, approvedProfiles);
            var RequestTemplate = new RequestTemplateRepository(session.getSessionData()).GetReqTemplateByID(id);
            return RequestTemplate;
        }

        [Route("api/request-template")]
        public RequestTemplateDTO Post([FromBody] RequestTemplateDTO dto, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.ADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            try
            {
                return new RequestTemplateRepository(session.getSessionData()).InsertReqTemplate(dto, session.SessionUser.UserInternalID);
            }
            catch (Exception ex)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                message.Content = new StringContent(ex.Message);
                throw new HttpResponseException(message);
            }
        }

        [Route("api/request-template")]
        public RequestTemplateDTO Put([FromBody] RequestTemplateDTO dto, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.ADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            try
            {
                return new RequestTemplateRepository(session.getSessionData()).UpdateReqTemplate(dto, session.SessionUser.UserInternalID);
            }
            catch (Exception ex)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                message.Content = new StringContent(ex.Message);
                throw new HttpResponseException(message);
            }
        }

        [Route("api/request-template/{id}")]
        public void Delete(int id, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.ADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            try
            {
                new RequestTemplateRepository(session.getSessionData()).DeleteReqTemplate(id);
            }
            catch (Exception ex)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                message.Content = new StringContent(ex.Message);
                throw new HttpResponseException(message);
            }
        }
    }
}