using Application.Dto.CertificateEligibilityDto;
using Application.IRepositories;
using Application.IServices;
using Application.ServiceResponse;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CertificateEligibilityService : ICertificateEligibilityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CertificateEligibilityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<List<CertificateEligibilityDto>>> CheckTraineeCertificateEligibilityAsync(string traineeId)
        {
            var response = new ServiceResponse<List<CertificateEligibilityDto>>();
            try
            {
                // Get all trainee's completed subjects (pass status)
                var completedSubjects = await GetTraineeCompletedSubjectsAsync(traineeId);
                
                // Get all plans that trainee is enrolled in
                var traineePlans = await GetTraineeEnrolledPlansAsync(traineeId);
                
                var eligibleCertificates = new List<CertificateEligibilityDto>();

                foreach (var plan in traineePlans)
                {
                    // Check Plan-level certificates
                    await CheckPlanCertificatesAsync(plan, completedSubjects, eligibleCertificates);
                    
                    // Check Course-level certificates within this plan
                    await CheckCourseCertificatesAsync(plan, completedSubjects, eligibleCertificates);
                    
                    // Check Subject-level certificates within this plan
                    await CheckSubjectCertificatesAsync(plan, completedSubjects, eligibleCertificates);
                }

                response.Success = true;
                response.Data = eligibleCertificates;
                response.Message = $"Found {eligibleCertificates.Count} eligible certificates";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to check certificate eligibility: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> ProcessTraineeSubjectCompletionAsync(string traineeId, string subjectId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                // This method should be called when a trainee passes a subject
                // It will check if they're now eligible for any certificates
                
                var eligibilityResult = await CheckTraineeCertificateEligibilityAsync(traineeId);
                
                if (eligibilityResult.Success)
                {
                    // Here you could trigger certificate generation or notifications
                    // For now, just return success
                    response.Success = true;
                    response.Data = true;
                    response.Message = "Subject completion processed successfully";
                }
                else
                {
                    response.Success = false;
                    response.Message = eligibilityResult.Message;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to process subject completion: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<List<CertificateEligibilityDto>>> GetEligibleCertificatesForTraineeAsync(string traineeId)
        {
            return await CheckTraineeCertificateEligibilityAsync(traineeId);
        }

        #region Private Helper Methods

        private async Task<List<SubjectCompletionInfo>> GetTraineeCompletedSubjectsAsync(string traineeId)
        {
            var context = _unitOfWork.Context as DbContext;
            var completedSubjects = new List<SubjectCompletionInfo>();

            // Get all trainee assignations with pass status
            var traineeAssignations = await _unitOfWork.TraineeAssignationRepository
                .GetByNullableExpressionWithOrderingAsync(ta => 
                    ta.TraineeId == traineeId && 
                    ta.OverallGradeStatus == OverallGradeStatus.Pass);

            foreach (var assignation in traineeAssignations)
            {
                // Get the class and subject for each assignation
                var classTraineeAssignations = await _unitOfWork.ClassTraineeAssignationRepository
                    .GetByNullableExpressionWithOrderingAsync(cta => cta.TraineeAssignationId == assignation.TraineeAssignationId);

                foreach (var cta in classTraineeAssignations)
                {
                    var classEntity = await _unitOfWork.ClassRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(c => c.ClassId == cta.ClassId);

                    if (classEntity != null)
                    {
                        var subject = await _unitOfWork.SubjectRepository
                            .GetSingleOrDefaultByNullableExpressionAsync(s => s.SubjectId == classEntity.SubjectId);

                        if (subject != null)
                        {
                            completedSubjects.Add(new SubjectCompletionInfo
                            {
                                SubjectId = subject.SubjectId,
                                SubjectName = subject.SubjectName,
                                CompletionDate = assignation.GradeDate
                            });
                        }
                    }
                }
            }

            return completedSubjects;
        }

        private async Task<List<Plan>> GetTraineeEnrolledPlansAsync(string traineeId)
        {
            // Get trainee's active plan enrollments
            var enrollments = await _unitOfWork.TraineePlanEnrollmentRepository
                .GetByNullableExpressionWithOrderingAsync(tpe => 
                    tpe.TraineeId == traineeId && tpe.IsActive);

            var plans = new List<Plan>();
            foreach (var enrollment in enrollments)
            {
                var plan = await _unitOfWork.PlanRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(p => p.PlanId == enrollment.PlanId);
                if (plan != null)
                {
                    plans.Add(plan);
                }
            }

            return plans;
        }

        private async Task CheckPlanCertificatesAsync(Plan plan, List<SubjectCompletionInfo> completedSubjects, List<CertificateEligibilityDto> eligibleCertificates)
        {
            var context = _unitOfWork.Context as DbContext;
            
            // Get all plan certificates
            var planCertificates = await context.Set<PlanCertificate>()
                .Include(pc => pc.Certificate)
                .Where(pc => pc.PlanId == plan.PlanId)
                .ToListAsync();

            foreach (var planCert in planCertificates)
            {
                var certificate = planCert.Certificate;
                
                // Get all subjects required for this plan
                var requiredSubjects = await context.Set<StudyRecord>()
                    .Include(sr => sr.Subject)
                    .Where(sr => sr.PlanId == plan.PlanId)
                    .Select(sr => new { sr.SubjectId, sr.Subject.SubjectName })
                    .ToListAsync();

                var missingRequirements = new List<string>();
                var isEligible = true;

                foreach (var requiredSubject in requiredSubjects)
                {
                    if (!completedSubjects.Any(cs => cs.SubjectId == requiredSubject.SubjectId))
                    {
                        missingRequirements.Add($"Subject: {requiredSubject.SubjectName}");
                        isEligible = false;
                    }
                }

                eligibleCertificates.Add(new CertificateEligibilityDto
                {
                    CertificateId = certificate.CertificateId,
                    CertificateName = certificate.CertificateCode,
                    PlanId = plan.PlanId,
                    PlanName = plan.PlanName,
                    CourseId = "",
                    CourseName = "",
                    SubjectId = "",
                    SubjectName = "",
                    IsEligible = isEligible,
                    MissingRequirements = missingRequirements,
                    CertificateType = CertificateType.PlanCertificate
                });
            }
        }

        private async Task CheckCourseCertificatesAsync(Plan plan, List<SubjectCompletionInfo> completedSubjects, List<CertificateEligibilityDto> eligibleCertificates)
        {
            var context = _unitOfWork.Context as DbContext;
            
            // Get all courses in this plan
            var courses = await context.Set<StudyRecord>()
                .Include(sr => sr.Course)
                .Where(sr => sr.PlanId == plan.PlanId)
                .Select(sr => new { sr.CourseId, sr.Course.CourseName })
                .Distinct()
                .ToListAsync();

            foreach (var course in courses)
            {
                // Get all course certificates
                var courseCertificates = await context.Set<CourseCertificate>()
                    .Include(cc => cc.Certificate)
                    .Where(cc => cc.CourseId == course.CourseId)
                    .ToListAsync();

                foreach (var courseCert in courseCertificates)
                {
                    var certificate = courseCert.Certificate;
                    
                    // Get all subjects required for this course
                    var requiredSubjects = await context.Set<StudyRecord>()
                        .Include(sr => sr.Subject)
                        .Where(sr => sr.PlanId == plan.PlanId && sr.CourseId == course.CourseId)
                        .Select(sr => new { sr.SubjectId, sr.Subject.SubjectName })
                        .ToListAsync();

                    var missingRequirements = new List<string>();
                    var isEligible = true;

                    foreach (var requiredSubject in requiredSubjects)
                    {
                        if (!completedSubjects.Any(cs => cs.SubjectId == requiredSubject.SubjectId))
                        {
                            missingRequirements.Add($"Subject: {requiredSubject.SubjectName}");
                            isEligible = false;
                        }
                    }

                    eligibleCertificates.Add(new CertificateEligibilityDto
                    {
                        CertificateId = certificate.CertificateId,
                        CertificateName = certificate.CertificateCode,
                        PlanId = plan.PlanId,
                        PlanName = plan.PlanName,
                        CourseId = course.CourseId,
                        CourseName = course.CourseName,
                        SubjectId = "",
                        SubjectName = "",
                        IsEligible = isEligible,
                        MissingRequirements = missingRequirements,
                        CertificateType = CertificateType.CourseCertificate
                    });
                }
            }
        }

        private async Task CheckSubjectCertificatesAsync(Plan plan, List<SubjectCompletionInfo> completedSubjects, List<CertificateEligibilityDto> eligibleCertificates)
        {
            var context = _unitOfWork.Context as DbContext;
            
            // Get all subjects in this plan
            var subjects = await context.Set<StudyRecord>()
                .Include(sr => sr.Subject)
                .Where(sr => sr.PlanId == plan.PlanId)
                .Select(sr => new { sr.SubjectId, sr.Subject.SubjectName })
                .Distinct()
                .ToListAsync();

            foreach (var subject in subjects)
            {
                // Get all subject certificates
                var subjectCertificates = await context.Set<SubjectCertificate>()
                    .Include(sc => sc.Certificate)
                    .Where(sc => sc.SubjectId == subject.SubjectId)
                    .ToListAsync();

                foreach (var subjectCert in subjectCertificates)
                {
                    var certificate = subjectCert.Certificate;
                    
                    // Check if this specific subject is completed
                    var isCompleted = completedSubjects.Any(cs => cs.SubjectId == subject.SubjectId);
                    
                    var missingRequirements = new List<string>();
                    if (!isCompleted)
                    {
                        missingRequirements.Add($"Subject: {subject.SubjectName}");
                    }

                    eligibleCertificates.Add(new CertificateEligibilityDto
                    {
                        CertificateId = certificate.CertificateId,
                        CertificateName = certificate.CertificateCode,
                        PlanId = plan.PlanId,
                        PlanName = plan.PlanName,
                        CourseId = "",
                        CourseName = "",
                        SubjectId = subject.SubjectId,
                        SubjectName = subject.SubjectName,
                        IsEligible = isCompleted,
                        MissingRequirements = missingRequirements,
                        CertificateType = CertificateType.SubjectCertificate
                    });
                }
            }
        }

        #endregion
    }

    // Helper class for tracking subject completion
    public class SubjectCompletionInfo
    {
        public string SubjectId { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public DateTime CompletionDate { get; set; }
    }
}
