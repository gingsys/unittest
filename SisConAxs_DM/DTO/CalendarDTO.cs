using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.DTO
{
    public partial class CalendarDTO
    {
        public int CalID { get; set; }
        public DateTime CalDate { get; set; }
        public string CalDescription { get; set; }
        public int CalIdCountry { get; set; }
        public bool CalAnual { get; set; }
        public int CalActive { get; set; }
    }
}
