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


    public class ResourcePeopleController : ApiController
    {
        [HttpGet]
        [Route("api/resource/people/{id}")]
        public IQueryable<ResourcePeopleDTO> GetByPeopleID(HttpRequestMessage msg, [FromUri]ResourcePeopleFilter filter, int id)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            var query = new ResourcePeopleRepository().GetByPeopleID(id, filter.onlyActive > 0);
            return query;
        }
    }
}
