using SisConAxs.Integration;
using SisConAxs.Integration.DTO;
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
    //[RoutePrefix("api/integration/oracle")]
    public class IntegrationOracleController : ApiController
    {
        [HttpGet]
        [Route("api/integration/oracle/log/{type}")]
        public IEnumerable<IntegrationOracleLogDTO> GetByType(string type, HttpRequestMessage msg, [FromUri] IntegrationOracleLogFilter filters)
        {
            var approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.ADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg, approvedProfiles);
            return new IntegrationOracleLogRepository().GetLogByType(type, filters.LogActive);
        }

        [HttpPost]
        [Route("api/integration/oracle/check-responsability-conflicts/{user}")]
        public OracleCheckReponsabilityResponse CheckResponsabilityConflicts(string user, HttpRequestMessage msg, [FromBody] List<string> responsabilities)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            return new IntegrationResourceOracle().CheckResponsabilityConflicts(user, responsabilities);
        }

        [HttpPost]
        [Route("api/integration/oracle/sync-metadata")]
        public void SyncMetadata(HttpRequestMessage msg)
        {
            var approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.ADMIN);
            approvedProfiles.Add(UserRole.SYSADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg);
            new IntegrationResourceOracle().SyncMetadata();
        }

        [HttpPost]
        [Route("api/integration/oracle/sync-resources")]
        public void SyncResources(HttpRequestMessage msg)
        {
            var approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.ADMIN);
            approvedProfiles.Add(UserRole.SYSADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg);
            new IntegrationResourceOracle().SyncResources();
        }

        [HttpPost]
        [Route("api/integration/oracle/sync-access")]
        public void SyncAccess(HttpRequestMessage msg)
        {
            var approvedProfiles = new List<int>();
            approvedProfiles.Add(UserRole.ADMIN);
            approvedProfiles.Add(UserRole.SYSADMIN);
            SessionInfo session = SessionManager.ValidateSession(msg);
            new IntegrationResourceOracle().SyncPeopleAccess();
        }
    }
}
