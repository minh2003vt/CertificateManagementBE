using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class TraineePlanEnrollment
    {
        [Key]
        public string TraineePlanEnrollmentId { get; set; } = string.Empty;

        [ForeignKey("Trainee")]
        public string TraineeId { get; set; } = string.Empty;
        public virtual User Trainee { get; set; } = null!;

        [ForeignKey("Plan")]
        public string PlanId { get; set; } = string.Empty;
        public virtual Plan Plan { get; set; } = null!;

        [ForeignKey("EnrolledByUser")]
        public string? EnrolledByUserId { get; set; }
        public virtual User? EnrolledByUser { get; set; }

        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
        public DateTime? CompletionDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
