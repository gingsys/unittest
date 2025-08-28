using System;
using System.Collections.Generic;

namespace SisConAxs_DM.DTO
{
    public class AccessTypesDTO
    {
        public int AccessTypeID { get; set; }
        public string AccessTypeName { get; set; }
        public int AccessTypeType { get; set; }
        //public int AccessTypeCompany { get; set; }

        public ICollection<AccessTypeValuesDTO> AccessTypeValues { get; set; }
    }
}
