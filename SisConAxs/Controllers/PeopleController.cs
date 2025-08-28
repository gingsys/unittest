using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using SisConAxs.Session;
using SisConAxs_DM.Repository;
using SisConAxs_DM.DTO;
using static SisConAxs_DM.Repository.PeopleRepository;
using SisConAxs_DM.DTO.Filters;
using SisConAxs_DM.DTO.Response;

namespace SisConAxs.Controllers
{
    public class PeopleController : ApiController
    {
        public IQueryable<PeopleDTO> GetPeople([FromUri] PeopleFilter filter, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            var query = new PeopleRepository(session.getSessionData()).GetPeople(filter);

            //if (query.Count() == 0)
            //{
            //    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
            //    message.Content = new StringContent("No se encontró ningún registro");
            //    throw new HttpResponseException(message);
            //}
            return query;
        }

        [Route("api/PeoplePaginate")]
        [HttpGet]
        public PaginationResponse<PeopleDTO> GetPeoplePaginate([FromUri] SisConAxs_DM.DTO.Filters.PeopleFilter filter, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            return new PeopleRepository(session.getSessionData()).GetPeoplePaginate(filter);
        }

        [Route("api/People/{id}")]
        [HttpGet]
        public IQueryable<PeopleDTO> GetPeopleById(int id, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            var query = new PeopleRepository(session.getSessionData()).GetPeopleById(id);
            //if (query.Count() == 0)
            //{
            //    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
            //    message.Content = new StringContent("No se encontró ningún registro");
            //    throw new HttpResponseException(message);
            //}
            return query;

        }

        [Route("api/People/hasAccess")]
        [HttpGet]
        public IQueryable<PeopleDTO> GetPeopleWithActives([FromUri] PeopleFilter filter, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            var query = new PeopleRepository(session.getSessionData()).GetPeoplewithActives(filter);
            //if (query.Count() == 0)
            //{
            //    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
            //    message.Content = new StringContent("No se encontró ningún registro");
            //    throw new HttpResponseException(message);
            //}
            return query;
        }

        [Route("api/People/loadFromExcel")]
        [HttpGet]
        public IQueryable<PeopleDTO> GetPeopleFromExcel(HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            try
            {
                var query = new PeopleRepository(session.getSessionData()).GetPeopleFromExcel();
                //if (query.Count() == 0)
                //{
                //    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                //    message.Content = new StringContent("No se encontró ningún registro");
                //    throw new HttpResponseException(message);
                //}
                return query;
            }
            catch (Exception ex)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                message.Content = new StringContent(ex.Message);
                throw new HttpResponseException(message);
            }
        }

        // POST api/<controller>
        public PeopleDTO Post([FromBody] PeopleDTO people, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            var validTypes = new List<int?> { PeopleDTO.CLAS_TYPE_CLIENTE, PeopleDTO.CLAS_TYPE_PROVEEDOR, PeopleDTO.CLAS_TYPE_TERCERO, PeopleDTO.CLAS_TYPE_COLABORADOR_EN_PROCESO };
            
            approvedProfiles.Add(UserRole.SYSADMIN);
            approvedProfiles.Add(UserRole.ADMIN);
            if (validTypes.Contains(people.PeopleTypeClasificacion))
            {
                approvedProfiles.Add(UserRole.CREA_PERSONAS);
            }
            SessionInfo session = SessionManager.ValidateSession(msg,approvedProfiles);
            try
            {
                return new PeopleRepository(session.getSessionData()).SavePeople(people);
                //return new PeopleRepository(session.getSessionData()).SaveAccessPersona(people, "user.automatico");   // esto es para la creacion automatica de la persona
            }
            catch (Exception ex)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                message.Content = new StringContent(ex.Message);
                throw new HttpResponseException(message);
            }
        }

        [Route("api/People/massiveLoad")]
        [HttpPost]
        public String[] MassiveLoad(HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.ADMIN);
            approvedProfiles.Add(UserRole.CREA_PERSONAS);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            try
            {
                return new PeopleRepository(session.getSessionData()).SavePeopleFromExcel();
            }
            catch (Exception ex)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                message.Content = new StringContent(ex.Message);
                throw new HttpResponseException(message);
            }
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        [Route("api/People/{id}")]
        public PeopleDTO Delete(int id, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.ADMIN);
            approvedProfiles.Add(UserRole.DAR_BAJA);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            try
            {
                return new PeopleRepository(session.getSessionData()).DeletePeople(id);
            }
            catch (Exception ex)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                message.Content = new StringContent(ex.Message);
                throw new HttpResponseException(message);
            }
        }

        [Route("api/People/approvers/pendings")]
        [HttpGet]
        public IQueryable<object> GetPeopleApproversRequestPendings(HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            var query = new PeopleRepository(session.getSessionData()).GetPeopleApproversRequestPendings();
            //if (query.Count() == 0)
            //{
            //    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
            //    message.Content = new StringContent("No se encontró ningún registro");
            //    throw new HttpResponseException(message);
            //}
            return query;

        }
    }
}