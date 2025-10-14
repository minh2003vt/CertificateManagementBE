using Application.IServices;
using Application.Dto.UserDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Certificate_Management_BE.Attributes;
using Certificate_Management_BE.Extensions;
using System.Security.Claims;

namespace Certificate_Management_BE.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        #region Login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authenticationService.LoginAsync(loginDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            if (!string.IsNullOrEmpty(result.Token))
            {
                Response.Cookies.Append("auth_token", result.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Set to true in production with HTTPS
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(50)
                });
            }

            return Ok(result);
        }
        #endregion

        #region ForgotPassword
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var result = await _authenticationService.ForgotPasswordAsync(dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion

        #region ResetPassword
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromQuery] string token, [FromBody] ResetPasswordDto dto)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { Success = false, Message = "Token is required" });
            }

            var result = await _authenticationService.ResetPasswordAsync(token, dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            // Set token in cookie (optional, for extra security)
            Response.Cookies.Append("reset_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(5)
            });

            return Ok(result);
        }

        #endregion

    }
}
