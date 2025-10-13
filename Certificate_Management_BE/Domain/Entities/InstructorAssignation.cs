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
    public class InstructorAssignation
    {
        //[Key]
        //public string AssignmentId { get; set; }
        [Key, Column(Order = 0)]
        [ForeignKey("Subject")]
        public string SubjectId { get; set; } = string.Empty;
        public virtual Subject Subject { get; set; } = null!;

        [Key, Column(Order = 1)]
        [ForeignKey("Instructor")]
        public string InstructorId { get; set; } = string.Empty;
        public virtual User Instructor { get; set; } = null!;

        [ForeignKey("AssignedByUser")]
        public string AssignedByUserId { get; set; } = string.Empty;
        public virtual User AssignedByUser { get; set; } = null!;
        public DateTime AssignDate { get; set; }
        public string Notes { get; set; } = string.Empty;

    }
}
