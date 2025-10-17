using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Certificate_Management_BE.Middleware
{
    public class UserStatusMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UserStatusMiddleware> _logger;

        public UserStatusMiddleware(RequestDelegate next, ILogger<UserStatusMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip status check for authentication endpoints
            if (IsAuthenticationEndpoint(context.Request.Path))
            {
                await _next(context);
                return;
            }

            // Check if the endpoint requires authorization
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<AuthorizeAttribute>() != null)
            {
                var userId = GetUserIdFromToken(context);
                
                if (!string.IsNullOrEmpty(userId))
                {
                    try
                    {
                        // Get IUnitOfWork from DI container
                        var unitOfWork = context.RequestServices.GetRequiredService<Application.IUnitOfWork>();
                        
                        var user = await unitOfWork.UserRepository
                            .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == userId);

                        if (user != null && user.Status != AccountStatus.Active)
                        {
                            _logger.LogWarning("User {UserId} attempted to access protected endpoint with status {Status}", 
                                userId, user.Status);

                            context.Response.StatusCode = 403;
                            context.Response.ContentType = "application/json";
                            
                            var errorResponse = new
                            {
                                success = false,
                                message = "Your account is not active. Please contact administrator.",
                                status = user.Status.ToString()
                            };

                            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(errorResponse));
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error checking user status for user {UserId}", userId);
                        // Continue to next middleware if there's an error checking status
                    }
                }
            }

            await _next(context);
        }

        private static bool IsAuthenticationEndpoint(PathString path)
        {
            var pathValue = path.Value?.ToLowerInvariant();
            return pathValue != null && (
                pathValue.Contains("/auth/login") ||
                pathValue.Contains("/auth/forgot-password") ||
                pathValue.Contains("/auth/reset-password") ||
                pathValue.Contains("/swagger") ||
                pathValue.Contains("/hubs")
            );
        }

        private static string? GetUserIdFromToken(HttpContext context)
        {
            try
            {
                var token = GetTokenFromRequest(context);
                if (string.IsNullOrEmpty(token))
                    return null;

                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);
                
                return jsonToken.Claims.FirstOrDefault(c => c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier)?.Value;
            }
            catch
            {
                return null;
            }
        }

        private static string? GetTokenFromRequest(HttpContext context)
        {
            // Check Authorization header
            var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
            if (authHeader?.StartsWith("Bearer ") == true)
            {
                return authHeader.Substring("Bearer ".Length).Trim();
            }

            // Check query string for SignalR
            var accessToken = context.Request.Query["access_token"].FirstOrDefault();
            if (!string.IsNullOrEmpty(accessToken))
            {
                return accessToken;
            }

            return null;
        }
    }
}



