using Application.IHubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Certificate_Management_BE.Hubs
{
    /// <summary>
    /// SignalR Hub for Instructor role
    /// </summary>
    [Authorize(Roles = "Instructor")]
    public class InstructorHub : Hub<INotificationHub>
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
            
            if (!string.IsNullOrEmpty(userId))
            {
                // Add to instructor group
                await Groups.AddToGroupAsync(Context.ConnectionId, "instructors");
                
                // Add to user-specific group
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
                
                Console.WriteLine($"[InstructorHub] Instructor '{username}' (ID: {userId}) connected (ConnectionId: {Context.ConnectionId})");
            }
            
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
            
            if (!string.IsNullOrEmpty(userId))
            {
                Console.WriteLine($"[InstructorHub] Instructor '{username}' (ID: {userId}) disconnected");
            }
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}

