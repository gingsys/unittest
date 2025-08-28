using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Models
{
    public partial class Calendar
    {
        public Calendar()
        {

        }

        public int CalID { get; set; }
        public DateTime CalDate { get; set; }
        public string CalDescription { get; set; }
        public int CalIdCountry { get; set; }
        public bool CalAnual { get; set; }
        public int CalActive { get; set; }
        //public string CreateUser { get; set; }
        //public DateTime CreateDate { get; set; }
        public string EditUser { get; set; }
        //public DateTime EditDate { get; set; }
    }
}
