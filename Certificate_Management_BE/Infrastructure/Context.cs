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
        public DbSet<StudyRecord> StudyRecords { get; set; }
        public DbSet<PlanCertificate> PlanCertificates { get; set; }
        public DbSet<ExternalCertificate> ExternalCertificates { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<UserDepartment> UserDepartments { get; set; }
        public DbSet<RequestEntity> RequestEntities { get; set; }

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

            modelBuilder.Entity<StudyRecord>()
                .HasKey(e => new { e.CourseId, e.PlanId, e.SubjectId });

            modelBuilder.Entity<SubjectCertificate>()
                .HasKey(e => new { e.CertificateId, e.SubjectId });

            modelBuilder.Entity<UserSpecialty>()
                .HasKey(e => new { e.UserId, e.SpecialtyId });

            modelBuilder.Entity<UserDepartment>()
                .HasKey(e => new { e.UserId, e.DepartmentId });

            modelBuilder.Entity<RequestEntity>()
                .HasKey(re => new { re.RequestId, re.EntityId });

            // Configure relationships
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Department>()
                .HasOne(d => d.Specialty)
                .WithMany(s => s.Departments)
                .HasForeignKey(d => d.SpecialtyId)
                .OnDelete(DeleteBehavior.Restrict);

            // UserDepartment relationships (many-to-many through junction table)
            modelBuilder.Entity<UserDepartment>()
                .HasOne(ud => ud.User)
                .WithMany(u => u.UserDepartments)
                .HasForeignKey(ud => ud.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserDepartment>()
                .HasOne(ud => ud.Department)
                .WithMany(d => d.UserDepartments)
                .HasForeignKey(ud => ud.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Class>()
                .HasOne(c => c.Instructor)
                .WithMany(u => u.Classes)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Class>()
                .HasOne(c => c.AprovedUser)
                .WithMany()
                .HasForeignKey(c => c.AprovedUserId)
                .OnDelete(DeleteBehavior.SetNull);

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
                .WithMany(u => u.IssuedCertificates)
                .HasForeignKey(c => c.IssuedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.ApprovedByUser)
                .WithMany(u => u.ApprovedCertificates)
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

            modelBuilder.Entity<CourseSubjectSpecialty>()
                .HasOne(c => c.ApprovedByUser)
                .WithMany()
                .HasForeignKey(c => c.ApprovedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<StudyRecord>()
                .HasOne(sr => sr.Course)
                .WithMany(c => c.StudyRecords)
                .HasForeignKey(sr => sr.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudyRecord>()
                .HasOne(sr => sr.Subject)
                .WithMany(s => s.StudyRecords)
                .HasForeignKey(sr => sr.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudyRecord>()
                .HasOne(sr => sr.Plan)
                .WithMany(p => p.StudyRecords)
                .HasForeignKey(sr => sr.PlanId)
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

            modelBuilder.Entity<RequestEntity>()
                .HasOne(re => re.Request)
                .WithMany(r => r.RequestEntities)
                .HasForeignKey(re => re.RequestId)
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

            modelBuilder.Entity<Course>()
                .HasOne(c => c.CreatedByUser)
                .WithMany()
                .HasForeignKey(c => c.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.AprovedUser)
                .WithMany()
                .HasForeignKey(c => c.AprovedUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Subject>()
                .HasOne(s => s.CreatedByUser)
                .WithMany()
                .HasForeignKey(s => s.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Subject>()
                .HasOne(s => s.AprovedUser)
                .WithMany()
                .HasForeignKey(s => s.AprovedUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Specialty>()
                .HasOne(s => s.CreatedByUser)
                .WithMany()
                .HasForeignKey(s => s.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Specialty>()
                .HasOne(s => s.UpdatedByUser)
                .WithMany()
                .HasForeignKey(s => s.UpdatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Plan>()
                .HasOne(p => p.CreatedByUser)
                .WithMany()
                .HasForeignKey(p => p.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Plan>()
                .HasOne(p => p.Specialty)
                .WithMany()
                .HasForeignKey(p => p.SpecialtyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Plan>()
                .HasOne(p => p.AprovedUser)
                .WithMany()
                .HasForeignKey(p => p.AprovedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserSpecialty>()
                .HasOne(us => us.User)
                .WithMany(u => u.UserSpecialties)
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserSpecialty>()
                .HasOne(us => us.Specialty)
                .WithMany(s => s.UserSpecialties)
                .HasForeignKey(us => us.SpecialtyId)
                .OnDelete(DeleteBehavior.Cascade);

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
                .Property(c => c.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Subject>()
                .Property(s => s.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Department>()
                .Property(d => d.Status)
                .HasConversion<string>();

            modelBuilder.Entity<TraineeAssignation>()
                .Property(t => t.RequestStatus)
                .HasConversion<string>();

            modelBuilder.Entity<TraineeAssignation>()
                .Property(t => t.GradeStatus)
                .HasConversion<string>();

            modelBuilder.Entity<TraineeAssignation>()
                .Property(t => t.Gradekind)
                .HasConversion<string>();


            modelBuilder.Entity<CertificateTemplate>()
                .Property(ct => ct.TemplateStatus)
                .HasConversion<string>();

            modelBuilder.Entity<CertificateTemplate>()
                .Property(ct => ct.TemplateContent)
                .HasColumnType("text");

            modelBuilder.Entity<DecisionTemplate>()
                .Property(dt => dt.TemplateStatus)
                .HasConversion<string>();

            modelBuilder.Entity<DecisionTemplate>()
                .Property(dt => dt.TemplateContent)
                .HasColumnType("text");

            modelBuilder.Entity<RequestEntity>()
                .Property(re => re.RequestType)
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

            modelBuilder.Entity<Report>()
                .Property(r => r.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Plan>()
                .Property(p => p.Status)
                .HasConversion<string>();


            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.CitizenId)
                .IsUnique();

            modelBuilder.Entity<Certificate>()
                .HasIndex(c => c.CertificateCode);

            modelBuilder.Entity<ExternalCertificate>()
                .HasIndex(ec => ec.CertificateCode);

            modelBuilder.Entity<Request>()
                .HasIndex(r => r.CreatedAt);

            // Enum conversions for Request
            modelBuilder.Entity<Request>()
                .Property(r => r.Status)
                .HasConversion<string>();

            // Add missing indexes for better performance
            modelBuilder.Entity<ExternalCertificate>()
                .HasIndex(ec => ec.CertificateCode);

            modelBuilder.Entity<Course>()
                .HasIndex(c => c.CreatedByUserId);

            modelBuilder.Entity<Course>()
                .HasIndex(c => c.AprovedUserId);

            modelBuilder.Entity<Subject>()
                .HasIndex(s => s.CreatedByUserId);

            modelBuilder.Entity<Subject>()
                .HasIndex(s => s.AprovedUserId);

            modelBuilder.Entity<Specialty>()
                .HasIndex(s => s.CreatedByUserId);

            modelBuilder.Entity<Specialty>()
                .HasIndex(s => s.UpdatedByUserId);

            modelBuilder.Entity<DecisionTemplate>()
                .HasIndex(dt => dt.CreatedByUserId);

            modelBuilder.Entity<DecisionTemplate>()
                .HasIndex(dt => dt.ApprovedByUserId);

            modelBuilder.Entity<CertificateTemplate>()
                .HasIndex(ct => ct.CreatedByUserId);

            modelBuilder.Entity<CertificateTemplate>()
                .HasIndex(ct => ct.ApprovedByUserId);

            modelBuilder.Entity<Department>()
                .HasIndex(d => d.SpecialtyId);

            modelBuilder.Entity<Class>()
                .HasIndex(c => c.InstructorId);

            modelBuilder.Entity<Class>()
                .HasIndex(c => c.AprovedUserId);

            modelBuilder.Entity<Certificate>()
                .HasIndex(c => c.UserId);

            modelBuilder.Entity<Certificate>()
                .HasIndex(c => c.CertificateTemplateId);

            modelBuilder.Entity<Certificate>()
                .HasIndex(c => c.IssuedByUserId);

            modelBuilder.Entity<Certificate>()
                .HasIndex(c => c.ApprovedByUserId);

            modelBuilder.Entity<Decision>()
                .HasIndex(d => d.IssuedByUserId);

            modelBuilder.Entity<Decision>()
                .HasIndex(d => d.CertificateId);

            modelBuilder.Entity<Decision>()
                .HasIndex(d => d.DecisionTemplateId);

            modelBuilder.Entity<TraineeAssignation>()
                .HasIndex(t => t.TraineeId);

            modelBuilder.Entity<TraineeAssignation>()
                .HasIndex(t => t.AssignedByUserId);

            modelBuilder.Entity<TraineeAssignation>()
                .HasIndex(t => t.ApprovedByUserId);

            modelBuilder.Entity<TraineeAssignation>()
                .HasIndex(t => t.RequestId);

            modelBuilder.Entity<TraineeAssignation>()
                .HasIndex(t => t.SubjectId);

            modelBuilder.Entity<TraineeAssignation>()
                .HasIndex(t => t.GradedByInstructorId);

            modelBuilder.Entity<InstructorAssignation>()
                .HasIndex(i => i.SubjectId);

            modelBuilder.Entity<InstructorAssignation>()
                .HasIndex(i => i.InstructorId);

            modelBuilder.Entity<InstructorAssignation>()
                .HasIndex(i => i.AssignedByUserId);

            modelBuilder.Entity<ClassTraineeAssignation>()
                .HasIndex(ct => ct.ClassId);

            modelBuilder.Entity<ClassTraineeAssignation>()
                .HasIndex(ct => ct.TraineeAssignationId);

            modelBuilder.Entity<CourseSubjectSpecialty>()
                .HasIndex(c => c.SpecialtyId);

            modelBuilder.Entity<CourseSubjectSpecialty>()
                .HasIndex(c => c.SubjectId);

            modelBuilder.Entity<CourseSubjectSpecialty>()
                .HasIndex(c => c.CourseId);

            modelBuilder.Entity<CourseSubjectSpecialty>()
                .HasIndex(c => c.ApprovedByUserId);

            modelBuilder.Entity<StudyRecord>()
                .HasIndex(sr => sr.CourseId);

            modelBuilder.Entity<StudyRecord>()
                .HasIndex(sr => sr.PlanId);

            modelBuilder.Entity<StudyRecord>()
                .HasIndex(sr => sr.SubjectId);

            modelBuilder.Entity<PlanCertificate>()
                .HasIndex(pc => pc.CertificateId);

            modelBuilder.Entity<PlanCertificate>()
                .HasIndex(pc => pc.PlanId);

            modelBuilder.Entity<SubjectCertificate>()
                .HasIndex(sc => sc.CertificateId);

            modelBuilder.Entity<SubjectCertificate>()
                .HasIndex(sc => sc.SubjectId);

            modelBuilder.Entity<CourseCertificate>()
                .HasIndex(cc => cc.CertificateId);

            modelBuilder.Entity<CourseCertificate>()
                .HasIndex(cc => cc.CourseId);

            modelBuilder.Entity<ExternalCertificate>()
                .HasIndex(ec => ec.UserId);

            modelBuilder.Entity<Request>()
                .HasIndex(r => r.RequestUserId);

            modelBuilder.Entity<Request>()
                .HasIndex(r => r.ApprovedByUserId);

            modelBuilder.Entity<Notification>()
                .HasIndex(n => n.UserId);

            modelBuilder.Entity<Report>()
                .HasIndex(r => r.GeneratedByUserId);


            modelBuilder.Entity<Session>()
                .HasIndex(s => s.UserId);

            modelBuilder.Entity<Plan>()
                .HasIndex(p => p.CreatedByUserId);

            modelBuilder.Entity<Plan>()
                .HasIndex(p => p.SpecialtyId);

            modelBuilder.Entity<Plan>()
                .HasIndex(p => p.AprovedUserId);

            modelBuilder.Entity<UserSpecialty>()
                .HasIndex(us => us.UserId);

            modelBuilder.Entity<UserSpecialty>()
                .HasIndex(us => us.SpecialtyId);

            modelBuilder.Entity<UserDepartment>()
                .HasIndex(ud => ud.UserId);

            modelBuilder.Entity<UserDepartment>()
                .HasIndex(ud => ud.DepartmentId);


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
