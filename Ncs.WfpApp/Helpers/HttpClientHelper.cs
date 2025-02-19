using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ncs.WpfApp.Helpers
{
    public static class HttpClientHelper
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri(ConfigurationHelper.GetApiBaseUrl()) // Set your base API URL
        };

        private static void SetAuthorizationHeader()
        {
            string token = SessionManager.IsAdmin ? SessionManager.AdminToken :
                           SessionManager.IsCustomer ? SessionManager.CustomerToken :
                           null;

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public static async Task<HttpResponseMessage> GetAsync(string url)
        {
            SetAuthorizationHeader();
            return await _httpClient.GetAsync(url);
        }

        public static async Task<HttpResponseMessage> PostAsync<T>(string url, T data)
        {
            SetAuthorizationHeader();
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(url, content);
        }

        public static async Task<HttpResponseMessage> PutAsync<T>(string url, T data)
        {
            SetAuthorizationHeader();
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _httpClient.PutAsync(url, content);
        }

        public static async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            SetAuthorizationHeader();
            return await _httpClient.DeleteAsync(url);
        }
    }
}
