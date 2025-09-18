using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Specialty
    {
        [Key]
        public string SpecialtyId { get; set; } = string.Empty;

        [Required]
        public string SpecialtyName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);

        public DateTime UpdatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);

        [ForeignKey("CreatedByUser")]
        public string? CreatedByUserId { get; set; }

        [ForeignKey("UpdatedByUser")]
        public string? UpdatedByUserId { get; set; }

        public int Status { get; set; } = 1;
        public virtual User? CreatedByUser { get; set; }
        public virtual User? UpdatedByUser { get; set; }
        public virtual ICollection<CourseSubjectSpecialty> CourseSubjectSpecialties { get; set; } = [];
        public virtual ICollection<User> Users { get; set; } = [];
    }
}
