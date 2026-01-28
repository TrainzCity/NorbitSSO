using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using UserApp.Model;
using UserApp.Tools;

namespace UserApp.Services
{
    public class TokenManager
    {
        private readonly string _tokenFilePath;
        private readonly string _userInfoFilePath;
        private readonly string _appDataFolder;

        public TokenManager()
        {
            _appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NorbitSSO");
            _tokenFilePath = Path.Combine(_appDataFolder, "token.dat");
            _userInfoFilePath = Path.Combine(_appDataFolder, "userinfo.dat");

            if (!Directory.Exists(_appDataFolder))
                Directory.CreateDirectory(_appDataFolder);
        }

        /// <summary>
        /// Saves JWT token and extracts user information
        /// </summary>
        public async Task SaveToken(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    throw new ArgumentException("Token cannot be null or empty");

                // Encrypt and save token
                byte[] dataToEncrypt = Encoding.UTF8.GetBytes(token);
                byte[] encryptedData = ProtectedData.Protect(dataToEncrypt, null, DataProtectionScope.CurrentUser);
                File.WriteAllBytes(_tokenFilePath, encryptedData);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save token: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Retrieves JWT token from storage
        /// </summary>
        public string GetToken()
        {
            try
            {
                if (!File.Exists(_tokenFilePath))
                    return string.Empty;

                byte[] encryptedData = File.ReadAllBytes(_tokenFilePath);
                byte[] decryptedData = ProtectedData.Unprotect(encryptedData, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(decryptedData);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve token: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Saves user session information extracted from token claims
        /// </summary>
        public void SaveUserInfo(User userInfo)
        {
            try
            {
                var json = JsonSerializer.Serialize(userInfo);
                byte[] dataToEncrypt = Encoding.UTF8.GetBytes(json);
                byte[] encryptedData = ProtectedData.Protect(dataToEncrypt, null, DataProtectionScope.CurrentUser);
                File.WriteAllBytes(_userInfoFilePath, encryptedData);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save user info: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Retrieves user session information
        /// </summary>
        public User GetUserInfo()
        {
            try
            {
                if (!File.Exists(_userInfoFilePath))
                    return null;

                byte[] encryptedData = File.ReadAllBytes(_userInfoFilePath);
                byte[] decryptedData = ProtectedData.Unprotect(encryptedData, null, DataProtectionScope.CurrentUser);
                string json = Encoding.UTF8.GetString(decryptedData);
                return JsonSerializer.Deserialize<User>(json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve user info: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the logged-in user's login name
        /// </summary>
        public string GetCurrentUserLogin()
        {
            try
            {
                var userInfo = GetUserInfo();
                if (userInfo == null)
                    throw new InvalidOperationException("No user session found");

                return userInfo.login;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get current user login: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Checks if user is currently logged in
        /// </summary>
        public bool IsUserLoggedIn()
        {
            try
            {
                string token = GetToken();
                if (string.IsNullOrEmpty(token))
                    return false;

                return !ClaimsHelper.IsTokenExpired(token);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Clears stored token and user info (logout)
        /// </summary>
        public void ClearToken()
        {
            try
            {
                if (File.Exists(_tokenFilePath))
                    File.Delete(_tokenFilePath);

                if (File.Exists(_userInfoFilePath))
                    File.Delete(_userInfoFilePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to clear token: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Checks if token exists
        /// </summary>
        public bool HasToken()
        {
            return File.Exists(_tokenFilePath);
        }
    }
}