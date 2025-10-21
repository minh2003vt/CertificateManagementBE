using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.Dto.DecisionTemplateDto
{
    public class UpdateDecisionTemplateDto
    {
        [Required]
        [MaxLength(200)]
        public string TemplateName { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public TemplateStatus TemplateStatus { get; set; }
    }
}
