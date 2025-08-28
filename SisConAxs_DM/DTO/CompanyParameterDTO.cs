using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.DTO
{
    public class CompanyParameterDTO
    {
        public const int SAP_ID_COMPANY = 608;
        public const int SRA_ID_COMPANY = 609;
        public const int ORACLE_ID_COMPANY = 610;

        public CompanyParameterDTO()
        {

        }

        public int CompanyID { get; set; }
        public int CompanyParameterID { get; set; }
        public string CompanyParameterDisplay { get; set; }
        public string Value { get; set; }
        //public int ValueInt { get; set; }
        //public Nullable<System.DateTime> ValueDate { get; set; }
    }
}
