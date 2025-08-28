using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs.Integration
{
    public interface IIntegrationIcarusAccess
    {
        string GetName();
        void SyncAccess();
        void SyncResource(AccessUserDTO accessUser);
        void SavePeopleAccess(WorkflowExecution wfExec, AccessRequestDetails requestDetail, Action<string, AccessRequestDetails> OnSuccess, Action<IntegrationResourceException> OnError);
        void DeletePeopleAccess(WorkflowExecution wfExec, AccessRequestDetails requestDetail, Action<string, AccessRequestDetails> OnSuccess, Action<IntegrationResourceException> OnError);
    }
}
