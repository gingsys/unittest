using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.DTO
{
    public partial class CompanyDTO
    {
        public int CompanyID { get; set; }
        public string CompanyTaxpayerID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyDisplay { get; set; }
        public string CompanyAddress { get; set; }
        public int CompanyCountry { get; set; }
        public string CompanyCountryName { get; set; }
        public int CompanyActive { get; set; }

        //public ICollection<CompanyParameterDTO> CompanyParameters { get; set; }
    }
}
