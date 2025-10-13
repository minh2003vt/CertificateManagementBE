using Application.Dto.UserDto;
using System.ComponentModel.DataAnnotations;

namespace Application.Dto.PlanDto
{
    /// <summary>
    /// DTO for creating a new plan
    /// </summary>
    public class CreatePlanDto
    {
        [Required(ErrorMessage = "Plan ID is required")]
        [MaxLength(50, ErrorMessage = "Plan ID cannot exceed 50 characters")]
        public string PlanId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Plan name is required")]
        [MaxLength(100, ErrorMessage = "Plan name cannot exceed 100 characters")]
        public string PlanName { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateOnly EndDate { get; set; }

        [Required(ErrorMessage = "Specialty ID is required")]
        public string SpecialtyId { get; set; } = string.Empty;

        public ValidationErrorsDto Validate()
        {
            var errors = new ValidationErrorsDto();

            if (string.IsNullOrWhiteSpace(PlanId))
                errors.Add("Plan ID is required");
            else if (PlanId.Length > 50)
                errors.Add("Plan ID cannot exceed 50 characters");

            if (string.IsNullOrWhiteSpace(PlanName))
                errors.Add("Plan name is required");
            else if (PlanName.Length > 100)
                errors.Add("Plan name cannot exceed 100 characters");

            if (Description != null && Description.Length > 1000)
                errors.Add("Description cannot exceed 1000 characters");

            if (StartDate == default)
                errors.Add("Start date is required");

            if (EndDate == default)
                errors.Add("End date is required");

            if (StartDate >= EndDate)
                errors.Add("End date must be after start date");

            if (string.IsNullOrWhiteSpace(SpecialtyId))
                errors.Add("Specialty ID is required");

            return errors;
        }
    }
}
