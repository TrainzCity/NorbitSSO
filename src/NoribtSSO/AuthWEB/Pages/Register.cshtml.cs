using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;
using AuthWEB.Model;

namespace AuthWEB.Pages
{
    public class Register : PageModel
    {
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

        public IActionResult OnPost()
        {
            // Validate model state
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please correct the validation errors below.";
                return Page();
            }

            // Validate password confirmation
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return Page();
            }

            // Validate password length
            if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
            {
                ErrorMessage = "Password must be at least 6 characters long.";
                return Page();
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(User.Surname))
            {
                ErrorMessage = "Last Name is required.";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(User.Name))
            {
                ErrorMessage = "First Name is required.";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(User.Phone))
            {
                ErrorMessage = "Phone Number is required.";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(User.Login))
            {
                ErrorMessage = "Username is required.";
                return Page();
            }

            // Validate phone number format (12 digits)
            if (User.Phone.Length != 12 || !User.Phone.All(char.IsDigit))
            {
                ErrorMessage = "Phone number must be exactly 12 digits.";
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
                        ErrorMessage = "Invalid email format.";
                        return Page();
                    }
                }
                catch
                {
                    ErrorMessage = "Invalid email format.";
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
                // Here you would save the user to the database
                // Example: await _context.Users.AddAsync(User);
                // await _context.SaveChangesAsync();

                SuccessMessage = $"Registration successful! Welcome, {User.Name} {User.Surname}. Your username is: {User.Login}";
                
                // Clear the form
                User = new();
                Password = string.Empty;
                ConfirmPassword = string.Empty;

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred during registration: {ex.Message}";
                return Page();
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