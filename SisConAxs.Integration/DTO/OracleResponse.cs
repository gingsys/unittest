using SisConAxs_DM.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs.Integration.DTO
{
    public class OracleAccessPeopleResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public Nullable<DateTime> AccessStartDate { get; set; }
        public Nullable<DateTime> AccessEndDate { get; set; }
    }

    public class OracleCheckReponsabilityResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public bool Response { get; set; }
        public List<string> Conflicts { get; set; }
    }
}
