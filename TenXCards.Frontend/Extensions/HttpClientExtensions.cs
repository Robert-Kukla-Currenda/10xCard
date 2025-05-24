using System.Net.Http.Json;

namespace TenXCards.Frontend.Extensions;

public static class HttpClientExtensions
{
    public static async Task<TResponse> PostAsJsonAsync<TRequest, TResponse>(
        this HttpClient client,
        string requestUri,
        TRequest request)
    {
        var response = await client.PostAsJsonAsync(requestUri, request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TResponse>())!;
    }
}
