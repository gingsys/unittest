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
    public class WorkflowApproveHierarchyController : ApiController
    {
        /// <summary>Llamada para obtener la lista de AccesTypes.</summary>
        /// <para>GET api/WorkflowApproveHierarchy </para>
        /// <returns>Lista de AccesType</returns>
        ///[Route("api/WorkflowApproveHierarchy/")]
        ///[HttpGet]
        public IQueryable<WorkflowApproveHierarchyDTO> GetWorkflowApproveHierarchy([FromUri] WfHierarchyFilter filter, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            var query = new WorkflowApproveHierarchyRepository(session.getSessionData()).GetWfApproveHierarchy(filter);
            //if (query.Count() == 0)
            //{
            //    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
            //    message.Content = new StringContent("No se encontró ningún registro");
            //    throw new HttpResponseException(message);
            //}
            return query;
        }

        /// <summary>Llamada para obtener el AccesType identificado por el id pasado como parametro.</summary>
        /// <para>GET api/WorkflowApproveHierarchy/5 </para>
        /// <exception cref="HttpResponseException">404 Not Found - Si no devolvió ningún item.</exception>
        /// <returns>Un AccesType</returns>
        [Route("api/WorkflowApproveHierarchy/{id}")]
        [HttpGet]
        public IQueryable<WorkflowApproveHierarchyDTO> GetAccessTypeById(int id, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            var query = new WorkflowApproveHierarchyRepository(session.getSessionData()).GetWfApproveHierarchyById(id);
            //if (query.Count() == 0) {
            //    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
            //    message.Content = new StringContent("No se encontró el registro");
            //    throw new HttpResponseException(message);
            //}
            return query;
        }

        // GET api/<controller>/5/values
        [Route("api/WorkflowApproveHierarchy/{id}/values")]
        [HttpGet]
        public string GetTypeAccessValues(int id, HttpRequestMessage msg)
        {
            return "Un valor";
        }

        // POST api/<controller>
        public WorkflowApproveHierarchyDTO Post([FromBody]WorkflowApproveHierarchyDTO dto, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.ADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            try
            {
                return new WorkflowApproveHierarchyRepository(session.getSessionData()).SaveWorkflowApproveHierarchy(dto, session.SessionUser.UserInternalID);
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
        [Route("api/WorkflowApproveHierarchy/{id}")]
        public void Delete(int id, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.ADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            try
            {
                new WorkflowApproveHierarchyRepository(session.getSessionData()).DeleteWorkflowApproveHierarchy(id);
            }
            catch (Exception ex)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                message.Content = new StringContent(ex.Message);
                throw new HttpResponseException(message);
            }
        }


        
        [HttpGet]
        [Route("api/WorkflowApproveHierarchy/GetMember/{department}/{position}")]
        public string GetPeopleMemberWorkflowApproveHierarchy(int department, int position, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            try
            {
                return new WorkflowApproveHierarchyRepository(session.getSessionData()).GetPeopleMemberWorkflowApproveHierarchy(department, position);
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