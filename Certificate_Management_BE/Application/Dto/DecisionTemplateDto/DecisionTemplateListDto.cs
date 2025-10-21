using Domain.Enums;

namespace Application.Dto.DecisionTemplateDto
{
    public class DecisionTemplateListDto
    {
        public string DecisionTemplateId { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TemplateStatus TemplateStatus { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
    }
}
