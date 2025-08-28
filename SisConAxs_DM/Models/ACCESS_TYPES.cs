using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SisConAxs_DM.Models
{
    public partial class AccessTypes
    {
        public AccessTypes()
        {
            this.AccessResources = new List<AccessResources>();
            this.AccessTypeValues = new ObservableCollection<AccessTypeValues>();
        }

        public int AccessTypeID { get; set; }
        public string AccessTypeName { get; set; }
        public int AccessTypeType { get; set; }
        //public string CreateUser { get; set; }
        //public Nullable<System.DateTime> CreateDate { get; set; }
        public string EditUser { get; set; }
        //public Nullable<System.DateTime> EditDate{ get; set; }

        public int AccessTypeCompany { get; set; }

        public virtual ICollection<AccessResources> AccessResources { get; set; }
        public virtual ObservableCollection<AccessTypeValues> AccessTypeValues { get; set; }
    }
}
