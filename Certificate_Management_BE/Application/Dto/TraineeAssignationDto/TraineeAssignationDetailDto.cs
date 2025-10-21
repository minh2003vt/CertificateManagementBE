using System;
using Domain.Enums;

namespace Application.Dto.TraineeAssignationDto
{
    public class TraineeAssignationDetailDto
    {
        public string TraineeAssignationId { get; set; } = string.Empty;
        public string TraineeId { get; set; } = string.Empty;
        public int ClassId { get; set; }
        public OverallGradeStatus OverallGradeStatus { get; set; }
        public AssignmentKind AssignmentKind { get; set; }
        public DateTime AssignDate { get; set; }
        public DateTime GradeDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}


