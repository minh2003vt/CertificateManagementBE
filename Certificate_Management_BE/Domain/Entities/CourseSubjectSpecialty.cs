using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CourseSubjectSpecialty
    {
        [ForeignKey("Specialty")]
        public string SpecialtyId { get; set; } = string.Empty;
        public virtual Specialty Specialty { get; set; } = null!;

        [ForeignKey("Subject")]
        public string SubjectId { get; set; } = string.Empty;
        public virtual Subject Subject { get; set; } = null!;
        [ForeignKey("Course")]
        public string CourseId { get; set; } = string.Empty;
        public virtual Course Course { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
    }
}
