using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AuthWEB.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AuthWEB.Pages
{
    public class Register : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        [BindProperty]
        public User User { get; set; } = new();

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            // Initialize page for GET request
        }
        public Register(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Fix non-Used field validation
            ModelState["User.Password"].Errors.Clear();
            ModelState["User.Password"].ValidationState = ModelValidationState.Valid;
            // Validate model state
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Пожалуйста, проверьте правильность заполнения полей формы";
                return Page();
            }

            // Validate password confirmation
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Пароли не совпадают.";
                return Page();
            }

            // Validate password length
            if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
            {
                ErrorMessage = "Пароль должен быть не менее 6 символов.";
                return Page();
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(User.Surname))
            {
                ErrorMessage = "Фамилия обязательна.";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(User.Name))
            {
                ErrorMessage = "Имя обязательно.";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(User.Phone))
            {
                ErrorMessage = "Номер телефона обязателен.";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(User.Login))
            {
                ErrorMessage = "Имя пользователя обязательно.";
                return Page();
            }

            // Validate phone number format (12 digits)
            string phoneDigits = User.Phone.Substring(1, 11);
            if (User.Phone[0] != '+' || User.Phone.Length != 12 || !phoneDigits.All(char.IsDigit))
            {
                ErrorMessage = "Неверный формат номера телефона";
                return Page();
            }

            // Validate email format if provided
            if (!string.IsNullOrWhiteSpace(User.Email))
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(User.Email);
                    if (addr.Address != User.Email)
                    {
                        ErrorMessage = "Неверный формат Email.";
                        return Page();
                    }
                }
                catch
                {
                    ErrorMessage = "Неверный формат Email.";
                    return Page();
                }
            }

            // Convert password to SHA-256 hash
            User.Password = HashPasswordToSHA256(Password);

            // Set default values
            User.Uuid = Guid.NewGuid();
            User.IsBlocked = false;

            try
            {
                // Send user data to NorbitAPI backend
                await SendUserToBackendAsync(User);

                SuccessMessage = $"Успешная регистрация! Добро пожаловать, {User.Name} {User.Surname}. Твой логин: {User.Login}";
                
                // Clear the form
                User = new();
                Password = string.Empty;
                ConfirmPassword = string.Empty;

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Во время регистрации возникла проблема: {ex.Message}";
                return Page();
            }
        }
        
        /// <summary>
        /// Sends the user data to the NorbitAPI backend via POST request
        /// </summary>
        /// <param name="user">The user object to register</param>
        /// <returns>Task representing the async operation</returns>
        private async Task SendUserToBackendAsync(User user)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                
                // Get API base URL from configuration
                var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
                var apiEndpoint = $"{apiBaseUrl}/api/Users";

                // Serialize user to JSON
                var jsonContent = JsonSerializer.Serialize(user);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Send POST request to backend
                var response = await client.PostAsync(apiEndpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Backend API returned {response.StatusCode}: {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Failed to connect to registration service: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error sending user data to backend: {ex.Message}", ex);
            }
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