using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SisConAxs.Session;
using SisConAxs_DM.Repository;
using SisConAxs_DM.DTO;
using System.Web;
using System.Configuration;
using System.Web.Script.Serialization;
using System.Net.Http.Headers;
using System.IO;
using Newtonsoft.Json;
using SisConAxs.Integration;

namespace SisConAxs.Controllers
{
    // Controller
    public class AccessRequestController : ApiController
    {
        /// <summary>Llamada para obtener la lista de AccesTypes.</summary>
        /// <para>GET api/AccessRequests </para>
        /// <returns>Lista de AccesType</returns>
        ///[Route("api/AccessRequests/")]
        ///[HttpGet]
        public AccessRequestPagedData GetAccessRequest([FromUri]AccessRequestFilter filter, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            var permissions = session.getSessionData().GetPermissions();
            var data = new AccessRequestRepository(session.getSessionData()).GetAccessRequest(filter, session.SessionUser.UserInternalID, permissions);
            //if (data == null)
            //{
            //    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
            //    message.Content = new StringContent("No se encontró ningún registro");
            //    throw new HttpResponseException(message);
            //}
            return data;
        }

        /// <summary>Llamada para obtener el AccesType identificado por el id pasado como parametro.</summary>
        /// <para>GET api/AccessRequests/5 </para>
        /// <exception cref="HttpResponseException">404 Not Found - Si no devolvió ningún item.</exception>
        /// <returns>Un AccesType</returns>
        [Route("api/AccessRequest/{id}")]
        [HttpGet]
        public AccessRequestDTO GetAccessRequestById(int id, [FromUri]AccessRequestFilter filter, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            var request = new AccessRequestRepository(session.getSessionData()).GetAccessRequestById(id, filter.ViewType, session.SessionUser.UserInternalID);
            if (request == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent("No se encontró el registro");
                throw new HttpResponseException(message);
            }
            return request;
        }


        // GET api/<controller>/details?filters
        [Route("api/AccessRequest/RequestBy/{user}")]
        [HttpGet]
        public PeopleDTO GetAccessRequestDetails(string user, HttpRequestMessage msg)
        {
            string requestBy = "";
            SessionInfo session = SessionManager.ValidateSession(msg);
            requestBy = user == "0" ? session.SessionUser.UserInternalID : user;
            var people = new PeopleRepository(session.getSessionData()).GetPeopleByUserId(requestBy);
            if (people == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent("No se encontró el registro");
                throw new HttpResponseException(message);
            }
            return people;
        }



        // GET api/<controller>/details?filters
        [Route("api/AccessRequest/Details")]
        [HttpGet]
        public List<AccessRequestDetailsDTO> GetAccessRequestDetails([FromUri]AccessRequestDetailFilter filter, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            var query = new AccessRequestRepository(session.getSessionData()).GetAccessRequestDetails(filter.PeopleID, filter.RequestType, filter.CompanyID);
            if (query == null || query.Count() == 0)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent("No tiene recursos para asignar.");
                throw new HttpResponseException(message);
            }
            return query;
        }

        [Route("api/AccessRequest/Details/{RequestDetailID}/History")]
        [HttpGet]
        public IQueryable<AccessRequestDetailHistoryDTO> GetDetailHistory(int RequestDetailID, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            var query = new AccessRequestRepository(session.getSessionData()).GetDetailHistory(RequestDetailID);
            return query;
        }




        [Route("api/AccessRequest/HaveAccess")]
        [HttpGet]
        public CommonValuesDTO GetHaveAccess([FromUri]AccessRequestDetailFilter filter, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            var commonvalue = new AccessRequestRepository(session.getSessionData()).GetHaveAccess(filter.PeopleID, filter.RequestType, session.SessionUser);
            return commonvalue;
        }


