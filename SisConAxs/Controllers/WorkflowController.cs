using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SisConAxs.Session;
using SisConAxs_DM.Repository;
using SisConAxs_DM.DTO;
using SisConAxs.Services;

namespace SisConAxs.Controllers
{
    public class WorkflowsController : ApiController
    {
        /// <summary>Llamada para obtener la lista de Workflows.</summary>
        /// <para>GET api/Workflows</para>
        /// <returns>Lista de Workflow</returns>
        public IQueryable<WorkflowDTO> GetWorkflows(HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            var query = new WorkflowRepository(session.getSessionData()).GetWorkflows();
            //if (query.Count() == 0)
            //{
            //    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
            //    message.Content = new StringContent("No se encontró ningún registro");
            //    throw new HttpResponseException(message);
            //}
            return query;
        }

        /// <summary>Llamada para obtener el Workflow identificado por el id pasado como parametro.</summary>
        /// <para>GET api/Workflows/5 </para>
        /// <exception cref="HttpResponseException">404 Not Found - Si no devolvió ningún item.</exception>
        /// <returns>Un Workflow</returns>
        [Route("api/Workflows/{id}")]
        [HttpGet]
        public WorkflowDTO GetResourceCategoryById(int id, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            WorkflowDTO workflowDTO = new WorkflowRepository(session.getSessionData()).GetWorkflowById(id);
            try
            {
                if (workflowDTO == null)
                {
                    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                    message.Content = new StringContent("No se encontró el registro");
                    throw new HttpResponseException(message);
                }
            }
            catch (Exception ex)
            {

                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                message.Content = new StringContent(ex.Message);
                throw new HttpResponseException(message);
            }
            return workflowDTO;
        }

        // GET api/<controller>/5/values
        [Route("api/Workflows/{id}/values")]
        [HttpGet]
        public string GetWorkflows(int id, HttpRequestMessage msg)
        {
            return "Un valor";
        }

        // POST api/<controller>
        public WorkflowDTO Post([FromBody]WorkflowDTO dto, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.ADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            validateWorkFlow(dto);
            try
            {
                return new WorkflowRepository(session.getSessionData()).SaveWorkflow(dto, session.SessionUser.UserInternalID);
            }
            catch (Exception ex)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                //LogManager.Error($"[{System.DateTime.Now}] {ex.Message}.");
                message.Content = new StringContent(ex.Message);
                throw new HttpResponseException(message);
            }
        }

