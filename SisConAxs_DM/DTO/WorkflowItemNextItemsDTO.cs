using System;
using System.Collections.Generic;

namespace SisConAxs_DM.DTO
{
    public class WorkflowItemNextDTO
    {
        public int WfParentItemID { get; set; }
        public int WfItemNextItemID { get; set; }
        public Nullable<int> WfItemNextWfID { get; set; }
        public int WfItemNextType { get; set; }
        public int WfItemNextStep { get; set; }
    }
}
