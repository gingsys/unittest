using System;
using System.Collections.Generic;

namespace SisConAxs_DM.DTO
{
    public class CommonValuesDTO
    {
        public int CommonValueID { get; set; }
        public int CommonValueSetID { get; set; }
        public string CommonValueName { get; set; }
        public string CommonValueDisplay { get; set; }
        public string CommonValueDesc { get; set; }

        public Nullable<int> CommonValueCompany { get; set; }
    }
}
