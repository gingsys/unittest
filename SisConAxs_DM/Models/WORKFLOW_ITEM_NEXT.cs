using System;
using System.Collections.Generic;

namespace SisConAxs_DM.Models
{
    public partial class WorkflowItemNext
    {
        public int WfParentItemID { get; set; }
        public int WfItemNextItemID { get; set; }
        public int WfItemNextWfID { get; set; }
        public int WfItemNextType { get; set; }
        public int WfItemNextStep { get; set; }

        public virtual WorkflowItems WorkflowItem { get; set; }
        public virtual WorkflowItems WorkflowParentItem { get; set; }
    }
}
