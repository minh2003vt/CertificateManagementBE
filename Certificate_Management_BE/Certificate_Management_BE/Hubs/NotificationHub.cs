using Application.IHubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Certificate_Management_BE.Hubs
{
    /// <summary>
    /// SignalR Hub for real-time notifications
    /// </summary>
    [Authorize]
    public class NotificationHub : Hub<INotificationHub>
    {
        /// <summary>
        /// Called when a client connects to the hub
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                // Add user to their personal group
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");

                // If user is admin, add to admin group
                var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
                if (userRole == "Admin")
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "admins");
                }

                Console.WriteLine($"User {userId} connected to NotificationHub (ConnectionId: {Context.ConnectionId})");
            }

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Called when a client disconnects from the hub
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                Console.WriteLine($"User {userId} disconnected from NotificationHub (ConnectionId: {Context.ConnectionId})");
            }

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Client can call this to manually join their user group (backup method)
        /// </summary>
        public async Task JoinUserGroup()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            }
        }

        /// <summary>
        /// Mark notification as read (called from client)
        /// </summary>
        public async Task MarkNotificationAsRead(int notificationId)
        {
            // This could be handled by the NotificationService
            // For now, just acknowledge
            await Clients.Caller.NotificationMarkedAsRead(notificationId);
        }
    }
}

