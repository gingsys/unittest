using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs.Integration.DTO
{
    public class OracleCheckResponsabilitiesConflictsPayload
    {
        public string UserName { get; set; }
        public List<string> Responsabilities { get; set; } = new List<string>();
    }

    public class OracleSaveResourcePeoplePayload
    {
        public class OracleAccess
        {
            public string AccessResponsabilityID { get; set; }
            public string AccessEndDate { get; set; }
        }

        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string UserEmployeeName { get; set; }
        public string UserEmployeeLastName { get; set; }
        public string UserCompanyID { get; set; }
        public string UserBirthday { get; set; }
        public string UserEmail { get; set; }
        public string UserType { get; set; }
        public string UserGender { get; set; }
        public string UserPassword { get; set; }
        public OracleAccess Access { get; set; }

        public OracleSaveResourcePeoplePayload()
        {
            Access = new OracleAccess();
        }

        public static OracleSaveResourcePeoplePayload FromRequestDetail(AccessRequestDetails detail)
        {
            // validación 
            var parameter = detail.AccessResources.ResourceParameters.FirstOrDefault(x => x.ResourceParameterID == AccessResourceParameterDTO.INTEGRACION_ORACLE_ID);
            if (parameter == null)
            {
                throw new IntegrationResourceException(IntegrationResourceException.ERROR_RESOURCE_WITHOUT_EXTERNAL_ID, $"> Integration.Oracle >> El recurso '{detail.AccessResources.ResourceFullName}' NO tiene vinculado un acceso del servicio ORACLE.");
            }

            var people = detail.AccessRequest.PeopleRequestTo;
            if(String.IsNullOrWhiteSpace(people.UserID))
            {
                throw new IntegrationResourceException(IntegrationResourceException.ERROR_CLIENT_VALIDATION, $"> Integration.Oracle >> La persona '{people.GetFullName()}' NO tiene un usuario válido.");
            }
            //try
            //{
            //    var addr = new System.Net.Mail.MailAddress(people.PeopleEmail);
            //}
            //catch
            //{
            //    throw new IntegrationResourceException(IntegrationResourceException.ERROR_CLIENT_VALIDATION, $"> Integration.Oracle >> La persona '{people.GetFullName()}' NO tiene un correo válido.");
            //}

            // Create PAYLOAD --------------------------------------------------------------------------
            var payload = new OracleSaveResourcePeoplePayload();
            payload.UserCode = people.PeopleInternalID;
            payload.UserName = (people.UserID ?? "").Trim();
            //if (String.IsNullOrEmpty(payload.UserName))
            //{
            //    var index = people.PeopleEmail.IndexOf("@");
            //    payload.UserName = people.PeopleEmail.Substring(0, index < 0 ? 0 : index);
            //}
            //if (String.IsNullOrEmpty(payload.UserName))
            //{
            //    payload.UserName = $"{people.PeopleLastName.ToLower().Replace(" ", "")}.{people.PeopleFirstName.ToLower().Replace(" ", "")}";
            //}

            payload.UserEmployeeName = (people.PeopleFirstName + " " + people.PeopleFirstName2).Trim();
            payload.UserEmployeeLastName = (people.PeopleLastName + " " + people.PeopleLastName2).Trim();
            payload.UserCompanyID = people.Company.CompanyTaxpayerID;
            payload.UserBirthday = String.Format("{0:dd/MM/yyyy}", people.PeopleBirthday);
            payload.UserEmail = people.PeopleEmail;
            payload.UserType = "I";   // I: interno, E: externo (proveedores)
            payload.UserGender = people.PeopleGender;
            payload.UserPassword = "";

            payload.Access.AccessResponsabilityID = parameter.Value;
            payload.Access.AccessEndDate = detail.ReqDetValidityUntil == null ? null : String.Format("{0:dd/MM/yyyy}", detail.ReqDetValidityUntil);
            return payload;
        }
    }

    public class OracleDeleteResourcePeoplePayload
    {
        public string UserCode { get; set; }
        public OracleAccess Access { get; set; }

        public class OracleAccess
        {
            public string AccessResponsabilityID { get; set; }
            public string AccessEndDate { get; set; }
        }

        public OracleDeleteResourcePeoplePayload()
        {
            Access = new OracleAccess();
        }
        public static OracleDeleteResourcePeoplePayload FromRequestDetail(AccessRequestDetails detail)
        {
            // validación 
            var parameter = detail.AccessResources.ResourceParameters.FirstOrDefault(x => x.ResourceParameterID == AccessResourceParameterDTO.INTEGRACION_ORACLE_ID);
            if (parameter == null)
            {
                throw new IntegrationResourceException(IntegrationResourceException.ERROR_RESOURCE_WITHOUT_EXTERNAL_ID, $"> Integration.Oracle >> El recurso '{detail.AccessResources.ResourceFullName}' NO tiene vinculado un acceso del servicio ORACLE.");
            }

            var payload = new OracleDeleteResourcePeoplePayload();
            payload.UserCode = detail.AccessRequest.PeopleRequestTo.PeopleInternalID;
            payload.Access.AccessResponsabilityID = parameter.Value;
            payload.Access.AccessEndDate = detail.ReqDetValidityUntil == null ? null : String.Format("{0:dd/MM/yyyy}", DateTime.Now);
            return payload;
        }
    }
}
