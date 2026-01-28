using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace UserApp.Services
{
    public class TokenManager
    {
        private readonly string _tokenFilePath;
        private readonly string _appDataFolder;

        public TokenManager()
        {
            _appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NorbitSSO");
            _tokenFilePath = Path.Combine(_appDataFolder, "token.dat");
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(_appDataFolder))
                Directory.CreateDirectory(_appDataFolder);
        }

        /// <summary>
        /// Saves JWT token securely to local storage
        /// </summary>
        public void SaveToken(string token)
        {
            try
            {
                // Simple encryption for token storage (optional but recommended)
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
        /// Retrieves JWT token from local storage
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
        /// Clears stored token (logout)
        /// </summary>
        public void ClearToken()
        {
            try
            {
                if (File.Exists(_tokenFilePath))
                    File.Delete(_tokenFilePath);
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