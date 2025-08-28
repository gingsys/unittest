using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.DTO
{
    public class IntegrationOracleLogDTO
    {
        public const string TYPE_UNMATCH_PEOPLE = "UNMATCH_PEOPLE";
        public const string TYPE_UNMATCH_RESPONSABILITY = "UNMATCH_RESPONSABILITY";

        public int LogID { get; set; }
        public string LogType { get; set; }
        public string LogData1 { get; set; }
        public string LogData2 { get; set; }
        public string LogMessage { get; set; }
        public int LogActive { get; set; } = 1;
        public DateTime CreateDate { get; set; }
    }
}
