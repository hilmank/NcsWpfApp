using System.Configuration;

namespace Ncs.WpfApp.Helpers
{
    public class ConfigurationHelper
    {
        public static string GetApiBaseUrl()
        {
            return ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:5000";
        }
    }
}
