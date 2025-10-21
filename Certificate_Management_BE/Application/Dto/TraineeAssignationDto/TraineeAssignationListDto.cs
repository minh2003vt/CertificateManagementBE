using System;

namespace Application.Dto.TraineeAssignationDto
{
    public class TraineeAssignationListDto
    {
        public string TraineeAssignationId { get; set; } = string.Empty;
        public string TraineeId { get; set; } = string.Empty;
        public int ClassId { get; set; }
        public DateTime AssignDate { get; set; }
    }
}


