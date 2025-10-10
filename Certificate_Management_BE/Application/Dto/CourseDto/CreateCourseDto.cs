using Application.Dto.UserDto;
using System.ComponentModel.DataAnnotations;

namespace Application.Dto.CourseDto
{
    /// <summary>
    /// DTO for creating a new course
    /// </summary>
    public class CreateCourseDto
    {
        [Required(ErrorMessage = "Course ID is required")]
        [MaxLength(50, ErrorMessage = "Course ID cannot exceed 50 characters")]
        public string CourseId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Course name is required")]
        [MaxLength(500, ErrorMessage = "Course name cannot exceed 500 characters")]
        public string CourseName { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        public ValidationErrorsDto Validate()
        {
            var errors = new ValidationErrorsDto();

            if (string.IsNullOrWhiteSpace(CourseId))
                errors.Add("Course ID is required");
            else if (CourseId.Length > 50)
                errors.Add("Course ID cannot exceed 50 characters");

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

