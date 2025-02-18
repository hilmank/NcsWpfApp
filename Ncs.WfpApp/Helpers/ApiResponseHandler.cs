using System;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;

public class ApiResponseHandler
{
    public static async Task<(string Message, string MessageDetail)> HandleApiResponse(HttpResponseMessage response)
    {
        string responseContent = await response.Content.ReadAsStringAsync();

        string message = response.StatusCode switch
        {
            HttpStatusCode.OK => "Success",
            HttpStatusCode.BadRequest => "Bad Request: Invalid syntax in the request.",
            HttpStatusCode.Unauthorized => "Unauthorized: Authentication is required or has failed.",
            HttpStatusCode.Forbidden => "Forbidden: You do not have permission to access this resource.",
            HttpStatusCode.NotFound => "Not Found: The requested resource could not be found.",
            HttpStatusCode.InternalServerError => "Internal Server Error: Something went wrong on the server.",
            _ => $"Unexpected Error: {response.StatusCode}"
        };

        return (message, responseContent);
    }
}
