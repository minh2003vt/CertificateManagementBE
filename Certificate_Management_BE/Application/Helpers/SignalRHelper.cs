namespace Application.Helpers
{
    /// <summary>
    /// Helper class for SignalR hub routing based on user role
    /// </summary>
    public static class SignalRHelper
    {
        /// <summary>
        /// Get SignalR hub URL based on role name
        /// </summary>
        /// <param name="roleName">Role name (Admin, Education Officer, Instructor, Trainee)</param>
        /// <returns>Hub URL path</returns>
        public static string GetHubUrlByRole(string roleName)
        {
            return roleName switch
            {
                "Administrator" => "/hubs/Administrator",
                "Education Officer" => "/hubs/education-officer",
                "Instructor" => "/hubs/instructor",
                "Trainee" => "/hubs/trainee",
                _ => throw new ArgumentException($"Unknown role: {roleName}")
            };
        }

        /// <summary>
        /// Get SignalR group name based on role
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <returns>Group name</returns>
        public static string GetGroupNameByRole(string roleName)
        {
            return roleName switch
            {
                "Administrator" => "Administrator",
                "Education Officer" => "education_officers",
                "Instructor" => "instructors",
                "Trainee" => "trainees",
                _ => throw new ArgumentException($"Unknown role: {roleName}")
            };
        }

        /// <summary>
        /// Get all available hub URLs
        /// </summary>
        public static Dictionary<string, string> GetAllHubUrls()
        {
            return new Dictionary<string, string>
            {
                { "Administrator", "/hubs/Administrator" },
                { "Education Officer", "/hubs/education-officer" },
                { "Instructor", "/hubs/instructor" },
                { "Trainee", "/hubs/trainee" }
            };
        }
    }
}

