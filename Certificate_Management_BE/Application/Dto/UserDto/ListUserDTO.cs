using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.UserDto
{
    public class ListUserDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public Sex Sex { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string CitizenId { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public  AccountStatus Status { get; set; }

    }
}
