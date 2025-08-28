using System;
using System.Collections.Generic;

namespace SisConAxs_DM.Models
{
    public partial class USER_OBJECT_ACCESS
    {
        public int UO_ID { get; set; }
        public int UO_USER_ID { get; set; }
        public int UO_OBJECT_ID { get; set; }
        public int UO_ACCESS_VALUE { get; set; }
        public System.DateTime UO_UP_DATE { get; set; }
        public int UO_UP_WORKFLOW { get; set; }
        public Nullable<System.DateTime> UO_DOWN_DATE { get; set; }
        public Nullable<int> UO_DOWN_WORKFLOW { get; set; }
        public int UO_ACTIVE { get; set; }
        public string CREATE_USER { get; set; }
        public System.DateTime CREATE_DATE { get; set; }
        public string EDIT_USER { get; set; }
        public System.DateTime EDIT_DATE { get; set; }

        public virtual AccessResources AccessResources { get; set; }
        public virtual AccessTypeValues AccessTypeValues { get; set; }
        public virtual AccessUsers AccessUsers { get; set; }
        public virtual WorkflowExecution WorkflowExecution { get; set; }
        public virtual WorkflowExecution WorkflowExecution1 { get; set; }
    }
}
