using Application.Dto.UserDto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.SubjectDto
{
    public class SubjectImportDto
    {
        [Required(ErrorMessage = "Subject ID is required")]
        public string SubjectId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subject name is required")]
        [MaxLength(500, ErrorMessage = "Subject name cannot exceed 500 characters")]
        public string SubjectName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int? MinAttendance { get; set; }

        public double? MinPracticeExamScore { get; set; }

        public double? MinFinalExamScore { get; set; }

        public double? MinTotalScore { get; set; }

        public ValidationErrorsDto Validate()
        {
            var errors = new ValidationErrorsDto();

            // Validate SubjectId format and length
            if (string.IsNullOrWhiteSpace(SubjectId))
            {
                errors.Add($"{nameof(SubjectId)}: Subject ID cannot be empty");
            }
            else if (SubjectId.Length > 50)
            {
                errors.Add($"{nameof(SubjectId)}: Subject ID cannot exceed 50 characters");
            }

            // Validate SubjectName
            if (string.IsNullOrWhiteSpace(SubjectName))
            {
                errors.Add($"{nameof(SubjectName)}: Subject name cannot be empty");
            }
            else if (SubjectName.Length > 500)
            {
                errors.Add($"{nameof(SubjectName)}: Subject name cannot exceed 500 characters");
            }

            // Validate score ranges (0-10)
            if (MinAttendance.HasValue && (MinAttendance.Value < 0 || MinAttendance.Value > 10))
            {
                errors.Add($"{nameof(MinAttendance)}: Min attendance must be between 0 and 10");
            }

            if (MinPracticeExamScore.HasValue && (MinPracticeExamScore.Value < 0 || MinPracticeExamScore.Value > 10))
            {
                errors.Add($"{nameof(MinPracticeExamScore)}: Min practice exam score must be between 0 and 10");
            }

            if (MinFinalExamScore.HasValue && (MinFinalExamScore.Value < 0 || MinFinalExamScore.Value > 10))
            {
                errors.Add($"{nameof(MinFinalExamScore)}: Min final exam score must be between 0 and 10");
            }

            // No validation for MinTotalScore - it will be auto-calculated

            return errors;
        }

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

    public class SubjectImportResultDto
    {
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public int TotalCount { get; set; }
        public List<SubjectImportErrorDto> Errors { get; set; } = new();
        public List<string> SuccessfulSubjectIds { get; set; } = new();
    }

    public class SubjectImportErrorDto
    {
        public string SubjectId { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int RowNumber { get; set; }
        public List<string> ValidationErrors { get; set; } = new();
        public string? GeneralError { get; set; }
    }
}

