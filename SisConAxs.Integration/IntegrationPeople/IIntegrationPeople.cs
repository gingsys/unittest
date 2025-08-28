using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs.Integration
{
    internal enum IntegrtionPeopleAction
    {
        TYPE_ACTIVATE = 1,
        TYPE_DEACTIVATE = 2,
        TYPE_MODIFY = 3
    }

    public interface IIntegrationPeople
    {
        void SyncPeopleActivate();
        void SyncPeopleDeactivate();        
        void SyncPeopleModify();

        void SyncUpdateFromServer();
    }
    public interface IIntegrationPeople<A> : IIntegrationPeople
    {
        List<A> GetPeopleMovement(DateTime startDate, DateTime endDate, string type);
    }
}