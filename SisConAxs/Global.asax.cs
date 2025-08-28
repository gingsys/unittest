using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SisConAxs.Services;
using SisConAxs.Session;


namespace SisConAxs
{
    using SisConAxs.Services.Cron;
    using SisConAxs_DM.Repository;
    using SisConAxs_DM.Utils;
    using System.Configuration;
    using System.Net;
    using System.Web.Caching;

    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            //WebApiConfig.Register(GlobalConfiguration.Configuration);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            EntityMappings dtoMappings = new EntityMappings();
            dtoMappings.Initialize();

            CronSyncManager.Start();                            // Inicia el Cron  -- esto se comenta para desactivar la sincronización
            RequestEmailDataStorage.Initialize();               // Inicia el almacenamiento de EmailData
            TaskManager taskManager = new TaskManager();        // Inicia el manejador de tareas
            SessionManager session = SessionManager.Instance;   // Inicia el manejador de sesiones
            
#if RELEASE
            ServerWakeUp.RegisterCacheEntry();                  // Mantiene la aplicación activa
#endif

            //ServiceIntegrationOracle.SyncAll();               // Usado sólo para ejecutar la sincronizacion Oracle al levantar la solucion y realizar la depuración del código, se desactiva por defecto
        }

        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    Exception ex = Server.GetLastError();

        //    // Handle HTTP errors
        //    if (ex.GetType() == typeof(HttpException))
        //    {
        //        if (ex.Message.Contains("NoCatch") || ex.Message.Contains("maxUrlLength"))
        //            return;

        //        LogManager.Error("Error en HTTP", ex);
        //    }
        //    else 
        //    {
        //        LogManager.Error("Error de Aplicación", ex);
        //    }
        //    Server.ClearError();
        //}
    }


    // Server WakeUp - Singleton class
    public class ServerWakeUp
    {
        static private string DummyCacheItemKey = "__CacheDimmyWakeUp__";
        static private string DummyPageUrl = "";

        private ServerWakeUp()
        {
        }
        static public bool RegisterCacheEntry()
        {
            if (null != HttpContext.Current.Cache[DummyCacheItemKey]) return false;
            if (String.IsNullOrEmpty(DummyPageUrl))
            {
                var webHostConf = new SystemConfigRepository().GetWebHostConfig();
                DummyPageUrl = $"{webHostConf.NotifConfHost}/api/Home/server-wakeup";
                //DummyPageUrl = "http://localhost:23457/api/Home/server-wakeup";
            }

            HttpContext.Current.Cache.Add(
                DummyCacheItemKey,
                "Test",
                null,
                DateTime.MaxValue, TimeSpan.FromMinutes(10),
                CacheItemPriority.Normal,
                new CacheItemRemovedCallback(CacheItemRemovedCallback));

            //// Test WakeUp server
            //var mailer = new NotificationMailer
            //{
            //    SendTo = ConfigurationManager.AppSettings["SupportAccount"].ToString(),
            //    Subject = "Test WakeUp",
            //    Body = "Prueba de application pool activo"
            //};
            //mailer.SendNotificationMail();

            return true;
        }
        static private void CacheItemRemovedCallback(string key, object value, CacheItemRemovedReason reason)
        {
            var client = WebRequest.Create(DummyPageUrl);
            client.Timeout = 5 * 60 * 1000;
            client.GetResponse();

            // Do the service works
            // DoWork();
        }
    }
}