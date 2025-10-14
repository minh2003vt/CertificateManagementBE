using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Class
    {
        [Key]
        public int ClassId { get; set; }

        [ForeignKey("Instructor")]
        public string InstructorId { get; set; } = string.Empty;
        public virtual User Instructor { get; set; } = null!;

        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }

        public virtual ICollection<ClassTraineeAssignation> ClassTraineeAssignations { get; set; } = [];


        [ForeignKey("AprovedUser")]
        public string? AprovedUserId { get; set; }
        public virtual User? AprovedUser { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
