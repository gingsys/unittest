using System;
using System.Collections.Generic;

namespace SisConAxs_DM.Models
{
    public partial class CommonValueSets
    {
        public CommonValueSets()
        {
            this.CommonValues = new List<CommonValues>();
        }

        public int CommonValueSetID { get; set; }
        public string CommonValueSetName { get; set; }
        public string CommonValueSetDesc { get; set; }
        public int CommonValueSetSeqSeed { get; set; }
        //public string CreateUser { get; set; }
        //public Nullable<System.DateTime> CreateDate { get; set; }
        public string EditUser { get; set; }
        //public Nullable<System.DateTime> EditDate { get; set; }
        public int CommonValueSetSystemValue { get; set; }
        public bool CommonValueSetRestrictedByCompany { get; set; }

        public virtual ICollection<CommonValues> CommonValues { get; set; }
    }
}
