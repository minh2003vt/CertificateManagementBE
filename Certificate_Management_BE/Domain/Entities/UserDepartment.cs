using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserDepartment
    {
        [Key, Column(Order = 0)]
        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;
        public virtual User User { get; set; } = null!;
        [Key, Column(Order = 1)]
        [ForeignKey("Department")]
        public string DepartmentId { get; set; } = string.Empty;
        public virtual Department Department { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
        public DateTime LeavedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
        public string?  Note{get; set; } = string.Empty;
    }
}
