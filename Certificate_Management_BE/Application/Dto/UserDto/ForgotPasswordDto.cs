using System.ComponentModel.DataAnnotations;

namespace Application.Dto.UserDto
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Email or Username is required")]
        public string EmailOrUsername { get; set; } = string.Empty;
    }
}

