using Application.Dto.UserDto;
using System.ComponentModel.DataAnnotations;

namespace Application.Dto.CourseSubjectSpecialtyDto
{
    /// <summary>
    /// DTO for creating a new course-subject-specialty relationship
    /// </summary>
    public class CreateCourseSubjectSpecialtyDto
    {
        [Required(ErrorMessage = "Specialty ID is required")]
        public string SpecialtyId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subject ID is required")]
        public string SubjectId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Course ID is required")]
        public string CourseId { get; set; } = string.Empty;

        public ValidationErrorsDto Validate()
        {
            var errors = new ValidationErrorsDto();

            if (string.IsNullOrWhiteSpace(SpecialtyId))
                errors.Add("Specialty ID is required");

            if (string.IsNullOrWhiteSpace(SubjectId))
                errors.Add("Subject ID is required");

            if (string.IsNullOrWhiteSpace(CourseId))
                errors.Add("Course ID is required");

            return errors;
        }
    }
}

