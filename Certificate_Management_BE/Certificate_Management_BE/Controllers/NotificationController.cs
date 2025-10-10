using Application.Dto.NotificationDto;
using Application.IServices;
using Certificate_Management_BE.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Certificate_Management_BE.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Get all notifications for the current user
        /// </summary>
        [HttpGet]
        [AuthorizeRoles()]
        public async Task<IActionResult> GetUserNotifications([FromQuery] bool unreadOnly = false)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Success = false, Message = "User ID not found in token" });
            }

            var result = await _notificationService.GetUserNotificationsAsync(userId, unreadOnly);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Mark a notification as read
        /// </summary>
        [HttpPut("{notificationId}/mark-read")]
        [AuthorizeRoles()]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Success = false, Message = "User ID not found in token" });
            }

            var result = await _notificationService.MarkAsReadAsync(notificationId, userId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Create a notification (Admin only)
        /// </summary>
        [HttpPost]
        [AuthorizeRoles("Administrator")]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationDto dto)
        {
            var result = await _notificationService.CreateNotificationAsync(dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Test notification system - sends notification to all admins (Admin only)
        /// </summary>
        [HttpPost("test-admin-notification")]
        [AuthorizeRoles("Administrator")]
        public async Task<IActionResult> TestAdminNotification()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Test User";

            await _notificationService.NotifyAdminsAboutNewTraineesAsync(
                successCount: 5,
                failureCount: 2,
                performedByUsername: username
            );

            return Ok(new
            {
                success = true,
                message = "Test notification sent to all admins. Check your notifications!"
            });
        }
    }
}

