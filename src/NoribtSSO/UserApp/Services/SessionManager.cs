using System;
using System.Net.Http;
using System.Threading.Tasks;
using UserApp.Tools;

namespace UserApp.Services
{
    /// <summary>
    /// Manages session lifecycle and token validation
    /// </summary>
    public class SessionManager
    {
        private readonly TokenManager _tokenManager;
        private readonly ApiClient _apiClient;
        private const string API_BASE_URL = "http://localhost:5207";

        public SessionManager()
        {
            _tokenManager = new TokenManager();
            _apiClient = new ApiClient();
        }

        /// <summary>
        /// Performs server-side logout and clears local session
        /// </summary>
        public async Task<bool> LogoutAsync()
        {
            try
            {
                // Step 1: Verify token is still valid before logout
                if (!_tokenManager.HasToken())
                {
                    // Token already cleared locally
                    _tokenManager.ClearToken();
                    return true;
                }

                string token = _tokenManager.GetToken();

                // Step 2: Call server logout endpoint
                bool serverLogoutSuccess = await CallLogoutEndpointAsync(token);

                // Step 3: Clear local token and session regardless of server response
                // (even if server is unavailable, we should clear local session)
                _tokenManager.ClearToken();

                return serverLogoutSuccess;
            }
            catch (Exception ex)
            {
                // Even if logout endpoint fails, clear local session
                try { _tokenManager.ClearToken(); } catch { }
                
                throw new Exception($"Logout process encountered an error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Calls the logout endpoint on the API server
        /// </summary>
        private async Task<bool> CallLogoutEndpointAsync(string token)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, $"{API_BASE_URL}/api/Users/logout");
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        System.Diagnostics.Debug.WriteLine("Server logout successful");
                        return true;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Server logout returned: {response.StatusCode}");
                        return false;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to reach logout endpoint: {ex.Message}");
                // Don't throw - let it fail gracefully since client session will be cleared anyway
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error calling logout endpoint: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Validates if the current token is still valid with the server
        /// </summary>
        public async Task<bool> ValidateTokenAsync()
        {
            try
            {
                if (!_tokenManager.HasToken())
                    return false;

                string token = _tokenManager.GetToken();

                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, $"{API_BASE_URL}/api/Users/validate-token");
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var response = await client.SendAsync(request);
                    return response.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if user has an active, valid session
        /// </summary>
        public bool HasActiveSession()
        {
            try
            {
                if (!_tokenManager.HasToken())
                    return false;

                string token = _tokenManager.GetToken();
                return !ClaimsHelper.IsTokenExpired(token);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets current user login if session is active
        /// </summary>
        public string GetCurrentUserLogin()
        {
            try
            {
                if (!HasActiveSession())
                    return null;

                return _tokenManager.GetCurrentUserLogin();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Performs emergency logout (clears everything without server call)
        /// Used when user needs to logout immediately
        /// </summary>
        public void EmergencyLogout()
        {
            try
            {
                _tokenManager.ClearToken();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Emergency logout error: {ex.Message}");
            }
        }
    }
}
