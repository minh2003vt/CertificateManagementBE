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
    public class Certificate
    {
        [Key]
        public string CertificateId { get; set; } = string.Empty;
        public string CertificateCode { get; set; } = string.Empty;

        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;
        public virtual User User { get; set; } = null!;

        [ForeignKey("CertificateTemplate")]
        public string CertificateTemplateId { get; set; } = string.Empty;
        public virtual CertificateTemplate CertificateTemplate { get; set; } = null!;

        [ForeignKey("IssuedByUser")]
        public string IssuedByUserId { get; set; } = string.Empty;
        public virtual User IssuedByUser { get; set; } = null!;

        public DateTime IssueDate { get; set; }

        [ForeignKey("ApprovedByUser")]
        public string? ApprovedByUserId { get; set; }
        public virtual User? ApprovedByUser { get; set; }

        public CertificateStatus Status { get; set; } 

        public DateTime SignDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
        public DateTime? ExpirationDate { get; set; }
        public string CertificateURL { get; set; } = string.Empty;
        public bool IsRevoked { get; set; } = false;
        public string? RevocationReason { get; set; }
        public DateTime? RevocationDate { get; set; }
        public bool IncludesRelearn { get; set; } = false;
        public string? RelearnSubjects { get; set; }

    }
}


