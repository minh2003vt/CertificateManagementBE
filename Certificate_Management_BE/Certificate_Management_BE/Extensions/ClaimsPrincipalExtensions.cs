using System.Security.Claims;

namespace Certificate_Management_BE.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst("UserId")?.Value ?? string.Empty;
        }
    }
}