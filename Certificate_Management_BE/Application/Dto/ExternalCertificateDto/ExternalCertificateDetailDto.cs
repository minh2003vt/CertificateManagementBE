using System;

namespace Application.Dto.ExternalCertificateDto
{
    public class ExternalCertificateDetailDto
    {
        public int ExternalCertificateId { get; set; }
        public string CertificateCode { get; set; } = string.Empty;
        public string CertificateName { get; set; } = string.Empty;
        public string IssuingOrganization { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string? UserFullName { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime Exp_date { get; set; }
        public string CertificateFileUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}

