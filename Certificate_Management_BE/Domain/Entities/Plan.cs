using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Plan
    {
        [Key]
        public string PlanId { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string PlanName { get; set; } = string.Empty; 
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
        public DateTime EndDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
        public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
        [ForeignKey("CreatedByUser")]
        public string? CreatedByUserId { get; set; }
        public virtual User? CreatedByUser { get; set; }
        public PlanStatus Status { get; set; } // pending, approved, rejected

        [ForeignKey("AprovedUser")]
        public string? AprovedUserId { get; set; } = string.Empty;
        public virtual User AprovedUser { get; set; } = null!;
        public DateTime? ApprovedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);

        [ForeignKey("Specialty")]
        public string SpecialtyId { get; set; } = string.Empty;
        public virtual Specialty? Specialty { get; set; }
        public virtual ICollection<StudyRecord> StudyRecords { get; set; } = [];
        public virtual ICollection<PlanCertificate> PlanCertificates { get; set; } = [];
    }
}
