using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.Dto.CertificateTemplateDto
{
    public class UpdateCertificateTemplateDto
    {
        [Required]
        [MaxLength(200)]
        public string TemplateName { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public CertificateKind CertificateKind { get; set; }
        
        [Required]
        public TemplateStatus TemplateStatus { get; set; }
    }
}
