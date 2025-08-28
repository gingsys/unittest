using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.DTO
{
    public class RequestTemplateDTO
    {
        public int ReqTemplateID { get; set; }
        public int ReqTemplateCompany { get; set; }
        public string ReqTemplateCompanyName { get; set; }
        public int ReqTemplateType { get; set; }
        public string ReqTemplateTypeName { get; set; }
        public int ReqTemplateEmployeeType { get; set; }
        public string ReqTemplateEmployeeTypeName { get; set; }
        public bool ReqTemplateActive { get; set; }

        public ICollection<RequestTemplateDetailDTO> ReqTemplateDetails { get; set; }
    }
}
