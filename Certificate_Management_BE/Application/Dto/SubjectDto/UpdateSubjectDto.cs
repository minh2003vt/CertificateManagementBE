using Application.Dto.UserDto;
using System.ComponentModel.DataAnnotations;

namespace Application.Dto.SubjectDto
{
    /// <summary>
    /// DTO for updating an existing subject
    /// </summary>
    public class UpdateSubjectDto
    {
        [Required(ErrorMessage = "Subject name is required")]
        [MaxLength(500, ErrorMessage = "Subject name cannot exceed 500 characters")]
        public string SubjectName { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Range(0, 100, ErrorMessage = "Minimum attendance must be between 0 and 100")]
        public int? MinAttendance { get; set; }

        [Range(0, 10, ErrorMessage = "Minimum practice exam score must be between 0 and 10")]
        public double? MinPracticeExamScore { get; set; }

        [Range(0, 10, ErrorMessage = "Minimum final exam score must be between 0 and 10")]
        public double? MinFinalExamScore { get; set; }

        // MinTotalScore is optional - will be auto-calculated if not provided
        [Range(0, 10, ErrorMessage = "Minimum total score must be between 0 and 10")]
        public double? MinTotalScore { get; set; }

        /// <summary>
        /// Validates the DTO and returns validation errors
        /// </summary>
        public ValidationErrorsDto Validate()
        {
            var errors = new ValidationErrorsDto();

            if (string.IsNullOrWhiteSpace(SubjectName))
                errors.Add("Subject name is required");

            if (MinAttendance.HasValue && (MinAttendance < 0 || MinAttendance > 100))
                errors.Add("Minimum attendance must be between 0 and 100");

            if (MinPracticeExamScore.HasValue && (MinPracticeExamScore < 0 || MinPracticeExamScore > 10))
                errors.Add("Minimum practice exam score must be between 0 and 10");

            if (MinFinalExamScore.HasValue && (MinFinalExamScore < 0 || MinFinalExamScore > 10))
                errors.Add("Minimum final exam score must be between 0 and 10");

            if (MinTotalScore.HasValue && (MinTotalScore < 0 || MinTotalScore > 10))
                errors.Add("Minimum total score must be between 0 and 10");

            return errors;
        }

        /// <summary>
        /// Calculates the minimum total score based on component scores
        /// </summary>
        public double CalculateMinTotalScore()
        {
            var totalComponents = 0;
            var sumRequired = 0.0;

            if (MinAttendance.HasValue)
            {
                totalComponents++;
                sumRequired += MinAttendance.Value;
            }

            if (MinPracticeExamScore.HasValue)
            {
                totalComponents++;
                sumRequired += MinPracticeExamScore.Value;
            }

            if (MinFinalExamScore.HasValue)
            {
                totalComponents++;
                sumRequired += MinFinalExamScore.Value;
            }

            if (totalComponents > 0)
            {
                return Math.Round(sumRequired / totalComponents, 2);
            }

            return 0;
        }
    }
}

