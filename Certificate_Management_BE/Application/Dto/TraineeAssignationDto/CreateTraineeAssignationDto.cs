using System.ComponentModel.DataAnnotations;

namespace Application.Dto.TraineeAssignationDto
{
    public class CreateTraineeAssignationDto
    {
        [Required]
        public string TraineeId { get; set; } = string.Empty;
        [Required]
        public int ClassId { get; set; }
    }
}


