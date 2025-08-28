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
using SisConAxs_DM.DTO.Response;

namespace SisConAxs.Controllers
{
    public class ResourceCategoriesController : ApiController
    {
        /// <summary>Llamada para obtener la lista de ResourceCategories.</summary>
        /// <para>GET api/ResourceCategories </para>
        /// <returns>Lista de ResourceCategory</returns>
        public IQueryable<ResourceCategoriesDTO> GetResourceCategories(HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            var query = new ResourceCategoriesRepository(session.getSessionData()).GetResourceCategories();
            //if (query.Count() == 0)
            //{
            //    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NoContent);
            //    message.Content = new StringContent("No se encontró ningún registro");
            //    throw new HttpResponseException(message);
            //}
            return query;
        }

        [Route("api/ResourceCategories/paginate")]
        [HttpGet]
        public PaginationResponse<ResourceCategoriesDTO> GetResourceCategoriesPaginate([FromUri] ResourceCategoryFilter filter, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            return new ResourceCategoriesRepository(session.getSessionData()).GetResourceCategoriesPaginate(filter);
        }

        /// <summary>Llamada para obtener el ResourceCategory identificado por el id pasado como parametro.</summary>
        /// <para>GET api/ResourceCategories/5 </para>
        /// <exception cref="HttpResponseException">404 Not Found - Si no devolvió ningún item.</exception>
        /// <returns>Un ResourceCategory</returns>
        [Route("api/ResourceCategories/{id}")]
        [HttpGet]
        public IQueryable<ResourceCategoriesDTO> GetResourceCategoryById(int id, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            var query = new ResourceCategoriesRepository(session.getSessionData()).GetResourceCategoryById(id);
            //if (query.Count() == 0)
            //{
            //    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
            //    message.Content = new StringContent("No se encontró el registro");
            //    throw new HttpResponseException(message);
            //}
            return query;
        }

        // POST api/<controller>
        public ResourceCategoriesDTO Post([FromBody]ResourceCategoriesDTO dto, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.SYSADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            try
            {
                return new ResourceCategoriesRepository(session.getSessionData()).SaveCategory(dto, session.SessionUser.UserInternalID);
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
        [Route("api/ResourceCategories/{id}")]
        public void Delete(int id, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.SYSADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            try
            {
                new ResourceCategoriesRepository(session.getSessionData()).DeleteResourceCategory(id);
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