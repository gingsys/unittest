using System;
using System.Collections.Generic;

namespace SisConAxs_DM.DTO
{
    public enum WfItemDestType
    {
        Aprobador = 1,
        Solicitante,
        SolicitadoPara,
        Otro,
        Ejecutor
    }

    public class WorkflowItemsDTO
    {
        public const int TYPE_NOTIFICATION = 37;
        public const int TYPE_ASK = 38;
        public const int TYPE_ACTION = 39;

        public const int ACTION_TYPE_CHANGE_APPROVER = 1;
        public const int ACTION_TYPE_EXECUTE_IN_SERVER = 2;

        public int WfItemId { get; set; }
        public int WfItemWfID { get; set; }
        public string WfItemName { get; set; }
        public int WfItemType { get; set; }
        public string WfItemTypeName { get; set; }
        public string WfItemSubject { get; set; }
        public string WfItemMessage { get; set; }
        public int WfItemStep { get; set; }
        public Nullable<int> WfItemDestType { get; set; }
        public string WfItemDestMail { get; set; }
        public Nullable<int> WfItemCcType { get; set; }
        public string WfItemCcMail { get; set; }
        public Nullable<int> WfItemNextItem { get; set; }
        public Nullable<int> WfItemApproveItem { get; set; }
        public Nullable<int> WfItemRejectItem { get; set; }
        public Nullable<int> WfItemTimeoutItem { get; set; }
        public Nullable<int> WfItemTimeoutDueTime { get; set; }
        public Nullable<int> WfItemTimeoutDueUnits { get; set; }
        public Nullable<int> WfItemActionProperty { get; set; }
        public Nullable<int> WfItemActionValue { get; set; }
        public string WfItemEnterCondition { get; set; }
        public string WfItemEnterParams { get; set; }
        public Nullable<int> WfItemPrevSibling { get; set; }
        public string WfItemExitValues { get; set; }

        public virtual ICollection<WorkflowItemNextDTO> WorkflowItemNext { get; set; }
        public virtual ICollection<WorkflowItemNextDTO> WorkflowItemNextParents { get; set; }
    }
}
