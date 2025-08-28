using SisConAxs.Integration;
using SisConAxs.Integration.IntegrationUser;
using SisConAxs_DM.Repository;
using SisConAxs_DM.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs.Service.AzureSync
{
    internal class ServiceAzureSync
    {
        static void Main(string[] args)
        {
            try
            {
                LogManager.Log("> ServiceIntegrationAzureAD >> Iniciando la sincronización de empleados.");
                var datetimeStart = DateTime.Now;

                SetConfiguration();
                var integrationPeople = new IntegrationPeopleAzure();
                integrationPeople.SyncData();

                var timeExecution = DateTime.Now.Subtract(datetimeStart);
                LogManager.Log($"> ServiceIntegrationAzureAD >> Sincronización de empleados terminada. Operación ejecutada en {timeExecution.TotalMinutes} minutos.");
            }
            catch (Exception ex)
            {
                LogManager.Error("> ServiceIntegrationAzureAD >> Error al sincronizar", ex);
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
