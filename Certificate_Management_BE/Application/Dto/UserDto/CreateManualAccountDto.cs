using System.ComponentModel.DataAnnotations;

namespace Application.Dto.UserDto
{
    public class CreateManualAccountDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string CitizenId { get; set; } = string.Empty;
        
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required]
        public DateOnly DateOfBirth { get; set; }
        
        public Domain.Enums.Sex Sex { get; set; } = Domain.Enums.Sex.Male;
        
        [Required]
        public int RoleId { get; set; }
    }
}
