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
    public class CertificateTemplate
    {
        [Key]
        public string CertificateTemplateId { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string TemplateName { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Column(TypeName = "text")]
        public string? TemplateFileUrl { get; set; } // HTML content

        [ForeignKey("CreatedByUser")]
        public string CreatedByUserId { get; set; } = string.Empty;
        public virtual User CreatedByUser { get; set; } = null!;
        public TemplateStatus TemplateStatus { get; set; }
        public CertificateKind CertificateKind { get; set; }
        [ForeignKey("ApprovedByUser")]
        public string? ApprovedByUserId { get; set; }
        public User? ApprovedByUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<Certificate> Certificates { get; set; } = [];
    }
}
