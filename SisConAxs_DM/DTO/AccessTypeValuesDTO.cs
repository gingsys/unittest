using System;
using System.Collections.Generic;

namespace SisConAxs_DM.DTO
{
    public class AccessTypeValuesDTO
    {
        public int AccessTypeID { get; set; }
        public int TypeValueID { get; set; }
        public string TypeValueName { get; set; }
        public string TypeValueDisplay { get; set; }
        public Nullable<int> TypeValueIntVal { get; set; }
        public string TypeValueCharVal { get; set; }
        public int TypeValueDefault { get; set; }
        public int TypeValueAdditional { get; set; }
    }
}
