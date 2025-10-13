using Application.Dto.StudyRecordDto;
using Application.IServices;
using Application.ServiceResponse;
using Domain.Entities;

namespace Application.Services
{
    public class StudyRecordService : IStudyRecordService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudyRecordService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<List<StudyRecordListDto>>> GetByPlanAndCourseAsync(string planId, string courseId)
        {
            var response = new ServiceResponse<List<StudyRecordListDto>>();
            try
            {
                var records = await _unitOfWork.PlanCourseRepository
                    .GetByNullableExpressionWithOrderingAsync(
                        sr => sr.PlanId == planId && sr.CourseId == courseId,
                        q => q.OrderBy(x => x.SubjectId)
                    );

                response.Success = true;
                response.Data = records.Select(r => new StudyRecordListDto
                {
                    PlanId = r.PlanId,
                    CourseId = r.CourseId,
                    SubjectId = r.SubjectId,
                    CreatedAt = r.CreatedAt
                }).ToList();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to get study records: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<List<StudyRecordListDto>>> CreateAsync(CreateStudyRecordDto dto, string createdByUserId)
        {
            var response = new ServiceResponse<List<StudyRecordListDto>>();
            try
            {
                // Validate plan
                var plan = await _unitOfWork.PlanRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(p => p.PlanId == dto.PlanId);
                if (plan == null)
                {
                    response.Success = false;
                    response.Message = $"Plan '{dto.PlanId}' not found";
                    return response;
                }

                // Check if plan is finished - prevent modification
                if (plan.Status == Domain.Enums.PlanStatus.finished)
                {
                    response.Success = false;
                    response.Message = "Plan has been finished, cannot modify study records";
                    return response;
                }

                // Validate course
                var course = await _unitOfWork.CourseRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(c => c.CourseId == dto.CourseId);
                if (course == null)
                {
                    response.Success = false;
                    response.Message = $"Course '{dto.CourseId}' not found";
                    return response;
                }

                // Get all subjects linked by CourseSubjectSpecialty for course and plan's specialty
                var cssList = await _unitOfWork.CourseSubjectSpecialtyRepository
                    .GetByNullableExpressionWithOrderingAsync(
                        css => css.CourseId == dto.CourseId && css.SpecialtyId == plan.SpecialtyId,
                        null
                    );

                if (!cssList.Any())
                {
                    response.Success = false;
                    response.Message = "No subjects found for Course and Plan's Specialty";
                    return response;
                }

                // Existing records to avoid duplicates
                var existing = await _unitOfWork.PlanCourseRepository
                    .GetByNullableExpressionWithOrderingAsync(
                        sr => sr.PlanId == dto.PlanId && sr.CourseId == dto.CourseId,
                        null
                    );
                var existingSubjects = existing.Select(e => e.SubjectId).ToHashSet();

                var created = new List<StudyRecord>();
                foreach (var css in cssList)
                {
                    if (existingSubjects.Contains(css.SubjectId)) continue;

                    created.Add(new StudyRecord
                    {
                        PlanId = dto.PlanId,
                        CourseId = dto.CourseId,
                        SubjectId = css.SubjectId,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    });
                }

                if (created.Count == 0)
                {
                    response.Success = true;
                    response.Data = existing.Select(r => new StudyRecordListDto
                    {
                        PlanId = r.PlanId,
                        CourseId = r.CourseId,
                        SubjectId = r.SubjectId,
                        CreatedAt = r.CreatedAt
                    }).ToList();
                    response.Message = "No new subjects to add";
                    return response;
                }

                foreach (var item in created)
                {
                    await _unitOfWork.PlanCourseRepository.AddAsync(item);
                }
                await _unitOfWork.SaveChangesAsync();

                var all = existing.Concat(created).ToList();
                response.Success = true;
                response.Data = all.Select(r => new StudyRecordListDto
                {
                    PlanId = r.PlanId,
                    CourseId = r.CourseId,
                    SubjectId = r.SubjectId,
                    CreatedAt = r.CreatedAt
                }).ToList();
                response.Message = $"Added {created.Count} study record(s)";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create study records: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteByPlanIdAsync(string planId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                // Validate plan and check status
                var plan = await _unitOfWork.PlanRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(p => p.PlanId == planId);
                if (plan == null)
                {
                    response.Success = false;
                    response.Message = $"Plan '{planId}' not found";
                    return response;
                }

                // Check if plan is finished - prevent deletion
                if (plan.Status == Domain.Enums.PlanStatus.finished)
                {
                    response.Success = false;
                    response.Message = "Plan has been finished, cannot delete study records";
                    return response;
                }

                var deleted = await _unitOfWork.PlanCourseRepository
                    .DeleteByNullableExpressionAsync(sr => sr.PlanId == planId);
                await _unitOfWork.SaveChangesAsync();

                response.Success = deleted > 0;
                response.Data = response.Success;
                response.Message = response.Success ? $"Deleted {deleted} study record(s) for plan {planId}" : "No records to delete";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to delete study records: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteSpecificAsync(string planId, string courseId, string subjectId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                // Validate plan and check status
                var plan = await _unitOfWork.PlanRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(p => p.PlanId == planId);
                if (plan == null)
                {
                    response.Success = false;
                    response.Message = $"Plan '{planId}' not found";
                    return response;
                }

                // Check if plan is finished - prevent deletion
                if (plan.Status == Domain.Enums.PlanStatus.finished)
                {
                    response.Success = false;
                    response.Message = "Plan has been finished, cannot delete study records";
                    return response;
                }

                var deleted = await _unitOfWork.PlanCourseRepository
                    .DeleteByNullableExpressionAsync(sr => sr.PlanId == planId && sr.CourseId == courseId && sr.SubjectId == subjectId);
                await _unitOfWork.SaveChangesAsync();

                response.Success = deleted > 0;
                response.Data = response.Success;
                response.Message = response.Success ? "Study record deleted successfully" : "Study record not found";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to delete study record: {ex.Message}";
            }
            return response;
        }
    }
}


