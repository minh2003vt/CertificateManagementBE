using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    using Domain.Entities;
    using Domain.Enums;
    using Microsoft.EntityFrameworkCore;

    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<CertificateTemplate> CertificateTemplates { get; set; }
        public DbSet<Decision> Decisions { get; set; }
        public DbSet<DecisionTemplate> DecisionTemplates { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<TraineeAssignation> TraineeAssignations { get; set; }
        public DbSet<InstructorAssignation> InstructorAssignations { get; set; }
        public DbSet<ClassTraineeAssignation> ClassTraineeAssignations { get; set; }
        public DbSet<CourseSubjectSpecialty> CourseSubjectSpecialties { get; set; }
        public DbSet<CourseCertificate> CourseCertificates { get; set; }
        public DbSet<SubjectCertificate> SubjectCertificates { get; set; }
        public DbSet<PlanCourse> PlanCourses { get; set; }
        public DbSet<PlanCertificate> PlanCertificates { get; set; }
        public DbSet<ExternalCertificate> ExternalCertificates { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Session> Sessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ClassTraineeAssignation>()
                .HasKey(e => new { e.ClassId, e.TraineeAssignationId });

            modelBuilder.Entity<CourseCertificate>()
                .HasKey(e => new { e.CertificateId, e.CourseId });

            modelBuilder.Entity<CourseSubjectSpecialty>()
                .HasKey(e => new { e.SpecialtyId, e.SubjectId, e.CourseId });

            modelBuilder.Entity<InstructorAssignation>()
                .HasKey(e => new { e.SubjectId, e.InstructorId });

            modelBuilder.Entity<PlanCertificate>()
                .HasKey(e => new { e.CertificateId, e.PlanId });

            modelBuilder.Entity<PlanCourse>()
                .HasKey(e => new { e.CourseId, e.PlanId });

            modelBuilder.Entity<SubjectCertificate>()
                .HasKey(e => new { e.CertificateId, e.SubjectId });

            // Configure relationships
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Department)
                .WithMany(d => d.Reports)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Specialty)
                .WithMany(s => s.Users)
                .HasForeignKey(u => u.SpecialtyId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Plan)
                .WithMany(p => p.Users)
                .HasForeignKey(u => u.PlanId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Department>()
                .HasOne(d => d.Specialty)
                .WithMany(s => s.Departments)
                .HasForeignKey(d => d.SpecialtyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Department>()
                .HasOne(d => d.Manager)
                .WithMany()
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Class>()
                .HasOne(c => c.Instructor)
                .WithMany(u => u.Classes)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.User)
                .WithMany(u => u.Certificates)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.CertificateTemplate)
                .WithMany(ct => ct.Certificates)
                .HasForeignKey(c => c.CertificateTemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.IssuedByUser)
                .WithMany()
                .HasForeignKey(c => c.IssuedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.ApprovedByUser)
                .WithMany()
                .HasForeignKey(c => c.ApprovedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<CertificateTemplate>()
                .HasOne(ct => ct.CreatedByUser)
                .WithMany()
                .HasForeignKey(ct => ct.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CertificateTemplate>()
                .HasOne(ct => ct.ApprovedByUser)
                .WithMany()
                .HasForeignKey(ct => ct.ApprovedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Decision>()
                .HasOne(d => d.IssuedByUser)
                .WithMany()
                .HasForeignKey(d => d.IssuedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Decision>()
                .HasOne(d => d.Certificate)
                .WithMany(c => c.Decisions)
                .HasForeignKey(d => d.CertificateId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Decision>()
                .HasOne(d => d.DecisionTemplate)
                .WithMany(dt => dt.Decisions)
                .HasForeignKey(d => d.DecisionTemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DecisionTemplate>()
                .HasOne(dt => dt.CreatedByUser)
                .WithMany()
                .HasForeignKey(dt => dt.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DecisionTemplate>()
                .HasOne(dt => dt.ApprovedByUser)
                .WithMany()
                .HasForeignKey(dt => dt.ApprovedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TraineeAssignation>()
                .HasOne(t => t.Trainee)
                .WithMany(u => u.TraineeAssignations)
                .HasForeignKey(t => t.TraineeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TraineeAssignation>()
                .HasOne(t => t.AssignedByUser)
                .WithMany()
                .HasForeignKey(t => t.AssignedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TraineeAssignation>()
                .HasOne(t => t.ApprovedByUser)
                .WithMany()
                .HasForeignKey(t => t.ApprovedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TraineeAssignation>()
                .HasOne(t => t.Request)
                .WithMany(r => r.TraineeAssignations)
                .HasForeignKey(t => t.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TraineeAssignation>()
                .HasOne(t => t.Subject)
                .WithMany(s => s.TraineeAssignations)
                .HasForeignKey(t => t.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TraineeAssignation>()
                .HasOne(t => t.GradedByInstructor)
                .WithMany()
                .HasForeignKey(t => t.GradedByInstructorId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<InstructorAssignation>()
                .HasOne(i => i.Subject)
                .WithMany(s => s.InstructorAssignations)
                .HasForeignKey(i => i.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InstructorAssignation>()
                .HasOne(i => i.Instructor)
                .WithMany(u => u.InstructorAssignations)
                .HasForeignKey(i => i.InstructorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InstructorAssignation>()
                .HasOne(i => i.AssignedByUser)
                .WithMany()
                .HasForeignKey(i => i.AssignedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ClassTraineeAssignation>()
                .HasOne(ct => ct.Class)
                .WithMany(c => c.ClassTraineeAssignations)
                .HasForeignKey(ct => ct.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClassTraineeAssignation>()
                .HasOne(ct => ct.TraineeAssignation)
                .WithMany(t => t.ClassTraineeAssignations)
                .HasForeignKey(ct => ct.TraineeAssignationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CourseSubjectSpecialty>()
                .HasOne(c => c.Specialty)
                .WithMany(s => s.CourseSubjectSpecialties)
                .HasForeignKey(c => c.SpecialtyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CourseSubjectSpecialty>()
                .HasOne(c => c.Subject)
                .WithMany(s => s.CourseSubjectSpecialties)
                .HasForeignKey(c => c.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CourseSubjectSpecialty>()
                .HasOne(c => c.Course)
                .WithMany(c => c.CourseSubjectSpecialties)
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlanCourse>()
                .HasOne(pc => pc.Course)
                .WithMany(c => c.PlanCourses)
                .HasForeignKey(pc => pc.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlanCourse>()
                .HasOne(pc => pc.Plan)
                .WithMany(p => p.PlanCourses)
                .HasForeignKey(pc => pc.PlanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExternalCertificate>()
                .HasOne(ec => ec.User)
                .WithMany(u => u.ExternalCertificates)
                .HasForeignKey(ec => ec.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Request>()
                .HasOne(r => r.RequestUser)
                .WithMany(u => u.Requests)
                .HasForeignKey(r => r.RequestUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Requests)
                .WithOne(r => r.RequestUser)
                .HasForeignKey(r => r.RequestUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Request>()
                .HasOne(r => r.ApprovedByUser)
                .WithMany(u => u.ApprovedRequests)
                .HasForeignKey(r => r.ApprovedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Report>()
                .HasOne(r => r.GeneratedByUser)
                .WithMany(u => u.Reports)
                .HasForeignKey(r => r.GeneratedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.Session)
                .WithMany(s => s.AuditLogs)
                .HasForeignKey(a => a.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Session>()
                .HasOne(s => s.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Sessions)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure enum conversions
            modelBuilder.Entity<User>()
                .Property(u => u.Sex)
                .HasConversion<string>();

            modelBuilder.Entity<User>()
                .Property(u => u.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Certificate>()
                .Property(c => c.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Course>()
                .Property(c => c.CourseLevel)
                .HasConversion<string>();

            modelBuilder.Entity<Course>()
                .Property(c => c.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Course>()
                .Property(c => c.Progress)
                .HasConversion<string>();

            modelBuilder.Entity<Department>()
                .Property(d => d.Status)
                .HasConversion<string>();

            modelBuilder.Entity<ExternalCertificate>()
                .Property(ec => ec.VerificationStatus)
                .HasConversion<string>();

            modelBuilder.Entity<TraineeAssignation>()
                .Property(t => t.RequestStatus)
                .HasConversion<string>();

            modelBuilder.Entity<TraineeAssignation>()
                .Property(t => t.GradeStatus)
                .HasConversion<string>();

            modelBuilder.Entity<InstructorAssignation>()
                .Property(i => i.RequestStatus)
                .HasConversion<string>();

            modelBuilder.Entity<CertificateTemplate>()
                .Property(ct => ct.TemplateStatus)
                .HasConversion<string>();

            modelBuilder.Entity<DecisionTemplate>()
                .Property(dt => dt.TemplateStatus)
                .HasConversion<string>();

            modelBuilder.Entity<Request>()
                .Property(r => r.RequestType)
                .HasConversion<string>();

            modelBuilder.Entity<Request>()
                .Property(r => r.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Decision>()
                .Property(d => d.DecisionStatus)
                .HasConversion<string>();

            modelBuilder.Entity<Report>()
                .Property(r => r.ReportType)
                .HasConversion<string>();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Certificate>()
                .HasIndex(c => c.CertificateCode);

            modelBuilder.Entity<ExternalCertificate>()
                .HasIndex(ec => ec.CertificateCode);

            modelBuilder.Entity<Request>()
                .HasIndex(r => r.RequestDate);

            modelBuilder.Entity<AuditLog>()
                .HasIndex(a => a.Timestamp);

            modelBuilder.Entity<AuditLog>()
                .Property(a => a.Timestamp)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<Certificate>()
                .Property(c => c.SignDate)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<CertificateTemplate>()
                .Property(ct => ct.CreatedAt)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<CertificateTemplate>()
                .Property(ct => ct.LastUpdatedAt)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<Course>()
                .Property(c => c.StartDateTime)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<Course>()
                .Property(c => c.EndDateTime)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<Course>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<Course>()
                .Property(c => c.UpdatedAt)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<CourseSubjectSpecialty>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<Decision>()
                .Property(d => d.IssueDate)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<Decision>()
                .Property(d => d.SignDate)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<DecisionTemplate>()
                .Property(dt => dt.CreatedAt)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<Department>()
                .Property(d => d.CreatedAt)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<Department>()
                .Property(d => d.UpdatedAt)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<ExternalCertificate>()
                .Property(ec => ec.CreatedAt)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<Notification>()
                .Property(n => n.CreatedAt)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<Report>()
                .Property(r => r.GenerateDate)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<Report>()
                .Property(r => r.StartDate)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<Report>()
                .Property(r => r.EndDate)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<Request>()
                .Property(r => r.RequestDate)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<Request>()
                .Property(r => r.CreatedAt)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<Request>()
                .Property(r => r.UpdatedAt)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<Session>()
                .Property(s => s.LoginTime)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<Specialty>()
                .Property(s => s.CreatedAt)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<Specialty>()
                .Property(s => s.UpdatedAt)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<Subject>()
                .Property(s => s.CreatedAt)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<Subject>()
                .Property(s => s.UpdatedAt)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<TraineeAssignation>()
                .Property(t => t.AssignDate)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<TraineeAssignation>()
                .Property(t => t.ApprovalDate)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<TraineeAssignation>()
                .Property(t => t.EvaluationDate)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<TraineeAssignation>()
                .Property(t => t.UpdateDate)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<User>()
                .Property(u => u.UpdatedAt)
                .HasDefaultValueSql("DATEADD(hour, 7, GETUTCDATE())");

            modelBuilder.Entity<User>()
                .HasQueryFilter(u => u.Status == AccountStatus.Active);

            modelBuilder.Entity<Certificate>()
                .HasQueryFilter(c => c.Status != CertificateStatus.Revoked);

            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.PlanCertificate)
                .WithOne(pc => pc.Certificate)
                .HasForeignKey<PlanCertificate>(pc => pc.CertificateId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.SubjectCertificate)
                .WithOne(sc => sc.Certificate)
                .HasForeignKey<SubjectCertificate>(sc => sc.CertificateId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.CourseCertificate)
                .WithOne(cc => cc.Certificate)
                .HasForeignKey<CourseCertificate>(cc => cc.CertificateId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
