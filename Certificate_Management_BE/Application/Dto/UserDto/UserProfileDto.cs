using Domain.Enums;
using System;

namespace Application.Dto.UserDto
{
    public class UserProfileDto
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public Sex Sex { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string CitizenId { get; set; } = string.Empty;
    }
   
}

