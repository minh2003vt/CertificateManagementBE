using Application.Dto.NotificationDto;
using Application.IHubs;
using Application.IServices;
using Application.ServiceResponse;
using Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<NotificationService> _logger;
        private readonly IHubManagerService _hubManager;

        public NotificationService(
            IUnitOfWork unitOfWork,
            ILogger<NotificationService> logger,
            IHubManagerService hubManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _hubManager = hubManager;
        }

        public async Task<ServiceResponse<NotificationDto>> CreateNotificationAsync(CreateNotificationDto dto)
        {
            try
            {
                var notification = new Domain.Entities.Notification
                {
                    UserId = dto.UserId,
                    Title = dto.Title,
                    Message = dto.Message,
                    NotificationType = dto.NotificationType,
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    IsRead = false
                };

                await _unitOfWork.NotificationRepository.AddAsync(notification);
                await _unitOfWork.SaveChangesAsync();

                var notificationDto = new NotificationDto
                {
                    NotificationId = notification.NotificationId,
                    Title = notification.Title,
                    Message = notification.Message,
                    NotificationType = notification.NotificationType,
                    CreatedAt = notification.CreatedAt,
                    IsRead = notification.IsRead
                };

                return new ServiceResponse<NotificationDto>
                {
                    Success = true,
                    Message = "Notification created successfully",
                    Data = notificationDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification");
                return new ServiceResponse<NotificationDto>
                {
                    Success = false,
                    Message = $"Failed to create notification: {ex.Message}"
                };
            }
        }

        public async Task<bool> SendSignalRNotificationAsync(string userId, string title, string message, object? data = null)
        {
            try
            {
                // Get user to determine their role
                var user = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == userId);

                if (user == null)
                {
                    _logger.LogWarning($"User {userId} not found, cannot send SignalR notification");
                    return false;
                }

                // Load role if not loaded
                if (user.Role == null && user.RoleId > 0)
                {
                    user.Role = await _unitOfWork.RoleRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(r => r.RoleId == user.RoleId);
                }

                var roleName = user.Role?.RoleName ?? "User";

                var notificationData = new
                {
                    userId = userId,
                    title = title,
                    message = message,
                    timestamp = DateTime.UtcNow,
                    data = data
                };

                // Send to user's personal group in their role-specific hub
                await _hubManager.SendToUserAsync(userId, roleName, notificationData);

                _logger.LogInformation($"Successfully sent SignalR notification to user {userId} via {roleName} hub");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send SignalR notification to user {userId}");
                return false;
            }
        }

        public async Task NotifyAdminsAboutNewTraineesAsync(int successCount, int failureCount, string performedByUsername)
        {
            try
            {
                // Get all admin users
                var context = _unitOfWork.Context as DbContext;
                if (context == null)
                {
                    _logger.LogError("Unable to access DbContext for querying admin users");
                    return;
                }

                var adminUsers = await context.Set<User>()
                    .Include(u => u.Role)
                    .Where(u => u.Role.RoleName == "Admin")
                    .ToListAsync();

                if (!adminUsers.Any())
                {
                    _logger.LogWarning("No admin users found to notify");
                    return;
                }

                var title = "New Trainees Imported";
                var message = $"{performedByUsername} imported {successCount + failureCount} trainee(s): {successCount} succeeded, {failureCount} failed.";
                var notificationType = "Trainee Import";

                foreach (var admin in adminUsers)
                {
                    // Create database notification
                    var createDto = new CreateNotificationDto
                    {
                        UserId = admin.UserId,
                        Title = title,
                        Message = message,
                        NotificationType = notificationType
                    };

                    await CreateNotificationAsync(createDto);

                    // Send real-time SignalR notification
                    var signalRData = new
                    {
                        type = "trainee_import",
                        successCount = successCount,
                        failureCount = failureCount,
                        performedBy = performedByUsername
                    };

                    await SendSignalRNotificationAsync(admin.UserId, title, message, signalRData);
                }

                _logger.LogInformation($"Notified {adminUsers.Count} admin(s) about trainee import");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying admins about new trainees");
            }
        }

        public async Task<ServiceResponse<List<NotificationDto>>> GetUserNotificationsAsync(string userId, bool unreadOnly = false)
        {
            try
            {
                var notifications = await _unitOfWork.NotificationRepository.GetByUserIdAsync(userId);

                if (unreadOnly)
                {
                    notifications = notifications.Where(n => !n.IsRead).ToList();
                }

                var notificationDtos = notifications
                    .OrderByDescending(n => n.CreatedAt)
                    .Select(n => new NotificationDto
                    {
                        NotificationId = n.NotificationId,
                        Title = n.Title,
                        Message = n.Message,
                        NotificationType = n.NotificationType,
                        CreatedAt = n.CreatedAt,
                        IsRead = n.IsRead
                    })
                    .ToList();

                return new ServiceResponse<List<NotificationDto>>
                {
                    Success = true,
                    Message = "Notifications retrieved successfully",
                    Data = notificationDtos
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user notifications");
                return new ServiceResponse<List<NotificationDto>>
                {
                    Success = false,
                    Message = $"Failed to retrieve notifications: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<bool>> MarkAsReadAsync(int notificationId, string userId)
        {
            try
            {
                var notification = await _unitOfWork.NotificationRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(n => n.NotificationId == notificationId);

                if (notification == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Notification not found"
                    };
                }

                if (notification.UserId != userId)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Unauthorized to mark this notification as read"
                    };
                }

                notification.IsRead = true;
                await _unitOfWork.NotificationRepository.UpdateAsync(notification);
                await _unitOfWork.SaveChangesAsync();

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Notification marked as read",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read");
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Failed to mark notification as read: {ex.Message}"
                };
            }
        }
    }
}

