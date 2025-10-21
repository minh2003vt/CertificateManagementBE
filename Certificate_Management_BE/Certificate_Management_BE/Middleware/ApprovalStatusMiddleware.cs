using Application.Helpers;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Certificate_Management_BE.Middleware
{
    /// <summary>
    /// Middleware to check approval status before allowing modifications
    /// </summary>
    public class ApprovalStatusMiddleware
    { 
    }
}






