using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.Dto.TraineeAssignationDto
{
    public class UpdateTraineeAssignationDto
    {
        public OverallGradeStatus? OverallGradeStatus { get; set; }
        public AssignmentKind? AssignmentKind { get; set; }
    }
}


