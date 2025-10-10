using Application.Dto.UserDto;
using System.ComponentModel.DataAnnotations;

namespace Application.Dto.InstructorAssignationDto
{
    /// <summary>
    /// DTO for updating an existing instructor assignation
    /// </summary>
    public class UpdateInstructorAssignationDto
    {
        [Required(ErrorMessage = "Assign date is required")]
        public DateTime AssignDate { get; set; }

        [Required(ErrorMessage = "Request status is required")]
        public string RequestStatus { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }

        public ValidationErrorsDto Validate()
        {
            var errors = new ValidationErrorsDto();

            if (AssignDate == default)
                errors.Add("Assign date is required");

            if (!string.IsNullOrWhiteSpace(RequestStatus))
            {
                var validStatuses = new[] { "Pending", "Approved", "Rejected", "Updating", "Deleting" };
                if (!validStatuses.Contains(RequestStatus))
                    errors.Add("Request status must be one of: Pending, Approved, Rejected, Updating, Deleting");
            }

            if (Notes != null && Notes.Length > 500)
                errors.Add("Notes cannot exceed 500 characters");

            return errors;
        }
    }
}

