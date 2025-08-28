using System;
using System.Collections.Generic;

namespace SisConAxs_DM.Models
{
    public partial class ResourceCategories
    {
        public ResourceCategories()
        {
            this.AccessResources = new List<AccessResources>();
        }

        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        //public string CreateUser { get; set; }
        //public Nullable<System.DateTime> CreateDate { get; set; }
        public string EditUser { get; set; }
        //public Nullable<System.DateTime> EditDate { get; set; }

        public virtual ICollection<AccessResources> AccessResources { get; set; }
    }
}
