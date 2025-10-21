using System.ComponentModel.DataAnnotations;

namespace Application.Dto.DecisionTemplateDto
{
    public class CreateDecisionTemplateDto
    {
        [Required]
        [MaxLength(200)]
        public string TemplateName { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}
