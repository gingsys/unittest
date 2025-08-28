using SisConAxs.Session;
using SisConAxs_DM.DTO;
using SisConAxs_DM.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SisConAxs.Controllers
{
    public class CalendarController : ApiController
    {
        // GET api/values
        [Route("api/Calendar/GetCalendar")]
        [HttpGet]
        public IQueryable<CalendarDTO> GetCalendar([FromUri]CalendarFilter filter, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            var query = new CalendarRepository(session.getSessionData()).GetCalendar(filter);
            return query;
        }

        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public CalendarDTO Post([FromBody]CalendarDTO calendar, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            try
            {
                return new CalendarRepository(session.getSessionData()).SaveCalendar(calendar, session.SessionUser);
            }
            catch (Exception ex)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                message.Content = new StringContent(ex.Message);
                throw new HttpResponseException(message);
            }
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        [Route("api/Calendar/{id}")]
        public bool Delete(int id, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            try
            {
                return new CalendarRepository(session.getSessionData()).DeleteCalendar(id);
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