using System.Text.Json;
using TenXCards.API.Models.OpenRouter;
using TenXCards.API.Exceptions;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
using TenXCards.API.Configuration;
using System.Net.Http.Json;

namespace TenXCards.API.Services.OpenRouter
{
    public class OpenRouterService : IOpenRouterService
    {
        private readonly ModelParameters _modelParameters;        
        private readonly HttpClient _httpClient;
        private readonly ILogger<OpenRouterService> _logger;
        private readonly AIServiceOptions _options;
        private readonly IErrorLoggingService _errorLoggingService;

        public OpenRouterService(
            ILogger<OpenRouterService> logger,
            HttpClient httpClient,            
            IOptions<AIServiceOptions> options,
            IErrorLoggingService errorLoggingService,
            IHttpClientFactory factory)
        {
            _logger = logger;
            _options = options.Value;
            _httpClient = httpClient;
            _errorLoggingService = errorLoggingService;
            

            _httpClient.BaseAddress = new Uri(_options.OpenRouterUrl);            
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.OpenRouterApiKey}");            


            _modelParameters = new ModelParameters
            {
                //Temperature = configuration.GetValue<double>("OpenRouter:Temperature", 0.7),
                //MaxTokens = configuration.GetValue<int>("OpenRouter:MaxTokens", 256)
            };
            
            _errorLoggingService = errorLoggingService;
        }


        public async Task<string> SendMessageAsync(Prompt prompt)
        {
            if (prompt is null || prompt.messages is null || prompt.messages.Count() == 0)
            {
                throw new ValidationException("Prompt cannot be empty");
            }

            try
            {
                var payload = new ChatPayload
                {
                    Model = _options.OpenRouterModelName,
                    Messages = prompt.messages,
                    Temperature = _modelParameters.Temperature,
                    MaxTokens = _modelParameters.MaxTokens,
                    ResponseFormat = new ResponseFormat
                    {
                        JsonSchema = new JsonSchema
                        {
                            Schema = new Dictionary<string, string> { { "result", "string" } }
                        }
                    }
                };

                //using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(_options.TimeoutSeconds));
                //using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, cancellationToken);

                var jsonSerializatonOptions = new JsonSerializerOptions
                {
                    Converters =
                    {
                        new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                    },
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };                

                var response = await _httpClient.PostAsJsonAsync("/api/v1/chat/completions", payload, jsonSerializatonOptions, CancellationToken.None);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new NetworkException(
                        $"API request failed: {response.StatusCode}",
                        new { response.StatusCode, Content = errorContent });
                }

                var r = await response.Content.ReadAsStringAsync();
                var result = await response.Content.ReadFromJsonAsync<JsonDocument>();

                return result?.RootElement.GetProperty("choices").GetString()
                    ?? throw new ValidationException("Invalid response format - missing result");                
            }
            catch (Exception ex) when (ex is not OpenRouterException)
            {
                _logger.LogError(ex, "Unexpected error during message processing");
                throw new OpenRouterException(
                    "An unexpected error occurred", "UNKNOWN_ERROR", ex.Message);
            }
        }
    }
}