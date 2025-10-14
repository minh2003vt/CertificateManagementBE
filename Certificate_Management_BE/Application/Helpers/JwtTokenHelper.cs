using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public class JwtTokenHelper
    {
        private readonly IConfiguration _configuration;
        private readonly string? _secretKey;
        private readonly string? _issuer;
        private readonly string? _audience;

        public JwtTokenHelper(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _secretKey = _configuration["Jwt:Key"];
            _issuer = _configuration["Jwt:Issuer"];
            _audience = _configuration["Jwt:Audience"];
        }

        public string GenerateToken(User user, IList<string> roles)
        {
            if (string.IsNullOrEmpty(_secretKey))
            {
                throw new InvalidOperationException("JWT SecretKey is not configured.");
            }
            if (string.IsNullOrEmpty(_issuer))
            {
                throw new InvalidOperationException("JWT Issuer is not configured.");
            }
            if (string.IsNullOrEmpty(_audience))
            {
                throw new InvalidOperationException("JWT Audience is not configured.");
            }

            if (user == null) throw new ArgumentNullException(nameof(user));
            if (roles == null || roles.Count == 0) throw new ArgumentException("Roles must not be null or empty.", nameof(roles));

            var claims = new List<Claim>
        {
            new(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Email, user.Email)
        };

            foreach (var role in roles)
            {
                if (string.IsNullOrEmpty(role)) continue;
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var secretKeyBytes = Encoding.UTF8.GetBytes(_secretKey);
            // Ensure the key is at least 32 bytes for HMAC-SHA256
            if (secretKeyBytes.Length < 32)
            {
                // Pad with zeros if too short
                Array.Resize(ref secretKeyBytes, 32);
            }
            else if (secretKeyBytes.Length > 32)
            {
                // Truncate if too long
                Array.Resize(ref secretKeyBytes, 32);
            }
            var secretKey = new SymmetricSecurityKey(secretKeyBytes);
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(50),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GeneratePasswordResetToken(string userId, string email)
        {
            if (string.IsNullOrEmpty(_secretKey))
            {
                throw new InvalidOperationException("JWT SecretKey is not configured.");
            }
            if (string.IsNullOrEmpty(_issuer))
            {
                throw new InvalidOperationException("JWT Issuer is not configured.");
            }
            if (string.IsNullOrEmpty(_audience))
            {
                throw new InvalidOperationException("JWT Audience is not configured.");
            }

            var claims = new List<Claim>
            {
                new(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.NameIdentifier, userId),
                new(ClaimTypes.Email, email),
                new("purpose", "password-reset")
            };

            var secretKeyBytes = Encoding.UTF8.GetBytes(_secretKey);
            // Ensure the key is at least 32 bytes for HMAC-SHA256
            if (secretKeyBytes.Length < 32)
            {
                // Pad with zeros if too short
                Array.Resize(ref secretKeyBytes, 32);
            }
            else if (secretKeyBytes.Length > 32)
            {
                // Truncate if too long
                Array.Resize(ref secretKeyBytes, 32);
            }
            var secretKey = new SymmetricSecurityKey(secretKeyBytes);
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5), // 5 minutes expiry
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidatePasswordResetToken(string token)
        {
            if (string.IsNullOrEmpty(_secretKey))
            {
                throw new InvalidOperationException("JWT SecretKey is not configured.");
            }

            var secretKeyBytes = Encoding.UTF8.GetBytes(_secretKey);
            // Ensure the key is at least 32 bytes for HMAC-SHA256
            if (secretKeyBytes.Length < 32)
            {
                // Pad with zeros if too short
                Array.Resize(ref secretKeyBytes, 32);
            }
            else if (secretKeyBytes.Length > 32)
            {
                // Truncate if too long
                Array.Resize(ref secretKeyBytes, 32);
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
                ClockSkew = TimeSpan.Zero // No tolerance for expiration
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                // Check if token has password-reset purpose
                var purposeClaim = principal.FindFirst("purpose");
                if (purposeClaim?.Value != "password-reset")
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
