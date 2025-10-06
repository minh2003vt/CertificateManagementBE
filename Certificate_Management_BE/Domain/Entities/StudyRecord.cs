using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class StudyRecord 
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Course")]
        public string CourseId { get; set; } = string.Empty;
        public virtual Course Course { get; set; } = null!;
        [Key, Column(Order = 2)]
        [ForeignKey("Subject")]
        public string SubjectId { get; set; } = string.Empty;
        public virtual Subject Subject { get; set; } = null!;

        [Key, Column(Order = 1)]
        [ForeignKey("Plan")]
        public string PlanId { get; set; } = string.Empty;
        public virtual Plan Plan { get; set; } = null!;

    }
}
