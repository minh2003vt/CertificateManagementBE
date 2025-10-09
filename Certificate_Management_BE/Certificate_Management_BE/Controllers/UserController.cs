using Application.IServices;
using Application.Dto.UserDto;
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

        #region UpdateProfile
        [HttpPut("profile")]
        [AuthorizeRoles()]
        public async Task<IActionResult> UpdateProfile([FromBody] UserUpdateProfileDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Success = false, Message = "User ID not found in token" });
            }

            var result = await _userService.UpdateUserProfileAsync(userId, dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        #endregion

        #region ChangePassword
        [HttpPost("change-password")]
        [AuthorizeRoles()]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Success = false, Message = "User ID not found in token" });
            }

            var result = await _userService.ChangePasswordAsync(userId, dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        #endregion
    }
}

