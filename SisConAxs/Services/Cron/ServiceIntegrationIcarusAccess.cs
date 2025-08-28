using SisConAxs.Integration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SisConAxs.Services.Cron
{
    public class ServiceIntegrationIcarusAccess
    {
        public static string CronExpression = ConfigurationManager.AppSettings["ScheduleIntegracionIcarusAccess"];

        public static void Execute()
        {
            SyncAll();
        }
        private static void SyncAll()
        {
            try
            {
                LogManager.Log("> ServiceIntegrationIcarusAccess >> Iniciando la sincronización de accesos Icarus.");
                var datetimeStart = DateTime.Now;

                var integracionResourceIcarusAccess = new IntegrationResourceIcarusAccess();
                integracionResourceIcarusAccess.SyncAccess();

                var timeExecution = DateTime.Now.Subtract(datetimeStart);
                LogManager.Log($"> ServiceIntegrationIcarusAccess >> Sincronización de accesos Icarus terminada. Operación ejecutada en {timeExecution.TotalMinutes} minutos.");
            }
            catch (Exception ex)
            {
                LogManager.Error("> ServiceIntegrationIcarusAccess >> Error al sincronizar", ex);
            }
        }
    }
}