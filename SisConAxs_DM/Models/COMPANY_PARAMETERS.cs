using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Models
{
    public partial class CompanyParameter
    {
        public CompanyParameter()
        {

        }

        public int CompanyID { get; set; }
        public int CompanyParameterID { get; set; }
        public string Value { get; set; }
        //public int ValueInt { get; set; }
        //public Nullable<System.DateTime> ValueDate { get; set; }

        //public string CreateUser { get; set; }
        //public Nullable<System.DateTime> CreateDate { get; set; }
        public string EditUser { get; set; }
        //public Nullable<System.DateTime> EditDate { get; set; }

        public virtual Company Company { get; set; }
        public virtual CommonValues ParameterName { get; set; }
    }
}