using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SisConAxs_DM.Models;
using SisConAxs_DM.DTO;
using Newtonsoft.Json;
using SisConAxs_DM.Repository;

namespace SisConAxs.Integration
{
    public class IntegrationResourceSRA : IIntegrationResource<ResponseAccessPeopleSRA>
    {
        public const int ACTION_ID = 2;

        public const int ERROR_NOT_FOUND = 0;       // recurso no encontrado
        public const int ERROR_HAS_RESOURSE = 0;    // ya tiene asignado el recurso
        public const int ERROR_HASNT_RESOURCE = 0;  // no tiene el recurso para dar de baja
        public const int ERROR_SAVE = 0;            // error al grabar
        public const int ERROR_SERVER = 0;          // error del servidor

        const string API_ADDRESS = "http://sra.gym.com.pe/api";

        public string GetName()
        {
            return "SRA";
        }

        public void SyncMetadata()
        {
            throw new NotImplementedException();
        }

        public List<ResponseAccessPeopleSRA> GetPeopleAccess()
        {
            throw new NotImplementedException();
        }
        public List<ResponseAccessPeopleSRA> GetPeopleAccess(string companyID)
        {
            throw new NotImplementedException();
        }

        public List<ResponseAccessPeopleSRA> GetPeopleAccess(string companyID, string peopleID)
        {
            throw new NotImplementedException();
        }

        public void SyncResources()
        {
            throw new NotImplementedException();
        }

        public void SyncPeopleAccess()
        {
            throw new NotImplementedException();
        }

        public void SavePeopleAccess(WorkflowExecution wfExec, AccessRequestDetails requestDetail, Action<string, AccessRequestDetails> OnSuccess, Action<IntegrationResourceException> OnError)
        {
            throw new NotImplementedException();
        }

        public void DeletePeopleAccess(WorkflowExecution wfExec, AccessRequestDetails requestDetail, Action<string, AccessRequestDetails> OnSuccess, Action<IntegrationResourceException> OnError)
        {
            throw new NotImplementedException();
        }
    }


    public class ResponseResponsabilitySRA
    {
        public int ResponsabilityID { get; set; }
        public string ResponsabilityCode { get; set; }
        public string ResponsabilityName { get; set; }
        public int ResponsabilityStatus { get; set; }
        public int ResponsabilityCompanyID { get; set; }
        public string ResponsabilityCompanyCode { get; set; }
        public string ResponsabilityCompanyName { get; set; }
    }

    public class ResponseAccessPeopleSRA
    {
        public string CompanyID { get; set; }
        public string PeopleID { get; set; }
        public string PerfilID { get; set; }
    }

    class PayloadResourceSRA
    {
        public string CompanyID { get; set; }
        public string PeopleID { get; set; }
        public string PerfilID { get; set; }
        public string Action { get; set; }

        // PARA CAMBIAR, ESTO ES UN EJEMPLO!!!!
        public static PayloadResourceSRA FromRequestDetail(AccessRequestDetails detail)
        {
            var payload = new PayloadResourceSRA();
            payload.CompanyID = detail.AccessRequest.RequestCompany.ToString();
            payload.PeopleID = detail.AccessRequest.RequestTo.ToString();
            payload.PerfilID = detail.ResourceID.ToString();
            return payload;
        }
    }
}
