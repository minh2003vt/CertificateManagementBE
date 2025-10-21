using System.ComponentModel.DataAnnotations;

namespace Application.Dto.TraineePlanEnrollmentDto
{
    public class CreateTraineePlanEnrollmentDto
    {
        [Required]
        public string TraineeId { get; set; } = string.Empty;
        
        [Required]
        public string PlanId { get; set; } = string.Empty;
        
        public string? Notes { get; set; }
    }
}
