using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Certificate_Management_BE.Extensions
{
    public static class UserStatusExtensions
    {
        /// <summary>
        /// Check if the current user has an active account status
        /// </summary>
        /// <param name="httpContext">The HTTP context</param>
        /// <returns>True if user is active, false otherwise</returns>
        public static async Task<bool> IsUserActiveAsync(this HttpContext httpContext)
        {
            var userId = httpContext.User.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return false;

            try
            {
                var unitOfWork = httpContext.RequestServices.GetRequiredService<Application.IUnitOfWork>();
                var user = await unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == userId);

                return user?.Status == AccountStatus.Active;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get the current user's account status
        /// </summary>
        /// <param name="httpContext">The HTTP context</param>
        /// <returns>The user's account status, or null if not found</returns>
        public static async Task<AccountStatus?> GetUserStatusAsync(this HttpContext httpContext)
        {
            var userId = httpContext.User.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return null;

            try
            {
                var unitOfWork = httpContext.RequestServices.GetRequiredService<Application.IUnitOfWork>();
                var user = await unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == userId);

                return user?.Status;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Check if user status is active and return appropriate response if not
        /// </summary>
        /// <param name="httpContext">The HTTP context</param>
        /// <returns>True if user is active, false if inactive (response already sent)</returns>
        public static async Task<bool> EnsureUserIsActiveAsync(this HttpContext httpContext)
        {
            var userId = httpContext.User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                httpContext.Response.StatusCode = 401;
                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "User not authenticated"
                }));
                return false;
            }

            try
            {
                var unitOfWork = httpContext.RequestServices.GetRequiredService<Application.IUnitOfWork>();
                var user = await unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == userId);

                if (user == null)
                {
                    httpContext.Response.StatusCode = 401;
                    httpContext.Response.ContentType = "application/json";
                    await httpContext.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
                    {
                        success = false,
                        message = "User not found"
                    }));
                    return false;
                }

                if (user.Status != AccountStatus.Active)
                {
                    httpContext.Response.StatusCode = 403;
                    httpContext.Response.ContentType = "application/json";
                    await httpContext.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
                    {
                        success = false,
                        message = "Your account is not active. Please contact administrator.",
                        status = user.Status.ToString()
                    }));
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                httpContext.Response.StatusCode = 500;
                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "Error checking user status"
                }));
                return false;
            }
        }
    }
}
