using System;
using System.Collections.Generic;
using System.Web;

namespace SisConAxs_DM.DTO
{
    public class AccessRequestDTO
    {
        public const int TYPE_ALTA = 46;
        public const int TYPE_BAJA = 47;
        public const int TYPE_MODIFICACION = 48;

        public const int PRIORITY_BAJA = 49;
        public const int PRIORITY_NORMAL = 50;
        public const int PRIORITY_ALTA = 51;
        public const int PRIORITY_URGENTE = 52;

        public const int STATUS_PENDING = 40;
        public const int STATUS_APPROVE = 43;
        public const int STATUS_REJECT = 44;
        public const int STATUS_PARCIAL = 45;
        public const int STATUS_ANNUL = 53;
        public const int STATUS_PROCESS = 55;
        public const int STATUS_ATTENDED = 64;
        public const int STATUS_UNDEFINIED = 69;
        public const int STATUS_TIMEOUT = 77;

        public int RequestID { get; set; }
        public int RequestNumber { get; set; }

        public string RequestBy { get; set; }
        public string RequestByName { get; set; }
        public string RequestByProject { get; set; }
        public string RequestByDepartment { get; set; }
        public string RequestByPosition { get; set; }

        public int RequestTo { get; set; }
        public string RequestToName { get; set; }
        public string RequestToEmail { get; set; }
        public int RequestToCompany { get; set; }
        public string RequestToCompanyName { get; set; }
        public string RequestToProject { get; set; }
        public string RequestToDepartment { get; set; }
        public string RequestToPosition { get; set; }   
        
        public string RequestToNumDoc { get; set; } // para visualizarse en la solicitud

        public int RequestType { get; set; }
        public string RequestTypeName { get; set; }
        public string RequestTypeDisplay { get; set; }
        public int RequestPriority { get; set; }
        public string RequestPriorityName { get; set; }
        public Nullable<int> RequestStatus { get; set; }
        public string RequestStatusName { get; set; }
        public Nullable<int> RequestDepartment { get; set; }
        public string RequestDepartmentName { get; set; }
        public Nullable<System.DateTime> RequestDate { get; set; }
        public Nullable<System.DateTime> RequestCompletedDate { get; set; }
        public string RequestNote { get; set; } = "";
        public int RequestForApprove { get; set;  }
        public int RequestNroItems { get; set; }

        public int RequestTotalRows { get; set; }
        public HttpPostedFile FileAttached { get; set; }
        public string FileAttachedString { get; set; }
        //public int RequestCompany { get; set; }
        public string AttentionTicket { get; set; }
        public string OracleUser { get; set; }
        public string OracleMenu { get; set; }
        public ICollection<AccessRequestDetailsDTO> AccessRequestDetails { get; set; }
        public bool validateWorkflow { get; set; }
        public int FlagIsApprover { get; set; }
    }
}
