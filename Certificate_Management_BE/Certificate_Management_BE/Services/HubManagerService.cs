using Application.IHubs;
using Application.IServices;
using Certificate_Management_BE.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Certificate_Management_BE.Services
{
    /// <summary>
    /// Service to manage SignalR hub notifications across different role-specific hubs
    /// </summary>
    public class HubManagerService : IHubManagerService
    {
        private readonly IHubContext<AdminHub, INotificationHub> _adminHub;
        private readonly IHubContext<EducationOfficerHub, INotificationHub> _educationOfficerHub;
        private readonly IHubContext<InstructorHub, INotificationHub> _instructorHub;
        private readonly IHubContext<TraineeHub, INotificationHub> _traineeHub;
        private readonly ILogger<HubManagerService> _logger;

        public HubManagerService(
            IHubContext<AdminHub, INotificationHub> adminHub,
            IHubContext<EducationOfficerHub, INotificationHub> educationOfficerHub,
            IHubContext<InstructorHub, INotificationHub> instructorHub,
            IHubContext<TraineeHub, INotificationHub> traineeHub,
            ILogger<HubManagerService> logger)
        {
            _adminHub = adminHub;
            _educationOfficerHub = educationOfficerHub;
            _instructorHub = instructorHub;
            _traineeHub = traineeHub;
            _logger = logger;
        }

        public async Task SendToUserAsync(string userId, string roleName, object notificationData)
        {
            try
            {
                // Send to appropriate hub based on role
                switch (roleName)
                {
                    case "Administrator":
                        await _adminHub.Clients.Group($"user_{userId}").ReceiveNotification(notificationData);
                        break;
                    case "Education Officer":
                        await _educationOfficerHub.Clients.Group($"user_{userId}").ReceiveNotification(notificationData);
                        break;
                    case "Instructor":
                        await _instructorHub.Clients.Group($"user_{userId}").ReceiveNotification(notificationData);
                        break;
                    case "Trainee":
                        await _traineeHub.Clients.Group($"user_{userId}").ReceiveNotification(notificationData);
                        break;
                    default:
                        _logger.LogWarning($"Unknown role: {roleName}");
                        return;
                }
                _logger.LogInformation($"Sent notification to user {userId} in {roleName} hub");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending notification to user {userId} in {roleName} hub");
            }
        }

        public async Task SendToGroupAsync(string groupName, object notificationData)
        {
            try
            {
                // Send to appropriate hub based on group name
                switch (groupName.ToLower())
                {
                    case "admins":
                        await _adminHub.Clients.Group(groupName).ReceiveNotification(notificationData);
                        break;
                    case "education_officers":
                        await _educationOfficerHub.Clients.Group(groupName).ReceiveNotification(notificationData);
                        break;
                    case "instructors":
                        await _instructorHub.Clients.Group(groupName).ReceiveNotification(notificationData);
                        break;
                    case "trainees":
                        await _traineeHub.Clients.Group(groupName).ReceiveNotification(notificationData);
                        break;
                    default:
                        _logger.LogWarning($"Unknown group name: {groupName}");
                        break;
                }
                _logger.LogInformation($"Sent notification to group: {groupName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending notification to group {groupName}");
            }
        }

    }
}

