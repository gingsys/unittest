using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Models
{
    public class ResourcePeopleLog
    {
        public ResourcePeopleLog()
        {

        }

        public int ResourcePeopleLogID { get; set; }
        public int ResourcePeopleID { get; set; }
        public int Action { get; set; }
        public string Source { get; set; }
        public string Description { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string EditUser { get; set; }
        //public Nullable<System.DateTime> EditDate { get; set; }

        public virtual ResourcePeople ResourcePeople { get; set; }
    }
}
