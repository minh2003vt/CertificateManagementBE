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
    public class Course
    {
        [Key]
        public string CourseId { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string CourseName { get; set; } = string.Empty;
        public string? Description { get; set; }
        [ForeignKey("CreatedByUser")]
        public string CreatedByUserId { get; set; } = string.Empty;
        public virtual User CreatedByUser { get; set; } = null!;
        public CourseStatus Status { get; set; } = CourseStatus.Pending; // pending, approved, rejected

        [ForeignKey("AprovedUser")]
        public string? AprovedUserId { get; set; }
        public virtual User? AprovedUser { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
        public DateTime UpdatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);

        public virtual ICollection<CourseSubjectSpecialty> CourseSubjectSpecialties { get; set; } = [];
        public virtual ICollection<StudyRecord> StudyRecords { get; set; } = [];
        public virtual ICollection<CourseCertificate> CourseCertificates { get; set; } = [];
    }
}
