using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    public class User
    {
        [Key]
        public string UserId { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public Sex Sex { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string CitizenId { get; set; } = string.Empty;


        [ForeignKey("Role")]
        public int RoleId { get; set; }
        public virtual Role Role { get; set; } = null!;

        [ForeignKey("Department")]
        public string? DepartmentId { get; set; }
        public AccountStatus Status { get; set; } = AccountStatus.Active;

        public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
        public DateTime UpdatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
        public DateTime? LastLogin { get; set; }

        public virtual Department? Department { get; set; }

        public string? AvatarUrl { get; set; }
        public virtual ICollection<UserSpecialty> UserSpecialties { get; set; } = [];
        public virtual ICollection<Class> Classes { get; set; } = [];
        public virtual ICollection<TraineeAssignation> TraineeAssignations { get; set; } = [];
        public virtual ICollection<InstructorAssignation> InstructorAssignations { get; set; } = [];
        public virtual ICollection<Session> Sessions { get; set; } = [];
        public virtual ICollection<Notification> Notifications { get; set; } = [];
        public virtual ICollection<Report> Reports { get; set; } = [];
        public virtual ICollection<Request> Requests { get; set; } = [];
        public virtual ICollection<Request>? ApprovedRequests { get; set; }
        public virtual ICollection<ExternalCertificate>? ExternalCertificates { get; set; }
        public virtual ICollection<Department>? ManagedDepartments { get; set; }
        public virtual ICollection<Certificate> Certificates { get; set; } = [];
        public virtual ICollection<Certificate>? IssuedCertificates { get; set; }
        public virtual ICollection<Certificate>? ApprovedCertificates { get; set; }
        public virtual ICollection<Decision> Decisions { get; set; } = [];
    }
}
