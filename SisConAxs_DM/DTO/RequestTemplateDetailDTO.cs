using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.DTO
{
    public class RequestTemplateDetailDTO
    {
        public int ReqTemplateID { get; set; }
        public int ReqTemplateDetID { get; set; }

        public int ReqTemplateDetCategoryID { get; set; }
        public string ReqTemplateDetCategoryName { get; set; }
        public int ReqTemplateDetResourceID { get; set; }
        public string ReqTemplateDetResourceName { get; set; }
        public string ReqTemplateDetResourceFullName { get; set; }
        public int ReqTemplateDetAccessTypeID { get; set; }

        public string ReqTemplateDetStrValue { get; set; }
        public int? ReqTemplateDetIntValue { get; set; }
        public int ReqTemplateDetTemporal { get; set; }
        public DateTime? ReqTemplateDetValidityFrom { get; set; }
        public DateTime? ReqTemplateDetValidityUntil { get; set; }
        public int ReqTemplateDetAdditional { get; set; }
        public string ReqTemplateDetAdditionalStrValue { get; set; }
        public int ReqTemplateDetAdditionalIntValue { get; set; }
    }
}
