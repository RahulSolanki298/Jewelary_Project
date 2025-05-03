using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;

namespace AdminDashboard.Server.Helper
{
    public static class JwtParser
    {
        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            if (string.IsNullOrWhiteSpace(jwt))
                throw new ArgumentException("Invalid JWT token.", nameof(jwt));

            var parts = jwt.Split('.');
            if (parts.Length != 3)
                throw new ArgumentException("JWT is not in a valid format.");

            var payload = parts[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            var claims = new List<Claim>();
            if (keyValuePairs != null)
            {
                foreach (var kvp in keyValuePairs)
                {
                    claims.Add(new Claim(kvp.Key, kvp.Value.ToString()));
                }
            }

            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
                case 1: base64 += "==="; break;
            }

            return Convert.FromBase64String(base64);
        }
    }

}
