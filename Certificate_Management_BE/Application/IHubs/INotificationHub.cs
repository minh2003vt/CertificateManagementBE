namespace Application.IHubs
{
    /// <summary>
    /// Interface for SignalR Notification Hub client methods
    /// </summary>
    public interface INotificationHub
    {
        /// <summary>
        /// Client method to receive notifications
        /// </summary>
        Task ReceiveNotification(object notificationData);

        /// <summary>
        /// Client method to receive notification marked as read confirmation
        /// </summary>
        Task NotificationMarkedAsRead(int notificationId);
    }
}

