using Application.IHubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Certificate_Management_BE.Hubs
{
    /// <summary>
    /// SignalR Hub for Education Officer role
    /// </summary>
    [Authorize(Roles = "Education Officer")]
    public class EducationOfficerHub : Hub<INotificationHub>
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
            
            if (!string.IsNullOrEmpty(userId))
            {
                // Add to education officer group
                await Groups.AddToGroupAsync(Context.ConnectionId, "education_officers");
                
                // Add to user-specific group
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
                
                Console.WriteLine($"[EducationOfficerHub] Education Officer '{username}' (ID: {userId}) connected (ConnectionId: {Context.ConnectionId})");
            }
            
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
            
            if (!string.IsNullOrEmpty(userId))
            {
                Console.WriteLine($"[EducationOfficerHub] Education Officer '{username}' (ID: {userId}) disconnected");
            }
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}

