using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;

namespace Certificate_Management_BE.Attributes
{
    /// <summary>
    /// Authorization attribute for role-based access control.
    /// Use without parameters [AuthorizeRoles()] to allow all authenticated users.
    /// Use with roles [AuthorizeRoles("Admin", "Manager")] to allow specific roles only.
    /// </summary>
    public class AuthorizeRolesAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public AuthorizeRolesAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // Check if user is authenticated
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // If no specific roles are required, allow all authenticated users
            if (_roles == null || _roles.Length == 0)
            {
                return;
            }

            // Check if user has any of the required roles
            var userRoles = user.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            if (!_roles.Any(role => userRoles.Contains(role)))
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}

