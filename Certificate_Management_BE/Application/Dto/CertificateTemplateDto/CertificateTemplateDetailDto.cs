using System;
using Domain.Enums;

namespace Application.Dto.CertificateTemplateDto
{
    public class CertificateTemplateDetailDto
    {
        public string CertificateTemplateId { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? TemplateFileUrl { get; set; } // HTML content
        public string CreatedByUserId { get; set; } = string.Empty;
        public string CreatedByUserName { get; set; } = string.Empty;
        public TemplateStatus TemplateStatus { get; set; }
        public CertificateKind CertificateKind { get; set; }
        public string? ApprovedByUserId { get; set; }
        public string? ApprovedByUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
