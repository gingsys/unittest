using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SisConAxs.Session;
using SisConAxs_DM.Repository;
using SisConAxs_DM.DTO;
using SisConAxs_DM.DTO.Filters;

namespace SisConAxs.Controllers
{
    public class AccessTypesController : ApiController
    {
        /// <summary>Llamada para obtener la lista de AccesTypes.</summary>
        /// <para>GET api/AccessTypes </para>
        /// <returns>Lista de AccesType</returns>
        ///[Route("api/AccessTypes/")]
        ///[HttpGet]
        [Route("api/AccessTypes")]
        public IQueryable<AccessTypesDTO> GetAccessTypes([FromUri] AccessTypeFilter filter, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);

            var query = new AccessTypesRepository(session.getSessionData()).GetAccessTypes(filter);
            //if (query.Count() == 0)
            //{
            //    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
            //    message.Content = new StringContent("No se encontró ningún registro");
            //    throw new HttpResponseException(message);
            //}
            return query;
        }

        /// <summary>Llamada para obtener el AccesType identificado por el id pasado como parametro.</summary>
        /// <para>GET api/AccessTypes/5 </para>
        /// <exception cref="HttpResponseException">404 Not Found - Si no devolvió ningún item.</exception>
        /// <returns>Un AccesType</returns>
        [Route("api/AccessTypes/{id}")]
        [HttpGet]
        public IQueryable<AccessTypesDTO> GetAccessTypeById(int id, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            var query = new AccessTypesRepository(session.getSessionData()).GetAccessTypeById(id);
            //if (query.Count() == 0) {
            //    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
            //    message.Content = new StringContent("No se encontró el registro");
            //    throw new HttpResponseException(message);
            //}
            return query;
        }

        // GET api/<controller>/5/values
        [Route("api/AccessTypes/{id}/values")]
        [HttpGet]
        public string GetTypeAccessValues(int id)
        {
            return "Un valor";
        }

        // POST api/<controller>
        [Route("api/AccessTypes")]
        public AccessTypesDTO Post([FromBody]AccessTypesDTO dto, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.ADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            try
            {
                return new AccessTypesRepository(session.getSessionData()).SaveAccessType(dto, session.SessionUser.UserInternalID);
            }
            catch(Exception ex)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                message.Content = new StringContent(ex.Message);
                throw new HttpResponseException(message);
            }
        }

        // PUT api/<controller>/5
        [Route("api/AccessTypes")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [Route("api/AccessTypes/{id}")]
        public void Delete(int id, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.ADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            try
            {
                new AccessTypesRepository(session.getSessionData()).DeleteAccessType(id);
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