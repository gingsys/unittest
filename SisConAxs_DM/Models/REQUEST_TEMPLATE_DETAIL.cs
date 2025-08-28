using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Models
{
    public class RequestTemplateDetail
    {
        public int ReqTemplateID { get; set; }
        public int ReqTemplateDetID { get; set; }
        public int ReqTemplateDetResourceID { get; set; }
        public string ReqTemplateDetStrValue { get; set; }
        public int? ReqTemplateDetIntValue { get; set; }
        public int ReqTemplateDetTemporal { get; set; }
        public DateTime? ReqTemplateDetValidityFrom { get; set; }
        public DateTime? ReqTemplateDetValidityUntil { get; set; }
        public int ReqTemplateDetAdditional { get; set; }
        public string ReqTemplateDetAdditionalStrValue { get; set; }
        public int ReqTemplateDetAdditionalIntValue { get; set; }
        public string EditUser { get; set; }


        public virtual RequestTemplate RelRequestTemplate { get; set; }
        public virtual AccessResources RelResource { get; set; }
    }
}
