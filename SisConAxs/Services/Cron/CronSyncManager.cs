using CronNET;
using SisConAxs.Integration;
using SisConAxs_DM.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace SisConAxs.Services.Cron
{
    class CronSyncManager
    {
        private static readonly CronDaemon CronDaemon = new CronDaemon();

        public static void Start()
        {
            try
            {
                //var mailer = new NotificationMailer { SendTo = "alex.chavez07@hotmail.com", Subject = "ICARUS TEST Cron", Body = "Ejemplo ejecutado desde Cron." };
                //mailer.SendNotificationMail();
                CronDaemon.AddJob(ServiceIntegrationOracle.CronExpression, ServiceIntegrationOracle.Execute);
                CronDaemon.AddJob(ServiceIntegrationSAP.CronExpression, ServiceIntegrationSAP.Execute);
                //CronDaemon.AddJob(ServiceIntegrationAzure.CronExpression, ServiceIntegrationAzure.Execute);
                //CronDaemon.AddJob(ServiceIntegrationSRA.CronExpression, ServiceIntegrationSRA.Execute);
                CronDaemon.AddJob(ServiceIntegrationIcarusAccess.CronExpression, ServiceIntegrationIcarusAccess.Execute);
                CronDaemon.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                LogManager.Error("> CronSyncManager >> Error al sincronizar", ex);
            }
        }

        public static void Stop()
        {
            CronDaemon.Stop();
        }
    }
}