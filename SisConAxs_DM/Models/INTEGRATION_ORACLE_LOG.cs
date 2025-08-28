using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Models
{
    public class IntegrationOracleLog
    {
        public int LogID { get; set; }
        public string LogType { get; set; }
        public string LogData1 { get; set; }
        public string LogData2 { get; set; }
        public string LogMessage { get; set; }
        public int LogActive { get; set; }

        //public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string EditUser { get; set; }
        //public Nullable<System.DateTime> EditDate { get; set; }
    }
}
