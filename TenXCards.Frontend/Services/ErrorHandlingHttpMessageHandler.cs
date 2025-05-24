using System.Net;
using System.Net.Http.Json;

namespace TenXCards.Frontend.Services;

public class ErrorHandlingHttpMessageHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                if (error != null)
                {
                    throw new ApiException(error.Message, response.StatusCode);
                }
                
                throw new ApiException(
                    response.StatusCode switch
                    {
                        HttpStatusCode.Unauthorized => "You are not authorized to perform this action",
                        HttpStatusCode.NotFound => "The requested resource was not found",
                        HttpStatusCode.BadRequest => "Invalid request",
                        _ => "An unexpected error occurred"
                    },
                    response.StatusCode
                );
            }

            return response;
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException("Unable to reach the server. Please check your internet connection.", HttpStatusCode.ServiceUnavailable);
        }
    }
}

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string[]>? Errors { get; set; }
}

public class ApiException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public ApiException(string message, HttpStatusCode statusCode) 
        : base(message)
    {
        StatusCode = statusCode;
    }
}
