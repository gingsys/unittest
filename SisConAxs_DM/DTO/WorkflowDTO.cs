using System;
using System.Collections.Generic;

namespace SisConAxs_DM.DTO
{
    public enum WfExecStatus
    {
        Pending,            /// 0: Pendiente nada se ha ejecutado todavia
        ToProcess,          /// 1: Listo para procesar
        WaitingResponse,    /// 2: Esperando Respuesta en una Consulta
        Responded,          /// 3: Respuesta recibida
        CancelRequest,          /// 4: Cancelación de solicitud
        FinishedApproved = 10,      /// 10: Ejecución terminada como Aprobado
        FinishedRejected = 20,      /// 20: Ejecución terminada como Rechazado
        FinishedTimeout = 30,      /// 30: Ejecución terminada como Timeout
        FinishedUndefinied = 90       /// 90: Ejecución terminada en estado indefinido!  
    }

    public enum WfExecResponse
    {
        Pending = -1,           /// -1: Esperando por una respuesta
        Rejected = 0,           /// 0: Respuesta de Rechazado
        Approved = 1,           /// 1: Respuesta de Aprobado
        Timeout = 2             /// 2: Sin Respuesta Timeout
    }

    public enum WfExecDueUnit
    {
        Minutes,
        Hours = 1,          /// 1: Unidad de Tiempo Horas
        Days                /// 2: Unidad de Tiempo Dias
    }

    public enum WfExecNextItem
    {
        Next = 1,           /// 0: Siguiente Item
        Approved,           /// 1: Siguiente Item de tipo Aprobado
        Rejected,           /// 2: Siguiente Item de tipo Rechazado
        Timeout             /// 3: Siguiente Item de tipo Timeout
    }

    public class WorkflowDTO
    {
        public WorkflowDTO()
        {
            this.WorkflowItems = new List<WorkflowItemsDTO>();
        }

        public int WfID { get; set; }
        public Nullable<int> WfApproveHierarchyID { get; set; }
        public string WfApproveHierarchyName { get; set; }
        public string WfName { get; set; }
        public string WfDescription { get; set; }
        public int WfActivo { get; set; }
        //public int WfCompany { get; set; }

        public virtual ICollection<WorkflowItemsDTO> WorkflowItems { get; set; }
    }
}
