using System;
using System.Collections.Generic;

namespace SisConAxs_DM.Models
{
    public partial class AccessTypeValues
    {
        public int AccessTypeID { get; set; }
        public int TypeValueID { get; set; }
        public string TypeValueName { get; set; }
        public string TypeValueDisplay { get; set; }
        public Nullable<int> TypeValueIntVal { get; set; }
        public string TypeValueCharVal { get; set; }
        public int TypeValueDefault { get; set; }
        public int TypeValueAdditional { get; set; }
        //public string CreateUser { get; set; }
        //public Nullable<System.DateTime> CreateDate { get; set; }
        public string EditUser { get; set; }
        //public Nullable<System.DateTime> EditDate { get; set; }

        public virtual AccessTypes AccessTypes { get; set; }
        //public virtual USER_OBJECT_ACCESS USER_OBJECT_ACCESS { get; set; }
    }
}
