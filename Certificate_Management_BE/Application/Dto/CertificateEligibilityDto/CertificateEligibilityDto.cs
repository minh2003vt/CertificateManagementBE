using Domain.Enums;

namespace Application.Dto.CertificateEligibilityDto
{
    public class CertificateEligibilityDto
    {
        public string CertificateId { get; set; } = string.Empty;
        public string CertificateName { get; set; } = string.Empty;
        public string PlanId { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public string CourseId { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string SubjectId { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public bool IsEligible { get; set; }
        public List<string> MissingRequirements { get; set; } = new();
        public CertificateType CertificateType { get; set; }
    }

    public enum CertificateType
    {
        PlanCertificate = 1,
        CourseCertificate = 2,
        SubjectCertificate = 3
    }
}
