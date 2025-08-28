using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Models
{
    public class RequestTemplate
    {
        public RequestTemplate() {
            ReqTemplateDetails = new List<RequestTemplateDetail>();
        }

        public int ReqTemplateID { get; set; }
        public int ReqTemplateCompany { get; set; }
        public int ReqTemplateType { get; set; }
        public int ReqTemplateEmployeeType { get; set; }
        public bool ReqTemplateActive { get; set; }
        public string EditUser { get; set; }
        public virtual ICollection<RequestTemplateDetail> ReqTemplateDetails { get; set; }

        public virtual CommonValues RelReqTemplateType { get; set; }
        public virtual CommonValues RelReqTemplateEmployeeType { get; set; }
    }
}
