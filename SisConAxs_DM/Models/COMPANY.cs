using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Models
{
    public partial class Company
    {
        public Company()
        {
            //this.CompanyParameters = new List<AccessResourceParameter>();
        }

        public int CompanyID { get; set; }
        public string CompanyTaxpayerID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyDisplay { get; set; }
        //public string CompanyAD { get; set; }
        public string CompanyAddress { get; set; }
        public int CompanyCountry { get; set; }
        public int CompanyActive { get; set; }
        //public string CreateUser { get; set; }
        //public Nullable<System.DateTime> CreateDate { get; set; }
        public string EditUser { get; set; }
        //public Nullable<System.DateTime> EditDate { get; set; }

        //public virtual ICollection<CompanyParameter> CompanyParameters { get; set; }

        public virtual ICollection<People> People { get; set; }
    }
}
