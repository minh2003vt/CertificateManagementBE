using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Subject
    {
        [Key]
        public string SubjectId { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string SubjectName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Credits { get; set; }
        public double PassingScore { get; set; }

        [ForeignKey("CreatedByUserId")]
        public string CreatedByUserId { get; set; } = string.Empty;
        public virtual User? CreatedByUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
        public DateTime UpdatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
        public virtual ICollection<CourseSubjectSpecialty> CourseSubjectSpecialties { get; set; } = [];
        public virtual ICollection<TraineeAssignation> TraineeAssignations { get; set; } = [];
        public virtual ICollection<InstructorAssignation> InstructorAssignations { get; set; } = [];
        public virtual ICollection<SubjectCertificate> SubjectCertificates { get; set; } = [];
    }
}
