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

        [ForeignKey("Subject")]
        public string SubjectId { get; set; } = string.Empty;
        public virtual Subject Subject { get; set; } = null!;

        [ForeignKey("ClassGroup")]
        public int ClassGroupId { get; set; }
        public virtual ClassGroup ClassGroup { get; set; } = null!;

        public virtual ICollection<ClassTraineeAssignation> ClassTraineeAssignations { get; set; } = [];


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
