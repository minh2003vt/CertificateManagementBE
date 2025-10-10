using Application.IServices;
using Application.Dto.UserDto;
using Certificate_Management_BE.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
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

        #region ImportTrainee
        /// <summary>
        /// Import trainees from Excel file
        /// </summary>
        /// <param name="file">Excel file (.xlsx or .xls)</param>
        /// <returns>Import result with success/failure counts</returns>
        [HttpPost("import-trainees")]
            [AuthorizeRoles("Education Officer")]
            [Consumes("multipart/form-data")]
            public async Task<IActionResult> ImportTrainees(IFormFile file)
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { Success = false, Message = "Please upload a valid Excel file" });
                }

                // Validate file extension
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (fileExtension != ".xlsx" && fileExtension != ".xls")
                {
                    return BadRequest(new { Success = false, Message = "Only Excel files (.xlsx, .xls) are allowed" });
                }

                // Get current user info for notification
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown User";

                var result = await _userService.ImportTraineesAsync(file, username);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            #endregion
        }
    } 

