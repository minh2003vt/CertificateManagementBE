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
        public DateOnly StartDate { get; set; } 
        public DateOnly EndDate { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("CreatedByUser")]
        public string? CreatedByUserId { get; set; }
        public virtual User? CreatedByUser { get; set; }
        public PlanStatus Status { get; set; } = PlanStatus.Pending;
        [ForeignKey("AprovedUser")]
        public string? AprovedUserId { get; set; }
        public virtual User? AprovedUser { get; set; }
        public DateTime? ApprovedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("Specialty")]
        public string SpecialtyId { get; set; } = string.Empty;
        public virtual Specialty? Specialty { get; set; }
        public virtual ICollection<StudyRecord> StudyRecords { get; set; } = [];
        public virtual ICollection<PlanCertificate> PlanCertificates { get; set; } = [];
    }
}
