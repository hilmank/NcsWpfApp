using System.Configuration;

namespace Ncs.WpfApp.Helpers
{
    public class ConfigurationHelper
    {
        public static string GetApiBaseUrl()
        {
            return ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:4000";
        }
        public static string GetApiVersion() => ConfigurationManager.AppSettings["ApiVersion"] ?? "/api/v1";
    }
}
