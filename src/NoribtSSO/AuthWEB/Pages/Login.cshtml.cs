using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace AuthWEB.Pages
{
    public class Login : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;
        public string JwtToken { get; set; } = string.Empty;

        public void OnGet()
        {
            // Initialize page for GET request
        }

        public Login(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validate model state
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please correct the validation errors below.";
                return Page();
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "Username is required.";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Password is required.";
                return Page();
            }

            // Validate password length
            if (Password.Length < 6)
            {
                ErrorMessage = "Password must be at least 6 characters long.";
                return Page();
            }

            try
            {
                // Convert password to SHA-256 hash
                byte[] passwordHash = HashPasswordToSHA256(Password);

                // Send login request to NorbitAPI backend
                JwtToken = await AuthenticateUserAsync(Username, passwordHash);

                if (string.IsNullOrEmpty(JwtToken))
                {
                    ErrorMessage = "Invalid username or password.";
                    return Page();
                }

                // Store JWT token in session or cookie for future use
                HttpContext.Session.SetString("JwtToken", JwtToken);

                // Also set cookie for convenience
                HttpContext.Response.Cookies.Append("JwtToken", JwtToken, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(2) // Token expires in 2 minutes
                });

                SuccessMessage = $"Login successful! Welcome, {Username}. Your JWT token has been securely stored.";
                
                // Clear the form
                Username = string.Empty;
                Password = string.Empty;

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred during login: {ex.Message}";
                return Page();
            }
        }

        /// <summary>
        /// Sends authentication request to NorbitAPI backend and retrieves JWT token
        /// </summary>
        /// <param name="login">The user's login/username</param>
        /// <param name="passwordHash">The SHA-256 hashed password as byte array</param>
        /// <returns>JWT token as string, or empty string if authentication fails</returns>
        private async Task<string> AuthenticateUserAsync(string login, byte[] passwordHash)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                // Get API base URL from configuration
                var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
        
                // Convert byte array to base64 string for transmission via query string
                var passwordBase64 = Convert.ToBase64String(passwordHash);
        
                // Build the URL with properly encoded query parameters
                var loginEncoded = Uri.EscapeDataString(login);
                var passwordEncoded = Uri.EscapeDataString(passwordBase64);
                var apiEndpoint = $"{apiBaseUrl}/api/Users/login?login={loginEncoded}&passwordBase64={passwordEncoded}";

                // Send GET request to backend login endpoint
                var response = await client.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    // Read the JWT token from response content
                    var content = await response.Content.ReadAsStringAsync();
            
                    if (!string.IsNullOrEmpty(content))
                    {
                        // Remove quotes if the token is wrapped in JSON quotes
                        string token = content.Trim('"');
                        return token;
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Invalid credentials
                    return string.Empty;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Backend API returned {response.StatusCode}: {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Failed to connect to authentication service: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during authentication: {ex.Message}", ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// Converts a plain text password to SHA-256 hash
        /// </summary>
        /// <param name="password">The plain text password to hash</param>
        /// <returns>Byte array containing the SHA-256 hash</returns>
        private byte[] HashPasswordToSHA256(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }
}