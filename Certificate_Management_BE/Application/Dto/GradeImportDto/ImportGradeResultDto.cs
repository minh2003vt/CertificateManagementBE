using System.Collections.Generic;

namespace Application.Dto.GradeImportDto
{
    public class ImportGradeResultDto
    {
        public int TotalRows { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<ImportGradeErrorDto> Errors { get; set; } = new();
    }

    public class ImportGradeErrorDto
    {
        public int RowNumber { get; set; }
        public string TraineeId { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}
