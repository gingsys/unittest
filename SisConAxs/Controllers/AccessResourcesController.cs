using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SisConAxs.Session;
using SisConAxs_DM.Repository;
using SisConAxs_DM.DTO;

namespace SisConAxs.Controllers
{
    public class AccessResourcesController : ApiController
    {
        /// <summary>Llamada para obtener la lista de AccessResources.</summary>
        /// <para>GET api/AccessResources </para>
        /// <returns>Lista de AccessResource</returns>
        public IQueryable<AccessResourcesDTO> GetAccessResources(HttpRequestMessage msg, [FromUri]AccessResourceFilter filter)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            var query = new AccessResourcesRepository(session.getSessionData()).GetAccessResources(filter);
            //if (query.Count() == 0)
            //{
            //    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
            //    message.Content = new StringContent("No se encontró ningún registro");
            //    throw new HttpResponseException(message);
            //}
            return query;
        }

        /// <summary>Llamada para obtener el AccessResource identificado por el id pasado como parametro.</summary>
        /// <para>GET api/AccessResources/5 </para>
        /// <exception cref="HttpResponseException">404 Not Found - Si no devolvió ningún item.</exception>
        /// <returns>Un AccessResource</returns>
        [Route("api/AccessResources/{id}")]
        [HttpGet]
        public IQueryable<AccessResourcesDTO> GetAccessResourceById(int id, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            var query = new AccessResourcesRepository(session.getSessionData()).GetAccessResourceById(id);
            //if (query.Count() == 0)
            //{
            //    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
            //    message.Content = new StringContent("No se encontró el registro");
            //    throw new HttpResponseException(message);
            //}
            return query;
        }

        // GET api/<controller>/5/values
        [Route("api/AccessResources/{id}/values")]
        [HttpGet]
        public string GetAccessResources(int id, HttpRequestMessage msg)
        {
            return "Un valor";
        }

        // POST api/<controller>
        public AccessResourcesDTO Post([FromBody]AccessResourcesDTO dto, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.ADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            try
            {
                return new AccessResourcesRepository(session.getSessionData()).SaveAccessResource(dto, session.SessionUser.UserInternalID);
            }
            catch (Exception ex)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                message.Content = new StringContent(ex.Message);
                throw new HttpResponseException(message);
            }
        }

        [Route("api/AccessResources/multiple")]
        public void SaveMultiple([FromBody]AccessResourcesSaveMultiple dto, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.ADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            try
            {
                new AccessResourcesRepository(session.getSessionData()).SaveMultiple(dto, session.SessionUser.UserInternalID);
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
        [Route("api/AccessResources/{id}")]
        public void Delete(int id, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.ADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            try
            {
                new AccessResourcesRepository(session.getSessionData()).DeleteAccessResource(id);
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