using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Models
{
    public partial class AccessResourceParameter
    {
        public AccessResourceParameter()
        {
            
        }

        public int ResourceID { get; set; }
        public int ResourceParameterID { get; set; }
        public Nullable<int> ResourceParameterMetadataID { get; set; }
        public string Value { get; set; }
        //public int ValueInt { get; set; }
        //public Nullable<System.DateTime> ValueDate { get; set; }

        //public string CreateUser { get; set; }
        //public Nullable<System.DateTime> CreateDate { get; set; }
        public string EditUser { get; set; }
        //public Nullable<System.DateTime> EditDate { get; set; }

        public virtual AccessResources AccessResource { get; set; }
        public virtual CommonValues ParameterName { get; set; }
    }
}
