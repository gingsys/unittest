using System.Configuration;

namespace SisConAxs.Integration.Helpers.AzureAuthentication
{
    internal class Settings
    {
        public const string ResourceUrl = "https://graph.windows.net";
        public static string[] Scopes => new[] { "https://graph.microsoft.com/.default" };

        public static string TenantId => ConfigurationManager.AppSettings["AzureTenantId"];
        public static string TenantName => ConfigurationManager.AppSettings["AzureTenantName"];
        public static string AuthString => "https://login.microsoftonline.com/" + TenantName;

        // client settings
        public static string ClientId => ConfigurationManager.AppSettings["AzureClientId"];
        public static string ClientSecret => ConfigurationManager.AppSettings["AzureClientSecret"];

        // user settings
        public static string Username => ConfigurationManager.AppSettings["AzureUsername"];
        public static string Password => ConfigurationManager.AppSettings["AzurePassword"];
    }
}
