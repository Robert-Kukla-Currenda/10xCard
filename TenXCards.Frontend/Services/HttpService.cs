using System.Net.Http.Json;
using System.Text.Json;

namespace TenXCards.Frontend.Services
{
    public interface IHttpService
    {
        Task<T?> GetAsync<T>(string endpoint);
        Task<HttpResponseMessage> PostAsJsonAsync(string endpoint, object data);
        Task<HttpResponseMessage> PutAsJsonAsync(string endpoint, object data);
        Task<HttpResponseMessage> DeleteAsync(string endpoint);
        void RemoveAuthorizations();
    }

    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly JsonSerializerOptions _jsonOptions;

        public HttpService(
            HttpClient httpClient,
            ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }

        public async Task<HttpResponseMessage> PostAsJsonAsync(string endpoint, object data)
        {
            await AddAuthorizationHeader();
            return await _httpClient.PostAsJsonAsync(endpoint, data);
        }

        public async Task<HttpResponseMessage> PutAsJsonAsync(string endpoint, object data)
        {
            await AddAuthorizationHeader();
            return await _httpClient.PutAsJsonAsync(endpoint, data);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            await AddAuthorizationHeader();
            return await _httpClient.DeleteAsync(endpoint);
        }

        public void RemoveAuthorizations()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        private async Task AddAuthorizationHeader()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {                
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
