using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PlanCourse
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Course")]
        public string CourseId { get; set; } = string.Empty;
        public virtual Course Course { get; set; } = null!;

        [Key, Column(Order = 1)]
        [ForeignKey("Plan")]
        public string PlanId { get; set; } = string.Empty;
        public virtual Plan Plan { get; set; } = null!;

    }
}
