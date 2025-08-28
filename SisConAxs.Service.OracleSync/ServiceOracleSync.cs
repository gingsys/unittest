using SisConAxs.Integration;
using SisConAxs_DM.Repository;
using SisConAxs_DM.Utils;
using System;

namespace SisConAxs.Service.OracleSync
{
    internal class ServiceOracleSync
    {
        static void Main(string[] args)
        {
            try
            {
                LogManager.Log("> ServiceIntegrationOracle >> Sincronización iniciada.");
                var datetimeStart = DateTime.Now;

                SetConfiguration();
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

        static void SetConfiguration()
        {
            SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);
            EntityMappings dtoMappings = new EntityMappings();
            dtoMappings.Initialize();
        }
    }
}
