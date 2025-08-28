using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using SisConAxs_DM.Repository;
using SisConAxs.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace SisConAxs.Services.Cron
{
    public class ServiceIntegrationSAP
    {
        public static string CronExpression = ConfigurationManager.AppSettings["ScheduleIntegracionSAP"].ToString();

        public static void Execute()
        {
            SyncAll();
        }

        private static void SyncAll()
        {
            try
            {
                LogManager.Log("> ServiceIntegrationSAP >> Iniciando la sincronización de empleados.");
                var datetimeStart = DateTime.Now;
                
                IIntegrationPeople integrationPeople = new IntegrationPeopleSAP();
                integrationPeople.SyncPeopleDeactivate();
                System.Threading.Thread.Sleep(60000);
                integrationPeople.SyncPeopleActivate();
                //integrationPeople.SyncPeopleModify();

                var timeExecution = DateTime.Now.Subtract(datetimeStart);
                LogManager.Log($"> ServiceIntegrationSAP >> Sincronización de empleados terminada. Operación ejecutada en {timeExecution.TotalMinutes} minutos.");
            }
            catch (Exception ex)
            {
                LogManager.Error("> ServiceIntegrationSAP >> Error al sincronizar", ex);
            }
        }
    }
}