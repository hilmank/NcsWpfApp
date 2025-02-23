using System;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Ncs.WpfApp.Models;
using System.Text.Json;

public class ApiResponseHandler
{
    public static async Task<(string Message, string MessageDetail)> HandleApiResponse(HttpResponseMessage response)
    {
        string responseContent = await response.Content.ReadAsStringAsync();

        try
        {
            // Deserialize the JSON response into ApiResponseModel<object>
            var apiResponse = JsonSerializer.Deserialize<ApiResponseModel<object>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Allows case-insensitive property mapping
            });

            if (apiResponse != null)
            {
                // If request was not successful OR the status code is not OK, return error messages
                if (!apiResponse.Success || response.StatusCode != HttpStatusCode.OK)
                {
                    return (apiResponse.Message ?? "Unknown error", apiResponse.MessageDetail ?? "");
                }

                // Return success message
                return ("Success", "");
            }
        }
        catch (JsonException)
        {
            // Return parsing error if JSON is malformed
            return ("Error parsing response", responseContent);
        }

        return ("Unexpected error", responseContent);
    }
}
