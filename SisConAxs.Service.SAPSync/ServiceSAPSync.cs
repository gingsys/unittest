using SisConAxs.Integration;
using SisConAxs_DM.Repository;
using SisConAxs_DM.Utils;
using System;

namespace SisConAxs.Service.SAPSync
{
    internal class ServiceSAPSync
    {
        static void Main(string[] args)
        {
            try
            {
                LogManager.Log("> ServiceIntegrationSAP >> Iniciando la sincronización de empleados.");
                var datetimeStart = DateTime.Now;

                SetConfiguration();
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

        static void SetConfiguration()
        {
            SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);
            EntityMappings dtoMappings = new EntityMappings();
            dtoMappings.Initialize();
        }
    }
}
