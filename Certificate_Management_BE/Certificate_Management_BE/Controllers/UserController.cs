using Application.IServices;
using Certificate_Management_BE.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Certificate_Management_BE.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        #region GetProfile
        [HttpGet("profile")]
        [AuthorizeRoles()] 
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Success = false, Message = "User ID not found in token" });
            }

            var result = await _userService.GetUserProfileAsync(userId);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        #endregion
    }
}