        [Route("api/Workflows/{id}")]
        public WorkflowDTO CopyWorkflow(int id, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.ADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            try
            {
                return new WorkflowRepository(session.getSessionData()).CopyWorkflow(id, session.SessionUser.UserInternalID);
            }
            catch (Exception ex)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                //LogManager.Error($"[{System.DateTime.Now}] {ex.Message}.");
                message.Content = new StringContent(ex.Message);
                throw new HttpResponseException(message);
            }
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [Route("api/Workflows/{id}")]
        public void Delete(int id, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.ADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            try
            {
                new WorkflowRepository(session.getSessionData()).DeleteWorkflow(id, session.SessionUser.UserInternalID);
            }
            catch (Exception ex)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                message.Content = new StringContent(ex.Message);
                throw new HttpResponseException(message);
            }
        }


        private void validateWorkFlow(WorkflowDTO wf)
        {
            List<WorkflowItemsDTO> list = wf.WorkflowItems.ToList();
            validateUnattachedItems(list);
            validateNextItemNotItself(list);
            validateNoLoop(list);
        }

        private void validateUnattachedItems(List<WorkflowItemsDTO> itemList)
        {
            List<WorkflowItemsDTO> list = null;
            bool success = false;

            foreach (WorkflowItemsDTO wfItem in itemList)
            {
                list = new List<WorkflowItemsDTO>();
                list.AddRange(itemList);
                list.Remove(wfItem);
                success = false;

                if (wfItem.WfItemStep == 1)
                { //el primer item del WF no necesita ser referenciado por algun otro
                    continue;
                }

                foreach (WorkflowItemsDTO item in list)
                {
                    if (item.WfItemTypeName == "CONSULTA")
                    {
                        if (item.WfItemApproveItem == wfItem.WfItemStep ||
                        item.WfItemRejectItem == wfItem.WfItemStep ||
                        item.WfItemTimeoutItem == wfItem.WfItemStep)
                        {
                            success = true;
                            break;
                        }
                    }
                    else if (item.WfItemNextItem == wfItem.WfItemStep)
                    {//si el item es de tipo notificacion o accion
                        success = true;
                        break;
                    }                    
                }

                if (!success)
                {
                    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                    message.Content = new StringContent("El item: " + wfItem.WfItemName + " esta suelto.");
                    throw new HttpResponseException(message);
                }
            }
        }

        private void validateNextItemNotItself(List<WorkflowItemsDTO> itemList)
        {
            HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            foreach (WorkflowItemsDTO item in itemList)
            {
                if (item.WfItemTypeName == "CONSULTA")
                {
                    if (item.WfItemApproveItem == item.WfItemStep ||
                    item.WfItemRejectItem == item.WfItemStep ||
                    item.WfItemTimeoutItem == item.WfItemStep)
                    {
                        message.Content = new StringContent("El item: " + item.WfItemName + " tiene uno de sus nextitem apuntandose a si mismo");
                        throw new HttpResponseException(message);
                    }
                }
                else if (item.WfItemNextItem == item.WfItemStep)
                {//si el item es de tipo notificacion o accion
                    message.Content = new StringContent("El item: " + item.WfItemName + " tiene como next item a si mismo");
                    throw new HttpResponseException(message);
                }
            }
        }

        private void validateNoLoop(List<WorkflowItemsDTO> wfItemList)
        {
            List<WorkflowItemsDTO> list = new List<WorkflowItemsDTO>();
            list.AddRange(wfItemList);
            List<WorkflowItemsDTO> nextItems = null;

            foreach (WorkflowItemsDTO item in list)
            {
                nextItems = new List<WorkflowItemsDTO>();
                if (item.WfItemTypeName == "CONSULTA")
                {
                    nextItems.Add(list.Where(x => x.WfItemStep == item.WfItemApproveItem).FirstOrDefault());
                    nextItems.Add(list.Where(x => x.WfItemStep == item.WfItemRejectItem).FirstOrDefault());
                    nextItems.Add(list.Where(x => x.WfItemStep == item.WfItemTimeoutItem).FirstOrDefault());
                    if (!loopDetected(item, nextItems))
                    {
                        continue;
                    }
                }
                else
                {//si el item es de tipo notificacion o accion
                    nextItems.Add(list.Where(x => x.WfItemStep == item.WfItemNextItem).FirstOrDefault());
                    if (!loopDetected(item, nextItems))
                    {
                        continue;
                    }
                }

                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                message.Content = new StringContent("El item: " + item.WfItemName + " tiene una relacion en loop.");
                throw new HttpResponseException(message);
            }

            //foreach (WorkflowItemsDTO item in wfItemList)
            //{
            //    if(recursive(wfItemList, item, item.WfItemStep)){
            //        HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            //        message.Content = new StringContent("El item: " + item.WfItemName + " tiene una relacion en loop.");
            //        throw new HttpResponseException(message);
            //    }
            //}            
        }

        private bool loopDetected(WorkflowItemsDTO wfItem, List<WorkflowItemsDTO> nextItems)
        {
            foreach (WorkflowItemsDTO item in nextItems)
            {
                if (item == null)
                    continue;
                if (item.WfItemTypeName == "CONSULTA" && (item.WfItemApproveItem == wfItem.WfItemStep || item.WfItemRejectItem == wfItem.WfItemStep || item.WfItemTimeoutItem == wfItem.WfItemStep))
                {
                    return true;
                }
                else if (item.WfItemNextItem == wfItem.WfItemStep) {
                    return true;
                }
            }
            
            return false;
        }

        private bool recursive(List<WorkflowItemsDTO> wfItemList, WorkflowItemsDTO item, int originalStep) {
            if (item.WfItemStep == originalStep)
                return true;
            if (item == null)
                return false;

            if (item.WfItemTypeName == "CONSULTA")
            {
                if (recursive(wfItemList, wfItemList.Where(x => x.WfItemStep == item.WfItemApproveItem).FirstOrDefault(), originalStep) ||
                    recursive(wfItemList, wfItemList.Where(x => x.WfItemStep == item.WfItemRejectItem).FirstOrDefault(), originalStep) ||
                    recursive(wfItemList, wfItemList.Where(x => x.WfItemStep == item.WfItemTimeoutItem).FirstOrDefault(), originalStep))
                    return true;
                else return false;
            }
            else {
                if (item.WfItemNextItem == null || item.WfItemNextItem == 0)
                    return false;
                return recursive(wfItemList, wfItemList.Where(x => x.WfItemStep == item.WfItemNextItem).FirstOrDefault(), originalStep);
            }
        }
    }
}