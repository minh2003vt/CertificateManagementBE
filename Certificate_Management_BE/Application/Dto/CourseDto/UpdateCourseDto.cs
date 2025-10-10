using Application.Dto.UserDto;
using System.ComponentModel.DataAnnotations;

namespace Application.Dto.CourseDto
{
    /// <summary>
    /// DTO for updating an existing course
    /// </summary>
    public class UpdateCourseDto
    {
        [Required(ErrorMessage = "Course name is required")]
        [MaxLength(500, ErrorMessage = "Course name cannot exceed 500 characters")]
        public string CourseName { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        public ValidationErrorsDto Validate()
        {
            var errors = new ValidationErrorsDto();

            if (string.IsNullOrWhiteSpace(CourseName))
                errors.Add("Course name is required");
            else if (CourseName.Length > 500)
                errors.Add("Course name cannot exceed 500 characters");

            if (Description != null && Description.Length > 1000)
                errors.Add("Description cannot exceed 1000 characters");

            return errors;
        }
    }
}

