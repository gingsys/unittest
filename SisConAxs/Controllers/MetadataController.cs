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
    public class MetadataController : ApiController
    {
        // GET: api/Metadata/5
        [Route("api/metadata/{id}")]
        [HttpGet]
        public MetadataDTO GetMetadataByID(int id, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            return new MetadataRepository().GetMetadataByID(id);
        }

        [Route("api/metadata/{id}/list")]
        [HttpGet]
        public IEnumerable<MetadataDTO> GetMetadataByParentID(int id, HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            return new MetadataRepository().GetMetadataByParentID(id);
        }

        // GET: api/Metadata
        [Route("api/metadata")]
        [HttpGet]
        public IEnumerable<MetadataDTO> Get(HttpRequestMessage msg)
        {
            SessionInfo session = SessionManager.ValidateSession(msg);
            return new MetadataRepository().GetMetadata();
        }

        [Route("api/metadata")]
        public CompanyDTO Post([FromBody]CompanyDTO dto, HttpRequestMessage msg)
        {
            throw new NotImplementedException();
        }

        // PUT api/<controller>/5
        [Route("api/metadata")]
        public void Put(int id, [FromBody]string value)
        {
            throw new NotImplementedException();
        }

        // DELETE api/<controller>/5
        [Route("api/metadata/{id}")]
        public void Delete(int id, HttpRequestMessage msg)
        {
            throw new NotImplementedException();
        }
    }
}
