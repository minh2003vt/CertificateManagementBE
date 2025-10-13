using Application;
using Application.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Context _context;
        private readonly IUserRepository _userRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly ICertificateRepository _certificateRepository;
        private readonly ICertificateTemplateRepository _certificateTemplateRepository;
        private readonly IClassRepository _classRepository;
        private readonly IClassTraineeAssignationRepository _classTraineeAssignationRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ICourseCertificateRepository _courseCertificateRepository;
        private readonly ICourseSubjectSpecialtyRepository _courseSubjectSpecialtyRepository;
        private readonly IDecisionRepository _decisionRepository;
        private readonly IDecisionTemplateRepository _decisionTemplateRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IExternalCertificateRepository _externalCertificateRepository;
        private readonly IInstructorAssignationRepository _instructorAssignationRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IPlanRepository _planRepository;
        private readonly IPlanCertificateRepository _planCertificateRepository;
        private readonly IStudyRecordRepository _planCourseRepository;
        private readonly IReportRepository _reportRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IRequestEntityRepository _requestEntityRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ISpecialtyRepository _specialtyRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly ISubjectCertificateRepository _subjectCertificateRepository;
        private readonly ITraineeAssignationRepository _traineeAssignationRepository;
        private readonly IUserSpecialtyRepository _userSpecialtyRepository;
        private readonly IUserDepartmentRepository _userDepartmentRepository;
        public UnitOfWork(
            Context context,
            IUserRepository userRepository,
            ISessionRepository sessionRepository,
            ICertificateRepository certificateRepository,
            ICertificateTemplateRepository certificateTemplateRepository,
            IClassRepository classRepository,
            IClassTraineeAssignationRepository classTraineeAssignationRepository,
            ICourseRepository courseRepository,
            ICourseCertificateRepository courseCertificateRepository,
            ICourseSubjectSpecialtyRepository courseSubjectSpecialtyRepository,
            IDecisionRepository decisionRepository,
            IDecisionTemplateRepository decisionTemplateRepository,
            IDepartmentRepository departmentRepository,
            IExternalCertificateRepository externalCertificateRepository,
            IInstructorAssignationRepository instructorAssignationRepository,
            INotificationRepository notificationRepository,
            IPlanRepository planRepository,
            IPlanCertificateRepository planCertificateRepository,
            IStudyRecordRepository planCourseRepository,
            IReportRepository reportRepository,
            IRequestRepository requestRepository,
            IRequestEntityRepository requestEntityRepository,
            IRoleRepository roleRepository,
            ISpecialtyRepository specialtyRepository,
            ISubjectRepository subjectRepository,
            ISubjectCertificateRepository subjectCertificateRepository,
            ITraineeAssignationRepository traineeAssignationRepository,
            IUserSpecialtyRepository userSpecialtyRepository,
            IUserDepartmentRepository userDepartmentRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
            _certificateRepository = certificateRepository;
            _certificateTemplateRepository = certificateTemplateRepository;
            _classRepository = classRepository;
            _classTraineeAssignationRepository = classTraineeAssignationRepository;
            _courseRepository = courseRepository;
            _courseCertificateRepository = courseCertificateRepository;
            _courseSubjectSpecialtyRepository = courseSubjectSpecialtyRepository;
            _decisionRepository = decisionRepository;
            _decisionTemplateRepository = decisionTemplateRepository;
            _departmentRepository = departmentRepository;
            _externalCertificateRepository = externalCertificateRepository;
            _instructorAssignationRepository = instructorAssignationRepository;
            _notificationRepository = notificationRepository;
            _planRepository = planRepository;
            _planCertificateRepository = planCertificateRepository;
            _planCourseRepository = planCourseRepository;
            _reportRepository = reportRepository;
            _requestRepository = requestRepository;
            _requestEntityRepository = requestEntityRepository;
            _roleRepository = roleRepository;
            _specialtyRepository = specialtyRepository;
            _subjectRepository = subjectRepository;
            _subjectCertificateRepository = subjectCertificateRepository;
            _traineeAssignationRepository = traineeAssignationRepository;
            _userSpecialtyRepository = userSpecialtyRepository;
            _userDepartmentRepository = userDepartmentRepository;
        }
        public object Context => _context;
        public IUserRepository UserRepository => _userRepository;
        public ISessionRepository SessionRepository => _sessionRepository;
        public ICertificateRepository CertificateRepository => _certificateRepository;
        public ICertificateTemplateRepository CertificateTemplateRepository => _certificateTemplateRepository;
        public IClassRepository ClassRepository => _classRepository;
        public IClassTraineeAssignationRepository ClassTraineeAssignationRepository => _classTraineeAssignationRepository;
        public ICourseRepository CourseRepository => _courseRepository;
        public ICourseCertificateRepository CourseCertificateRepository => _courseCertificateRepository;
        public ICourseSubjectSpecialtyRepository CourseSubjectSpecialtyRepository => _courseSubjectSpecialtyRepository;
        public IDecisionRepository DecisionRepository => _decisionRepository;
        public IDecisionTemplateRepository DecisionTemplateRepository => _decisionTemplateRepository;
        public IDepartmentRepository DepartmentRepository => _departmentRepository;
        public IExternalCertificateRepository ExternalCertificateRepository => _externalCertificateRepository;
        public IInstructorAssignationRepository InstructorAssignationRepository => _instructorAssignationRepository;
        public INotificationRepository NotificationRepository => _notificationRepository;
        public IPlanRepository PlanRepository => _planRepository;
        public IPlanCertificateRepository PlanCertificateRepository => _planCertificateRepository;
        public IStudyRecordRepository PlanCourseRepository => _planCourseRepository;
        public IReportRepository ReportRepository => _reportRepository;
        public IRequestRepository RequestRepository => _requestRepository;
        public IRequestEntityRepository RequestEntityRepository => _requestEntityRepository;
        public IRoleRepository RoleRepository => _roleRepository;
        public ISpecialtyRepository SpecialtyRepository => _specialtyRepository;
        public ISubjectRepository SubjectRepository => _subjectRepository;
        public ISubjectCertificateRepository SubjectCertificateRepository => _subjectCertificateRepository;
        public ITraineeAssignationRepository TraineeAssignationRepository => _traineeAssignationRepository;
        public IUserSpecialtyRepository UserSpecialtyRepository => _userSpecialtyRepository;
        public IUserDepartmentRepository UserDepartmentRepository => _userDepartmentRepository;

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
