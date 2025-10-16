using Application.Dto.NotificationDto;
using Application.ServiceResponse;

namespace Application.IServices
{
    public interface INotificationService
    {
        /// <summary>
        /// Creates a notification in the database
        /// </summary>
        Task<ServiceResponse<NotificationDto>> CreateNotificationAsync(CreateNotificationDto dto);

        /// <summary>
        /// Sends a real-time notification to a user via SignalR
        /// </summary>
        Task<bool> SendSignalRNotificationAsync(string userId, string title, string message, object? data = null);

        /// <summary>
        /// Notifies all admin users about new trainees being imported
        /// </summary>
        Task NotifyAdminsAboutNewTraineesAsync(int successCount, int failureCount, string performedByUsername);

        /// <summary>
        /// Gets all notifications for a specific user
        /// </summary>
        Task<ServiceResponse<List<NotificationDto>>> GetUserNotificationsAsync(string userId, bool unreadOnly = false);

        /// <summary>
        /// Marks a notification as read
        /// </summary>
        Task<ServiceResponse<bool>> MarkAsReadAsync(int notificationId, string userId);

        /// <summary>
        /// Sends a welcome notification to a specific user (DB + SignalR)
        /// </summary>
        Task SendWelcomeAsync(string userId);
    }
}

