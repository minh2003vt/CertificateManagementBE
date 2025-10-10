using Application.Dto.UserDto;
using System.ComponentModel.DataAnnotations;

namespace Application.Dto.SpecialtyDto
{
    /// <summary>
    /// DTO for creating a new specialty
    /// </summary>
    public class CreateSpecialtyDto
    {
        [Required(ErrorMessage = "Specialty ID is required")]
        [MaxLength(50, ErrorMessage = "Specialty ID cannot exceed 50 characters")]
        public string SpecialtyId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Specialty name is required")]
        [MaxLength(200, ErrorMessage = "Specialty name cannot exceed 200 characters")]
        public string SpecialtyName { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        public ValidationErrorsDto Validate()
        {
            var errors = new ValidationErrorsDto();

            if (string.IsNullOrWhiteSpace(SpecialtyId))
                errors.Add("Specialty ID is required");
            else if (SpecialtyId.Length > 50)
                errors.Add("Specialty ID cannot exceed 50 characters");

            if (string.IsNullOrWhiteSpace(SpecialtyName))
                errors.Add("Specialty name is required");
            else if (SpecialtyName.Length > 200)
                errors.Add("Specialty name cannot exceed 200 characters");

            if (Description != null && Description.Length > 1000)
                errors.Add("Description cannot exceed 1000 characters");

            return errors;
        }
    }
}

