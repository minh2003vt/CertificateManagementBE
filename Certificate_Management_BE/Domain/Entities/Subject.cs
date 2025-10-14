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
    public class Subject
    {
        [Key]
        public string SubjectId { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string SubjectName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? MinAttendance { get; set; }

        [Range(0, 10, ErrorMessage = "Value must be between 0 and 10.")]
        public double? MinPracticeExamScore { get; set; }

        [Range(0, 10, ErrorMessage = "Value must be between 0 and 10.")]
        public double? MinFinalExamScore { get; set; }

        [Required]
        [Range(0, 10, ErrorMessage = "Value must be between 0 and 10.")]
        public double MinTotalScore { get; set; } = 0;

        [ForeignKey("CreatedByUser")]
        public string? CreatedByUserId { get; set; }
        public virtual User? CreatedByUser { get; set; }
        [ForeignKey("AprovedUser")]
        public string? AprovedUserId { get; set; }
        public virtual User? AprovedUser { get; set; }
        public SubjectStatus Status { get; set; } = SubjectStatus.Pending; // pending, approved, rejected
        public DateTime? ApprovedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<CourseSubjectSpecialty> CourseSubjectSpecialties { get; set; } = [];
        public virtual ICollection<TraineeAssignation> TraineeAssignations { get; set; } = [];
        public virtual ICollection<InstructorAssignation> InstructorAssignations { get; set; } = [];
        public virtual ICollection<SubjectCertificate> SubjectCertificates { get; set; } = [];
        public virtual ICollection<StudyRecord> StudyRecords { get; set; } = [];

    }
}