        // POST api/<controller>
        public AccessRequestDTO Post(HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add((int)UserRole.SOLICITANTE);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            string errorMessage = "";
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var dto = new JavaScriptSerializer().Deserialize<AccessRequestDTO>(httpRequest.Params["entity"]);

                PeopleDTO solicitante = new PeopleRepository(session.getSessionData()).GetPeopleByUserId(session.SessionUser.UserInternalID);
                if (solicitante == null) // NO ESTA COMO PERSONA EN EL SISTEMA
                {
                    solicitante = new PeopleDTO { UserID = session.SessionUser.UserInternalID };
                }

                var requestBaja = dto.AccessRequestDetails.Any(x => x.RequestDetType == AccessRequestDTO.TYPE_BAJA);
                if (dto.RequestTo != solicitante.PeopleID && requestBaja)
                {
                    approvedProfiles.Clear();
                    approvedProfiles.Add(UserRole.DAR_BAJA);
                    session = SessionManager.ValidateSession(msg, approvedProfiles);
                }

                if (!string.IsNullOrWhiteSpace(dto.RequestNote))
                {
                    string names = !string.IsNullOrWhiteSpace(solicitante.PeopleOrdinalNames) ? solicitante.PeopleOrdinalNames : solicitante.UserID;
                    dto.RequestNote = $"[{DateTime.Now.ToString()} - {names}] {dto.RequestNote}";
                }

                if (httpRequest.Files.Count > 0)
                {
                    dto.FileAttached = httpRequest.Files[0];
                }

                //var checkOracleConflicts = new IntegrationResourceOracle().CheckResponsabilityConflicts(dto);
                //if (!checkOracleConflicts.Response)
                //{
                //    throw new ModelExceptionDetails<List<string>>("No puede crear la solicitud porque tiene conflictos entre los permisos Oracle.", checkOracleConflicts.Conflicts);
                //}

                return new AccessRequestRepository(session.getSessionData()).InsertAccessRequest(dto, session.SessionUser.UserInternalID);
            }
            catch (ModelExceptionDetails<Object> ex)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                message.Content = new StringContent(JsonConvert.SerializeObject(ex.GetErrorResponseDTO()));
                throw new HttpResponseException(message);
            }
            catch (HttpResponseException ex)
            {
                errorMessage = "No cuentas con privilegios suficientes para ejecutar algun tipo de baja";
                HttpResponseMessage message = new HttpResponseMessage(ex.Response.StatusCode);
                message.Content = new StringContent(errorMessage);
                throw new HttpResponseException(message);
            }
            catch (Exception ex)
            {
                errorMessage = "";
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                message.Content = new StringContent(ex.Message);
                throw new HttpResponseException(message);
            }
            finally
            {
                if (!String.IsNullOrWhiteSpace(errorMessage))
                {
                    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                    message.Content = new StringContent(errorMessage);
                    throw new HttpResponseException(message);
                }
            }
        }

        // DELETE api/<controller>/Approve
        [HttpPost]
        [Route("api/AccessRequest/Approve")]
        public void ApprovePost(HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.APROBADOR);
            SessionInfo session = SessionManager.ValidateSession(msg,approvedProfiles);
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var obj = new JavaScriptSerializer().Deserialize<AccessRequestApproveDTO>(httpRequest.Params["entity"]);
                if (httpRequest.Files.Count > 0)
                {
                    obj.FileAttached = httpRequest.Files[0];
                }
                new AccessRequestRepository(session.getSessionData()).ApproveAccessRequest(obj);
            }
            catch (Exception ex)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                message.Content = new StringContent(ex.Message);
                throw new HttpResponseException(message);
            }
        }

        [HttpPost]
        [Route("api/AccessRequest/MassiveDeactivateAccess/")]
        public void MassiveDeactivateAccess([FromBody]AccesRequestExcelFilter filter, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.ADMIN);
            approvedProfiles.Add(UserRole.DAR_BAJA);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            try
            {
                new AccessRequestRepository(session.getSessionData()).MassiveDeactivateAccess(filter.listPeopleID);
            }
            catch (Exception ex)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                message.Content = new StringContent(ex.Message);
                throw new HttpResponseException(message);
            }
        }

        [HttpPost]
        [Route("api/AccessRequest/MassiveDeactivateAccess/loadFromExcel/")]
        public IQueryable<PeopleDTO> MassiveDeactivateAccessLoadFromExcel(HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.ADMIN);
            approvedProfiles.Add(UserRole.DAR_BAJA);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            try
            {
                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count > 0)
                {
                    string filePath = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["FilePath"];
                    string fileName = ConfigurationManager.AppSettings["FileName"];
                    string fullPath = filePath + @"\" + fileName;

                    var postedFile = httpRequest.Files[0];
                    postedFile.SaveAs(fullPath);
                }
                return new PeopleRepository(session.getSessionData()).GetPeopleFromExcel();
            }
            catch (Exception ex)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                message.Content = new StringContent(ex.Message);
                throw new HttpResponseException(message);
            }
        }

        [HttpPost]
        [Route("api/AccessRequest/MassiveDeactivateAccess/deleteFromExcel/")]
        public void MassiveDeactivateAccessDeleteFromExcel([FromBody]AccesRequestExcelFilter filter, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.ADMIN);
            approvedProfiles.Add(UserRole.DAR_BAJA);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            try
            {
                new AccessRequestRepository(session.getSessionData()).MassiveDeactivateAccessFromExcel();
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
        [Route("api/AccessRequest/{id}")]
        public void Delete(int id, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            try
            {
                new AccessRequestRepository(session.getSessionData()).DeleteAccessRequest(id);
            }
            catch (Exception ex)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                message.Content = new StringContent(ex.Message);
                throw new HttpResponseException(message);
            }
        }

        [HttpPost]
        [Route("api/AccessRequest/cancel/{id}")]
        public void CancelRequest(int id, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            try
            {
                string UserInternalID = session.SessionUser.UserInternalID;
                new AccessRequestRepository(session.getSessionData()).CancelRequest(id, UserInternalID);
            }
            catch (Exception ex)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                message.Content = new StringContent(ex.Message);
                throw new HttpResponseException(message);
            }
        }


        [Route("api/accessrequest/pendings/{user}")]
        [HttpGet]
        public object GetRequestPendings(HttpRequestMessage msg, string user)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            return new AccessRequestRepository(session.getSessionData()).GetRequestApprovePending(user);
        }
        //DESCARGA DE ARCHIVOS "&"
        [Route("api/accessrequest/download")]
        [HttpGet]
        public HttpResponseMessage DownloadFile([FromUri]DownloadFileFilter filter, HttpRequestMessage msg)
        {
            _ = new HttpResponseMessage();
            try
            {
                
                var bytesFile = new AccessRequestRepository(null).DownloadFile(filter.FileName, filter.RequestNumber);

                HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new ByteArrayContent(bytesFile);
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octec-stream");
                result.Content.Headers.ContentDisposition.FileName = filter.FileName;

                return result;
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.Gone);
            }
        }

    }
}