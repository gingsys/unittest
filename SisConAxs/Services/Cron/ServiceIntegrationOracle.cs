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
    public class ServiceIntegrationOracle
    {
        public static string CronExpression = ConfigurationManager.AppSettings["ScheduleIntegracionOracle"];

        public static void Execute()
        {
            SyncAll();
            //SendMails();
        }

        private static void SyncAll()
        {
            try
            {
                LogManager.Log("> ServiceIntegrationOracle >> Sincronización iniciada.");
                var datetimeStart = DateTime.Now;

                IIntegrationResource integrationResource = IntegrationResourceFactory.Get(IntegrationResourceOracle.ACTION_ID);
                integrationResource.SyncMetadata();
                integrationResource.SyncResources();
                integrationResource.SyncPeopleAccess();

                var timeExecution = DateTime.Now.Subtract(datetimeStart);
                LogManager.Log($"> ServiceIntegrationOracle >> Sincronización terminada. Operación ejecutada en {timeExecution.TotalMinutes} minutos.");
            }
            catch (Exception ex)
            {
                LogManager.Error("> ServiceIntegrationOracle >> Error al sincronizar", ex);
            }
        }
    }
}