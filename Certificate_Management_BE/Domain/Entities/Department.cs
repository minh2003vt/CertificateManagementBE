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
    public class Department
    {
        [Key]
        public string DepartmentId { get; set; } = string.Empty;

        [Required]
        public string DepartmentName { get; set; } = string.Empty;
        public string DepartmentDescription { get; set; } = string.Empty;

        [ForeignKey("Specialty")]
        public string SpecialtyId { get; set; } = string.Empty;
        public virtual Specialty Specialty { get; set; } = null!;
        public DepartmentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<UserDepartment> UserDepartments { get; set; } = [];
    }
}
