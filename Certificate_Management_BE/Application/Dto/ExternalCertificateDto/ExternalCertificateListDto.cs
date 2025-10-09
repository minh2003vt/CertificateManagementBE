namespace Application.Dto.ExternalCertificateDto
{
    public class ExternalCertificateListDto
    {
        public int ExternalCertificateId { get; set; }
        public string CertificateCode { get; set; } = string.Empty;
        public string CertificateName { get; set; } = string.Empty;
    }
}

