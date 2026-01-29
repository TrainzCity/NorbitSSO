using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using UserApp.Services;

namespace UserApp.Tools
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly TokenManager _tokenManager;
        private const string API_BASE_URL = "http://localhost:4080"; // Adjust based on your setup

        public ApiClient()
        {
            _httpClient = new HttpClient();
            _tokenManager = new TokenManager();
        }

        /// <summary>
        /// Makes authenticated API request with JWT token
        /// </summary>
        public async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                string token = _tokenManager.GetToken();
                
                if (string.IsNullOrEmpty(token))
                    throw new Exception("No authentication token found. Please log in first.");

                var request = new HttpRequestMessage(HttpMethod.Get, $"{API_BASE_URL}{endpoint}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content);
            }
            catch (Exception ex)
            {
                throw new Exception($"API request failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Makes authenticated POST request with JWT token
        /// </summary>
        public async Task<T> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                string token = _tokenManager.GetToken();
                
                if (string.IsNullOrEmpty(token))
                    throw new Exception("No authentication token found. Please log in first.");

                var request = new HttpRequestMessage(HttpMethod.Post, $"{API_BASE_URL}{endpoint}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                var jsonContent = JsonSerializer.Serialize(data);
                request.Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(responseContent);
            }
            catch (Exception ex)
            {
                throw new Exception($"API request failed: {ex.Message}", ex);
            }
        }
    }
}