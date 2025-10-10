using Application.Dto.UserDto;
using System.ComponentModel.DataAnnotations;

namespace Application.Dto.InstructorAssignationDto
{
    /// <summary>
    /// DTO for creating a new instructor assignation
    /// </summary>
    public class CreateInstructorAssignationDto
    {
        [Required(ErrorMessage = "Subject ID is required")]
        public string SubjectId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Instructor ID is required")]
        public string InstructorId { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }

        public ValidationErrorsDto Validate()
        {
            var errors = new ValidationErrorsDto();

            if (string.IsNullOrWhiteSpace(SubjectId))
                errors.Add("Subject ID is required");

            if (string.IsNullOrWhiteSpace(InstructorId))
                errors.Add("Instructor ID is required");

            if (Notes != null && Notes.Length > 500)
                errors.Add("Notes cannot exceed 500 characters");

            return errors;
        }
    }
}

