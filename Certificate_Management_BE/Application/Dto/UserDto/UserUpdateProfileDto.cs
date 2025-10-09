using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.UserDto
{
    public class UserUpdateProfileDto
    { 
        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Sex is required")]
        public Sex Sex { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Citizen ID is required")]
        [RegularExpression(@"^(\d{9}|\d{12})$", ErrorMessage = "Citizen ID must contain either 9 or 12 digits.")]
        public string CitizenId { get; set; } = string.Empty;
    }
}
