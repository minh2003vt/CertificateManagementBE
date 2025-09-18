using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Plan
    {
        [Key]
        public string PlanId { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string PlanName { get; set; } = string.Empty;
        public virtual ICollection<PlanCourse> PlanCourses { get; set; } = [];
        public virtual ICollection<User> Users { get; set; } = [];
        public virtual ICollection<PlanCertificate> PlanCertificates { get; set; } = [];
    }
}
