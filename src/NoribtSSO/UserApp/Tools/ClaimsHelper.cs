using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;

namespace UserApp.Tools
{
    /// <summary>
    /// Helper class to extract and manage claims from JWT tokens
    /// </summary>
    public class ClaimsHelper
    {
        /// <summary>
        /// Extracts claims from a JWT token string without validation
        /// (Use this when you already have a valid token from authentication)
        /// </summary>
        public static Dictionary<string, string> ExtractClaimsFromToken(string token)
        {
            var claims = new Dictionary<string, string>();

            try
            {
                if (string.IsNullOrEmpty(token))
                    throw new ArgumentException("Token cannot be null or empty");

                var handler = new JwtSecurityTokenHandler();
                
                // Read token without validation (token is already validated by server)
                var jwtToken = handler.ReadJwtToken(token);

                if (jwtToken == null)
                    throw new InvalidOperationException("Invalid token format");

                // Extract all claims
                foreach (var claim in jwtToken.Claims)
                {
                    claims[claim.Type] = claim.Value;
                }

                return claims;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to extract claims from token: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Extracts the login/username from JWT token claims
        /// </summary>
        public static string ExtractUuidFromToken(string token)
        {
            try
            {
                var claims = ExtractClaimsFromToken(token);

                // ClaimTypes.Name contains the login/username
                if (claims.TryGetValue(ClaimTypes.Sid, out var uuid))
                    return uuid;

                // Fallback to "uuid" if ClaimTypes.Name not found
                if (claims.TryGetValue("uuid", out var sidClaimValue))
                    return sidClaimValue;

                throw new InvalidOperationException("Login claim not found in token");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to extract login from token: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Extracts a specific claim value by type
        /// </summary>
        public static string GetClaimValue(string token, string claimType)
        {
            try
            {
                var claims = ExtractClaimsFromToken(token);

                if (claims.TryGetValue(claimType, out var value))
                    return value;

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to extract claim '{claimType}' from token: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Checks if token has expired
        /// </summary>
        public static bool IsTokenExpired(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                // Check expiration time
                return jwtToken.ValidTo <= DateTime.UtcNow;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Gets token expiration time
        /// </summary>
        public static DateTime GetTokenExpirationTime(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                return jwtToken.ValidTo;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get token expiration: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets all claims as a formatted string for debugging
        /// </summary>
        public static string GetAllClaimsAsString(string token)
        {
            try
            {
                var claims = ExtractClaimsFromToken(token);
                var claimsString = "JWT Token Claims:\n";
                
                foreach (var claim in claims)
                {
                    claimsString += $"  {claim.Key}: {claim.Value}\n";
                }

                return claimsString;
            }
            catch (Exception ex)
            {
                return $"Error retrieving claims: {ex.Message}";
            }
        }
    }
}
