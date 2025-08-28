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
    public class ServiceIntegrationAzure
    {
        public static string CronExpression = ConfigurationManager.AppSettings["ScheduleIntegracionAzureAD"].ToString();

        public static void Execute()
        {
            SyncAll();
        }

        private static async void SyncAll()
        {
            try
            {
                LogManager.Log("> ServiceIntegrationAzure >> Iniciando la sincronización de empleados.");
                var datetimeStart = DateTime.Now;

                var integrationPeople = new IntegrationPeopleAzure();
                integrationPeople.SyncData();

                var timeExecution = DateTime.Now.Subtract(datetimeStart);
                LogManager.Log($"> ServiceIntegrationAzure >> Sincronización de empleados terminada. Operación ejecutada en {timeExecution.TotalMinutes} minutos.");
            }
            catch (Exception ex)
            {
                LogManager.Error("> ServiceIntegrationAzure >> Error al sincronizar", ex);
            }
        }
    }
}