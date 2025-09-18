using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    public class ExternalCertificate
    {
        [Key]
        public int ExternalCertificateId { get; set; }

        public string CertificateCode { get; set; } = string.Empty;
        public string CertificateName { get; set; } = string.Empty;
        public string IssuingOrganization { get; set; } = string.Empty;

        [ForeignKey("User")]
        public string? UserId { get; set; }
        public virtual User? User { get; set; }

        [ForeignKey("VerifiedByUser")]
        public string VerifiedByUserId { get; set; } = string.Empty;
        public virtual User? VerifiedByUser { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime VerifyDate { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        [Required]
        public string CertificateFileURL { get; set; } = string.Empty;// Path to uploaded file
        public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
    }
}
