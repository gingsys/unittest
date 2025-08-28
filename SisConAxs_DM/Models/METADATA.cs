using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Models
{
    public partial class Metadata
    {
        public int MetadataID { get; set; }
        public Nullable<int> MetadataParentID { get; set; }
        public string MetadataDisplay { get; set; }
        public string MetadataDescription { get; set; }
        public string MetadataInt1 { get; set; }

        //public int MetadataInt1 { get; set; }
        public int MetadataInt2 { get; set; }
        public int MetadataInt3 { get; set; }
        public int MetadataInt4 { get; set; }
        public int MetadataInt5 { get; set; }
        public string MetadataStr1 { get; set; }
        public string MetadataStr2 { get; set; }
        public string MetadataStr3 { get; set; }
        public string MetadataStr4 { get; set; }
        public string MetadataStr5 { get; set; }
        public Nullable<DateTime> MetadataDatetime1 { get; set; }
        public Nullable<DateTime> MetadataDatetime2 { get; set; }
        public Nullable<DateTime> MetadataDatetime3 { get; set; }
        public int MetadataActive { get; set; }

        //public string CreateUser { get; set; }
        //public Nullable<System.DateTime> CreateDate { get; set; }
        public string EditUser { get; set; }
        //public Nullable<System.DateTime> EditDate { get; set; }

        //public virtual Metadata MetadataParent { get; set; }
        //public virtual ICollection<Metadata> MetadataChilds { get; set; }
    }
}
