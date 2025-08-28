using SisConAxs_DM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs.Integration
{
    public enum IntegrationResourceAction
    {
        TYPE_SUBSCRIBE = 1,
        TYPE_UNSUBSCRIBE = 2
    }

    public interface IIntegrationResource<A> : IIntegrationResource
    {
        List<A> GetPeopleAccess();
        List<A> GetPeopleAccess(string companyID);
        List<A> GetPeopleAccess(string companyID, string peopleID);
    }

    public interface IIntegrationResource
    {
        string GetName();
        void SyncMetadata();
        void SyncResources();
        void SyncPeopleAccess();

        void SavePeopleAccess(WorkflowExecution wfExec, AccessRequestDetails requestDetail, Action<string, AccessRequestDetails> OnSuccess, Action<IntegrationResourceException> OnError);
        void DeletePeopleAccess(WorkflowExecution wfExec, AccessRequestDetails requestDetail, Action<string, AccessRequestDetails> OnSuccess, Action<IntegrationResourceException> OnError);
    }
}
