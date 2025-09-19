using Application.IServices;
using Application.ViewModels.UserDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var result = await _authenticationService.LoginAsync(loginModel);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion
    }
}
