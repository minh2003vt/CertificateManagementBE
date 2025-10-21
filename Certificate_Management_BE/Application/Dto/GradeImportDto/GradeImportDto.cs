using System.ComponentModel.DataAnnotations;

namespace Application.Dto.GradeImportDto
{
    public class GradeImportDto
    {
        [Required]
        public string TraineeId { get; set; } = string.Empty;
        
        public int? Attendance { get; set; }
        public decimal? PracticalExamScore { get; set; }
        public decimal? FinalExamScore { get; set; }
        public string? Note { get; set; }
    }
}
