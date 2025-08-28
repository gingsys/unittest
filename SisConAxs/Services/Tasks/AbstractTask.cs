using SisConAxs_DM.DTO;
using SisConAxs_DM.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace SisConAxs.Services
{
    public abstract class AbstractTask
    {
        protected TaskManager Manager;
        protected WorkflowExecutionRepository WFExecuteRepository = new WorkflowExecutionRepository();
        protected Timer Timer;
        protected int Lapse = 5000;  // milisecs
        public bool Initialized { get { return this._Initialized; } }
        private bool _Initialized = false;

        public AbstractTask(TaskManager manager)
        {
            this.Manager = manager;
        }

        public void Initialize()
        {
            Timer = new Timer(this.Execute, null, 0, Timeout.Infinite);
            _Initialized = true;
            LogManager.Log(String.Format("> {0} iniciado!", this.GetType().Name));
        }

        public abstract void Execute(object state);

        public string GetCurrentDateStr()
        {
            return DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        public DateTime GetExpireDate(DateTime execExpireDate, int companyID, int? dueTime, int? dueUnits, bool applyExclusionDay = false)
        {
            // Convetimos dias a minutos
            switch (dueUnits)
            {
                case (int)WfExecDueUnit.Minutes: break;
                case (int)WfExecDueUnit.Hours: dueTime = dueTime * 60; break;
                case (int)WfExecDueUnit.Days: dueTime = (dueTime * 24) * 60; break;
            }            
            DateTime wfExecExpireDate = execExpireDate.AddMinutes((int)dueTime);
            
            if (applyExclusionDay && dueUnits != (int)WfExecDueUnit.Days)
                return wfExecExpireDate;

            bool weekend = false;
            DayOfWeek dayExpire = wfExecExpireDate.DayOfWeek;
            switch (dayExpire)
            {
                case DayOfWeek.Saturday: wfExecExpireDate = wfExecExpireDate.AddDays(2); weekend = true; break;
                case DayOfWeek.Sunday: wfExecExpireDate = wfExecExpireDate.AddDays(1); weekend = true; break;
            }

            if (!weekend) { goto Isholiday; }

        Isholiday:
            var company = new CompanyRepository().GetCompanyById(companyID);
            bool success = new SystemConfigRepository().ValidateHoliday(wfExecExpireDate, company.CompanyCountry);
            if (success)
            {
                wfExecExpireDate = wfExecExpireDate.AddDays(1);
                goto Isholiday;
            }

            dayExpire = wfExecExpireDate.DayOfWeek;
            switch (dayExpire)
            {
                case DayOfWeek.Saturday: wfExecExpireDate = wfExecExpireDate.AddDays(2); break;
                case DayOfWeek.Sunday: wfExecExpireDate = wfExecExpireDate.AddDays(1); break;
            }

            return wfExecExpireDate;
        }
    }
}