using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.UserDto
{
    public class UserStatusDto
    {
        public string UserId { get; set; } = string.Empty;
        public AccountStatus Status { get; set; } = AccountStatus.Active;
    }
}
