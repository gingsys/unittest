using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SisConAxs.Session;
using SisConAxs.Services;
using SisConAxs_DM.Repository;
using SisConAxs_DM.DTO;

namespace SisConAxs.Controllers
{
    public class CommonValueSetsController : ApiController
    {
        /// <summary>Llamada para obtener la lista de CommonValueSets.</summary>
        /// <para>GET api/CommonValueSets </para>
        /// <returns>Lista de CommonValueSet</returns>
        public IQueryable<CommonValueSetsDTO> GetCommonValueSets(HttpRequestMessage msg, [FromUri]FilterCommonValueSets filter)
        {
            var session = SessionManager.ValidateSession(msg);
            var query = new CommonValueSetsRepository(session.getSessionData()).GetCommonValueSets(filter);
            //if (query.Count() == 0)
            //{
            //    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
            //    message.Content = new StringContent("No se encontró ningún registro");
            //    throw new HttpResponseException(message);
            //}
            return query;
        }

        /// <summary>Llamada para obtener el CommonValueSet identificado por el id pasado como parametro.</summary>
        /// <para>GET api/CommonValueSets/5 </para>
        /// <exception cref="HttpResponseException">404 Not Found - Si no devolvió ningún item.</exception>
        /// <returns>Un CommonValueSet</returns>
        [Route("api/CommonValueSets/{id}")]
        [HttpGet]
        public IQueryable<CommonValueSetsDTO> GetCommonValueSetById(string id, HttpRequestMessage msg)
        {
            var session = SessionManager.ValidateSession(msg);
            var query = new CommonValueSetsRepository(session.getSessionData()).GetCommonValueSetById(id);
            //if (query.Count() == 0)
            //{
            //    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
            //    message.Content = new StringContent("No se encontró el registro");
            //    throw new HttpResponseException(message);
            //}
            return query;
        }

        // GET api/<controller>/5/values
        [Route("api/CommonValueSets/{id}/values")]
        [HttpGet]
        public IQueryable<CommonValuesDTO> GetCommonValues(string id, HttpRequestMessage msg, [FromUri] CommonValuesDTO filter)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            var query = new CommonValueSetsRepository(session.getSessionData()).GetCommonValuesBySet(id, session.SessionUser, filter);
            //if (query.Count() == 0)
            //{
            //    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
            //    message.Content = new StringContent("No se encontró el registro");
            //    throw new HttpResponseException(message);
            //}
            return query;
        }

        [Route("api/CommonValueSets/GetDefaultValue/{valueset}")]
        [HttpGet]
        public CommonValuesDTO GetDefaultValue(string valueset, HttpRequestMessage msg)
        {
            var session = SessionManager.ValidateSession(msg);
            CommonValuesDTO defaultValue = new CommonValueSetsRepository(session.getSessionData()).GetDefaultValue(valueset);
            if (defaultValue == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent("No se encontró el registro");
                throw new HttpResponseException(message);
            }
            return defaultValue;
        }

        

        // POST api/<controller>
        public CommonValueSetsDTO Post([FromBody]CommonValueSetsDTO dto, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.ADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            try {
                return new CommonValueSetsRepository(session.getSessionData()).SaveCommonValueSet(dto, session.SessionUser.UserInternalID);
            }
            catch(Exception ex)
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
        [Route("api/CommonValueSets/{id}")]
        [HttpDelete]
        public void Delete(int id, HttpRequestMessage msg)
        {
            LogManager.Log("Al momento de ingresar");
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.ADMIN);
            var session = SessionManager.ValidateSession(msg, approvedProfiles);
            try
            {
                LogManager.Log("Antes de borrar");
                new CommonValueSetsRepository(session.getSessionData()).DeleteCommonValueSet(id);
                LogManager.Log("Despues de borrar");
            }
            catch (Exception ex)
            {
                LogManager.Log("En el Catch 1");
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.PreconditionFailed); // InternalServerError);
                message.Content = new StringContent(ex.Message);
                message.ReasonPhrase = ex.Message;
                LogManager.Error("Error a intentar eliminar un registro de commonvalue: ", ex);
                throw new HttpResponseException(message);
            }
        }
 
    }
}