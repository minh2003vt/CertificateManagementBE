using Application.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public interface IUnitOfWork
    {
        public IUserRepository UserRepository { get; }
        public ISessionRepository SessionRepository { get; }
        public IAuditLogRepository AuditLogRepository { get; }
        public ICertificateRepository CertificateRepository { get; }
        public ICertificateTemplateRepository CertificateTemplateRepository { get; }
        public IClassRepository ClassRepository { get; }
        public IClassTraineeAssignationRepository ClassTraineeAssignationRepository { get; }
        public ICourseRepository CourseRepository { get; }
        public ICourseCertificateRepository CourseCertificateRepository { get; }
        public ICourseSubjectSpecialtyRepository CourseSubjectSpecialtyRepository { get; }
        public IDecisionRepository DecisionRepository { get; }
        public IDecisionTemplateRepository DecisionTemplateRepository { get; }
        public IDepartmentRepository DepartmentRepository { get; }
        public IExternalCertificateRepository ExternalCertificateRepository { get; }
        public IInstructorAssignationRepository InstructorAssignationRepository { get; }
        public INotificationRepository NotificationRepository { get; }
        public IPlanRepository PlanRepository { get; }
        public IPlanCertificateRepository PlanCertificateRepository { get; }
        public IPlanCourseRepository PlanCourseRepository { get; }
        public IReportRepository ReportRepository { get; }
        public IRequestRepository RequestRepository { get; }
        public IRoleRepository RoleRepository { get; }
        public ISpecialtyRepository SpecialtyRepository { get; }
        public ISubjectRepository SubjectRepository { get; }
        public ISubjectCertificateRepository SubjectCertificateRepository { get; }
        public ITraineeAssignationRepository TraineeAssignationRepository { get; }
    }
}
