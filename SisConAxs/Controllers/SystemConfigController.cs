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
using System.Net.Mail;
using SisConAxs_DM.Utils;

namespace SisConAxs.Controllers
{
    public class SystemConfigController : ApiController
    {
        [Route("api/NotifConfig/")]
        [HttpGet]
        public NotifConfigDTO GetNotifConfig(HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            //approvedProfiles.Add(UserRole.ADMIN);
            approvedProfiles.Add(UserRole.SYSADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            NotifConfigDTO notifConf = new SystemConfigRepository().GetNotifConfig();
            if (notifConf == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent("No se encontró ningún registro");
                throw new HttpResponseException(message);
            }
            return notifConf;
        }


        // POST api/<controller>
        [Route("api/NotifConfig/")]
        [HttpPost]
        public NotifConfigDTO Post([FromBody]NotifConfigDTO dto, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int>();
            //approvedProfiles.Add(UserRole.ADMIN);
            approvedProfiles.Add(UserRole.SYSADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            NotifConfigDTO notifConfig;
            try
            {
                notifConfig = new SystemConfigRepository().SaveNotifConfig(dto, session.SessionUser.UserInternalID);
                return notifConfig;
            }
            catch (Exception ex)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                message.Content = new StringContent(ex.Message);
                throw new HttpResponseException(message);
            }
        }

        [Route("api/NotifConfig/test-config")]
        [HttpPost]
        public NotifConfigDTO TestConfig([FromBody]NotifConfigDTO dto, HttpRequestMessage msg)
        {
            List<int> approvedProfiles = new List<int> { UserRole.SYSADMIN };
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            try
            {
                var prm = GetParams();
                var mailer = new NotificationMailer { SendTo = prm["sendTo"], Subject = prm["subject"], Body = prm["body"] };
                mailer.SendNotificationMail(dto);
                return dto;
            }
            catch (Exception ex)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                message.Content = new StringContent(ex.Message);
                throw new HttpResponseException(message);
            }
        }

        protected Dictionary<string, string> GetParams()
        {
            return this.Request.GetQueryNameValuePairs()
                    .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
        }
    }
}
