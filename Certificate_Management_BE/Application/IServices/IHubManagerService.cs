namespace Application.IServices
{
    /// <summary>
    /// Service to manage SignalR hub notifications across different role-specific hubs
    /// </summary>
    public interface IHubManagerService
    {
        Task SendToUserAsync(string userId, string roleName, object notificationData);
        Task SendToGroupAsync(string groupName, object notificationData);
    }
}

