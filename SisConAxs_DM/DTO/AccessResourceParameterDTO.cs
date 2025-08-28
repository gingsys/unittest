using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.DTO
{
    public class AccessResourceParameterDTO
    {
        public const int INTEGRACION_ORACLE_COMPANY_MOUNT = 1201;
        public const int INTEGRACION_ORACLE_ID = 1202;
        public const int INTEGRACION_ICARUS_ACCESS = 2958;
        //public const int INTEGRACION_SRA_COMPANY_MOUNT = 0;
        //public const int INTEGRACION_SRA_ID = 0;

        public AccessResourceParameterDTO()
        {

        }

        public int ResourceID { get; set; }
        public int ResourceParameterID { get; set; }
        public string ResourceParameterDisplay { get; set; }
        public Nullable<int> ResourceParameterMetadataID { get; set; }
        public string Value { get; set; }
        public string ValueDisplay { get; set; }
        //public int ValueInt { get; set; }
        //public Nullable<System.DateTime> ValueDate { get; set; }
    }
}
