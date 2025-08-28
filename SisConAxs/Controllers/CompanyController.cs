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
    public class CompanyController : ApiController
    {
        public IQueryable<CompanyDTO> GetCompanies(HttpRequestMessage msg)
        {
            //List<int> approvedProfiles = new List<int>();
            //approvedProfiles.Add((int)SITBaseApiController.ROL.SysAdmin);
            SessionInfo session = SessionManager.ValidateSession(msg); //, approvedProfiles);
            var query = new CompanyRepository().GetCompanies();
            //if (query.Count() == 0)
            //{
            //    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
            //    message.Content = new StringContent("No se encontró ningún registro");
            //    throw new HttpResponseException(message);
            //}
            return query;
        }

        [Route("api/company/paginate")]
        [HttpGet]
        public PaginationResponse<CompanyDTO> GetCompaniesPaginate([FromUri] CompanyFilter filter, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            return new CompanyRepository().GetCompaniesPaginate(filter);
        }

        /// <summary>Llamada para obtener el ResourceCategory identificado por el id pasado como parametro.</summary>
        /// <para>GET api/Companies/5 </para>
        /// <exception cref="HttpResponseException">404 Not Found - Si no devolvió ningún item.</exception>
        /// <returns>Un ResourceCategory</returns>
        [Route("api/Company/{id}")]
        [HttpGet]
        public CompanyDTO GetCompanyById(int id, HttpRequestMessage msg)
        {
            //List<int> approvedProfiles = new List<int>();
            //approvedProfiles.Add((int)SITBaseApiController.ROL.SysAdmin);
            SessionInfo session = SessionManager.ValidateSession(msg); //, approvedProfiles);
            var company = new CompanyRepository().GetCompanyById(id);
            return company;
        }

        // POST api/<controller>
        public CompanyDTO Post([FromBody]CompanyDTO dto, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add((int)UserRole.SYSADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            try
            {
                return new CompanyRepository().SaveCompany(dto, session.SessionUser.UserInternalID);
            }
            catch (Exception ex)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                message.Content = new StringContent(ex.Message);
                throw new HttpResponseException(message);
            }
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [Route("api/Company/{id}")]
        public void Delete(int id, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add((int)UserRole.SYSADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            try
            {
                new CompanyRepository().DeleteCompany(id);
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
