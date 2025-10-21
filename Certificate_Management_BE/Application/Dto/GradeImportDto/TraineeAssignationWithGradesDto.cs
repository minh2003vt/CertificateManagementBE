using System.Collections.Generic;

namespace Application.Dto.GradeImportDto
{
    public class TraineeAssignationWithGradesDto
    {
        public string TraineeAssignationId { get; set; } = string.Empty;
        public string TraineeId { get; set; } = string.Empty;
        public string TraineeName { get; set; } = string.Empty;
        public string AssignedByUserId { get; set; } = string.Empty;
        public string AssignedByUserName { get; set; } = string.Empty;
        public DateTime AssignDate { get; set; }
        public Domain.Enums.OverallGradeStatus OverallGradeStatus { get; set; }
        public Domain.Enums.AssignmentKind AssignmentKind { get; set; }
        public DateTime GradeDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public List<TraineeAssignationGradeDto> Grades { get; set; } = new();
    }

    public class TraineeAssignationGradeDto
    {
        public string TraineeAssignationGradeId { get; set; } = string.Empty;
        public string TraineeAssignationId { get; set; } = string.Empty;
        public Domain.Enums.GradeKind GradeKind { get; set; }
        public decimal Grade { get; set; }
        public Domain.Enums.GradeStatus GradeStatus { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
