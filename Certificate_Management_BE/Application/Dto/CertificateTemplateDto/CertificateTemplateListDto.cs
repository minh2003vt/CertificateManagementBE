using Domain.Enums;

namespace Application.Dto.CertificateTemplateDto
{
    public class CertificateTemplateListDto
    {
        public string CertificateTemplateId { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TemplateStatus TemplateStatus { get; set; }
        public CertificateKind CertificateKind { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
    }
}
