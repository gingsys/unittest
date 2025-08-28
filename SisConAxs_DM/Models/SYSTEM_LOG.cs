using System;
using System.Collections.Generic;

namespace SisConAxs_DM.Models
{
    public partial class SystemLog
    {
        public int LogID { get; set; }
        public System.DateTime LogDate { get; set; }
        public string LogType { get; set; }
        public string LogMessage { get; set; }
    }
}
