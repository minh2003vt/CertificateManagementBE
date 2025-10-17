using Application.IServices;
using Application.Dto.UserDto;
using Certificate_Management_BE.Attributes;
using Certificate_Management_BE.Extensions;
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
        private readonly IAuthenticationService _authenticationService;

        public UserController(IUserService userService, IAuthenticationService authenticationService)
        {
            _userService = userService;
            _authenticationService = authenticationService;
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

        #region UploadAvatar
        /// <summary>
        /// Upload user avatar to Cloudinary and update profile
        /// </summary>
        [HttpPut("profile/avatar")]
        [AuthorizeRoles()]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Success = false, Message = "User ID not found in token" });
            }

            var result = await _userService.UploadAvatarAsync(userId, file);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        #endregion

        #region DeleteAvatar
        /// <summary>
        /// Delete user avatar on Cloudinary and clear AvatarUrl
        /// </summary>
        [HttpDelete("profile/avatar")]
        [AuthorizeRoles()]
        public async Task<IActionResult> DeleteAvatar()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Success = false, Message = "User ID not found in token" });
            }

            var result = await _userService.DeleteAvatarAsync(userId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion

        #region GetAllUsers
        [HttpGet("all")]
        [AuthorizeRoles("Administrator", "Education Officer")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllUsersAsync();
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

        #region AdminUpdateUserProfile
        /// <summary>
        /// Admin updates a user's profile by userId
        /// </summary>
        [HttpPut("profile/{userId}")]
        [AuthorizeRoles("Administrator")]
        public async Task<IActionResult> UpdateUserProfileByAdmin(string userId, [FromBody] UserUpdateProfileDto dto)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { Success = false, Message = "User ID is required" });
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

        #region UpdateUserStatus
        /// <summary>
        /// Update user account status (Active, Deactivated, Pending)
        /// </summary>
        /// <param name="dto">User status update DTO containing UserId and Status</param>
        /// <returns>Success message with updated user ID</returns>
        [HttpPut("status")]
        [AuthorizeRoles("Education Officer","Administrator")]
        public async Task<IActionResult> UpdateUserStatus([FromBody] UserStatusDto dto)
        {
            if (string.IsNullOrEmpty(dto.UserId))
            {
                return BadRequest(new { Success = false, Message = "User ID is required" });
            }

            var result = await _userService.UpdateUserStatusAsync(dto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        #endregion

        #region SendCredentialsEmail
        /// <summary>
        /// Send credentials email to user containing username and password
        /// </summary>
        /// <param name="userId">User ID to send credentials to</param>
        /// <returns>Success message confirming email was sent</returns>
        [HttpPost("send-credentials/{userId}")]
        [AuthorizeRoles("Education Officer", "Administrator")]
        public async Task<IActionResult> SendCredentialsEmail(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { Success = false, Message = "User ID is required" });
            }

            var result = await _userService.SendCredentialsEmailAsync(userId);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        #endregion

        /// <summary>
        /// Create manual account (Education Officer only)
        /// </summary>
        [HttpPost("create-manual-account")]
        [AuthorizeRoles("Administrator")]
        public async Task<IActionResult> CreateManualAccount([FromBody] CreateManualAccountDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token");
            }

            var result = await _authenticationService.CreateManualAccountAsync(dto, userId);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        #region GetAllByRole
        [HttpGet("get-all-by-role")]
        [AuthorizeRoles("Administrator", "Education Officer")]
        public async Task<IActionResult> GetAllByRole(int roleid)
        {
            var result = await _userService.GetAllByRoleAsync(roleid);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion
    }
}

