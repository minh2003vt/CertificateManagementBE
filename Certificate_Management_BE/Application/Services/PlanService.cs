using Application.Dto.PlanDto;
using Application.IServices;
using Application;
using Application.ServiceResponse;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class PlanService : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<List<PlanListDto>>> GetAllAsync()
        {
            var response = new ServiceResponse<List<PlanListDto>>();
            try
            {
                var plans = await _unitOfWork.PlanRepository
                    .GetByNullableExpressionWithOrderingAsync(
                        null,
                        p => p.OrderBy(x => x.PlanName)
                    );

                var planDtos = new List<PlanListDto>();
                foreach (var plan in plans)
                {
                    // Load CreatedByUser navigation property
                    User? createdByUser = null;
                    if (!string.IsNullOrEmpty(plan.CreatedByUserId))
                    {
                        createdByUser = await _unitOfWork.UserRepository
                            .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == plan.CreatedByUserId);
                    }

                    // Load Specialty navigation property
                    var specialty = await _unitOfWork.SpecialtyRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(s => s.SpecialtyId == plan.SpecialtyId);

                    planDtos.Add(new PlanListDto
                    {
                        PlanId = plan.PlanId,
                        PlanName = plan.PlanName,
                        Description = plan.Description,
                        StartDate = plan.StartDate,
                        EndDate = plan.EndDate,
                        Status = plan.Status.ToString(),
                        CreatedAt = plan.CreatedAt
                    });
                }

                response.Success = true;
                response.Data = planDtos;
                response.Message = $"Retrieved {planDtos.Count} plans successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve plans: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<PlanDetailDto>> GetByIdAsync(string planId)
        {
            var response = new ServiceResponse<PlanDetailDto>();
            try
            {
                var plan = await _unitOfWork.PlanRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(p => p.PlanId == planId);

                if (plan == null)
                {
                    response.Success = false;
                    response.Message = "Plan not found";
                    return response;
                }

                // Load CreatedByUser navigation property
                User? createdByUser = null;
                if (!string.IsNullOrEmpty(plan.CreatedByUserId))
                {
                    createdByUser = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == plan.CreatedByUserId);
                }

                // Load AprovedUser navigation property
                User? aprovedUser = null;
                if (!string.IsNullOrEmpty(plan.AprovedUserId))
                {
                    aprovedUser = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == plan.AprovedUserId);
                }

                // Load Specialty navigation property
                var specialty = await _unitOfWork.SpecialtyRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(s => s.SpecialtyId == plan.SpecialtyId);

                var planDto = new PlanDetailDto
                {
                    PlanId = plan.PlanId,
                    PlanName = plan.PlanName,
                    Description = plan.Description,
                    StartDate = plan.StartDate,
                    EndDate = plan.EndDate,
                    Status = plan.Status.ToString(),
                    CreatedByUserId = plan.CreatedByUserId,
                    CreatedByUserName = createdByUser?.FullName,
                    AprovedUserId = plan.AprovedUserId,
                    AprovedUserName = aprovedUser?.FullName,
                    ApprovedAt = plan.ApprovedAt,
                    SpecialtyId = plan.SpecialtyId,
                    SpecialtyName = specialty?.SpecialtyName,
                    CreatedAt = plan.CreatedAt
                };

                response.Success = true;
                response.Data = planDto;
                response.Message = "Plan retrieved successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve plan: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<PlanWithCoursesDto>> GetByIdWithCoursesAsync(string planId)
        {
            var response = new ServiceResponse<PlanWithCoursesDto>();
            try
            {
                var plan = await _unitOfWork.PlanRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(p => p.PlanId == planId);

                if (plan == null)
                {
                    response.Success = false;
                    response.Message = "Plan not found";
                    return response;
                }

                // Load CreatedByUser navigation property
                User? createdByUser = null;
                if (!string.IsNullOrEmpty(plan.CreatedByUserId))
                {
                    createdByUser = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == plan.CreatedByUserId);
                }

                // Load AprovedUser navigation property
                User? aprovedUser = null;
                if (!string.IsNullOrEmpty(plan.AprovedUserId))
                {
                    aprovedUser = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == plan.AprovedUserId);
                }

                // Load Specialty navigation property
                var specialty = await _unitOfWork.SpecialtyRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(s => s.SpecialtyId == plan.SpecialtyId);

                // Get all study records for this plan to get courses
                var studyRecords = await _unitOfWork.PlanCourseRepository
                    .GetByNullableExpressionWithOrderingAsync(
                        sr => sr.PlanId == planId,
                        sr => sr.OrderBy(x => x.CourseId)
                    );

                // Group by course and get unique courses
                var courseIds = studyRecords.Select(sr => sr.CourseId).Distinct().ToList();
                var courses = new List<CourseWithSubjectsDto>();

                foreach (var courseId in courseIds)
                {
                    // Get course details
                    var course = await _unitOfWork.CourseRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(c => c.CourseId == courseId);

                    if (course != null)
                    {
                        // Get subjects for this course in this plan
                        var courseSubjects = studyRecords
                            .Where(sr => sr.CourseId == courseId)
                            .Select(sr => sr.SubjectId)
                            .Distinct()
                            .ToList();

                        var subjects = new List<SubjectDto>();
                        foreach (var subjectId in courseSubjects)
                        {
                            var subject = await _unitOfWork.SubjectRepository
                                .GetSingleOrDefaultByNullableExpressionAsync(s => s.SubjectId == subjectId);

                            if (subject != null)
                            {
                                subjects.Add(new SubjectDto
                                {
                                    SubjectId = subject.SubjectId,
                                    SubjectName = subject.SubjectName,
                                    Description = subject.Description,
                                    Status = subject.Status.ToString(),
                                    CreatedAt = subject.CreatedAt
                                });
                            }
                        }

                        courses.Add(new CourseWithSubjectsDto
                        {
                            CourseId = course.CourseId,
                            CourseName = course.CourseName,
                            Description = course.Description,
                            Status = course.Status.ToString(),
                            CreatedAt = course.CreatedAt,
                            Subjects = subjects
                        });
                    }
                }

                var planDto = new PlanWithCoursesDto
                {
                    PlanId = plan.PlanId,
                    PlanName = plan.PlanName,
                    Description = plan.Description,
                    StartDate = plan.StartDate,
                    EndDate = plan.EndDate,
                    Status = plan.Status.ToString(),
                    CreatedByUserId = plan.CreatedByUserId,
                    CreatedByUserName = createdByUser?.FullName,
                    AprovedUserId = plan.AprovedUserId,
                    AprovedUserName = aprovedUser?.FullName,
                    ApprovedAt = plan.ApprovedAt,
                    SpecialtyId = plan.SpecialtyId,
                    SpecialtyName = specialty?.SpecialtyName,
                    CreatedAt = plan.CreatedAt,
                    Courses = courses
                };

                response.Success = true;
                response.Data = planDto;
                response.Message = "Plan retrieved successfully with courses and subjects";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve plan: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<PlanDetailDto>> CreateAsync(CreatePlanDto dto, string createdByUserId)
        {
            var response = new ServiceResponse<PlanDetailDto>();
            try
            {
                var validationErrors = dto.Validate();
                if (validationErrors.Errors.Any())
                {
                    response.Success = false;
                    response.Message = "Validation failed";
                    response.ErrorMessages = validationErrors.Errors;
                    return response;
                }

                var existingPlan = await _unitOfWork.PlanRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(p => p.PlanId == dto.PlanId);

                if (existingPlan != null)
                {
                    response.Success = false;
                    response.Message = $"Plan with ID '{dto.PlanId}' already exists";
                    return response;
                }

                // Verify specialty exists
                var specialty = await _unitOfWork.SpecialtyRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(s => s.SpecialtyId == dto.SpecialtyId);

                if (specialty == null)
                {
                    response.Success = false;
                    response.Message = $"Specialty with ID '{dto.SpecialtyId}' not found";
                    return response;
                }

                var plan = new Plan
                {
                    PlanId = dto.PlanId,
                    PlanName = dto.PlanName,
                    Description = dto.Description ?? string.Empty,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    CreatedByUserId = createdByUserId,
                    SpecialtyId = dto.SpecialtyId,
                    Status = PlanStatus.Pending, 
                    CreatedAt = DateTime.UtcNow.AddHours(7)
                };

                await _unitOfWork.PlanRepository.AddAsync(plan);
                await _unitOfWork.SaveChangesAsync();

                // Retrieve the created plan with user info
                var createdPlan = await _unitOfWork.PlanRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(p => p.PlanId == plan.PlanId);

                User? createdByUser = null;
                if (!string.IsNullOrEmpty(createdPlan.CreatedByUserId))
                {
                    createdByUser = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == createdPlan.CreatedByUserId);
                }

                var specialtyInfo = await _unitOfWork.SpecialtyRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(s => s.SpecialtyId == createdPlan.SpecialtyId);

                var planDto = new PlanDetailDto
                {
                    PlanId = createdPlan.PlanId,
                    PlanName = createdPlan.PlanName,
                    Description = createdPlan.Description,
                    StartDate = createdPlan.StartDate,
                    EndDate = createdPlan.EndDate,
                    Status = createdPlan.Status.ToString(),
                    CreatedByUserId = createdPlan.CreatedByUserId,
                    CreatedByUserName = createdByUser?.FullName,
                    AprovedUserId = createdPlan.AprovedUserId,
                    AprovedUserName = null, // Newly created, no approver yet
                    ApprovedAt = null,
                    SpecialtyId = createdPlan.SpecialtyId,
                    SpecialtyName = specialtyInfo?.SpecialtyName,
                    CreatedAt = createdPlan.CreatedAt
                };

                response.Success = true;
                response.Data = planDto;
                response.Message = "Plan created successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create plan: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<PlanDetailDto>> UpdateAsync(string planId, UpdatePlanDto dto)
        {
            var response = new ServiceResponse<PlanDetailDto>();
            try
            {
                var validationErrors = dto.Validate();
                if (validationErrors.Errors.Any())
                {
                    response.Success = false;
                    response.Message = "Validation failed";
                    response.ErrorMessages = validationErrors.Errors;
                    return response;
                }

                var plan = await _unitOfWork.PlanRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(p => p.PlanId == planId);

                if (plan == null)
                {
                    response.Success = false;
                    response.Message = "Plan not found";
                    return response;
                }

                // Check if plan is approved or finished - prevent modification
                if (plan.Status == PlanStatus.Approved )
                {
                    response.Success = false;
                    response.Message = "Plan has been approved, please request to modify";
                    return response;
                }
                if (plan.Status == PlanStatus.finished )
                {
                    response.Success = false;
                    response.Message = "Plan has been finished";
                    return response;
                }

                // Verify specialty exists
                var specialty = await _unitOfWork.SpecialtyRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(s => s.SpecialtyId == dto.SpecialtyId);

                if (specialty == null)
                {
                    response.Success = false;
                    response.Message = $"Specialty with ID '{dto.SpecialtyId}' not found";
                    return response;
                }

                plan.PlanName = dto.PlanName;
                plan.Description = dto.Description ?? string.Empty;
                plan.StartDate = dto.StartDate;
                plan.EndDate = dto.EndDate;
                plan.SpecialtyId = dto.SpecialtyId;

                await _unitOfWork.PlanRepository.UpdateAsync(plan);
                await _unitOfWork.SaveChangesAsync();

                // Retrieve updated plan with user info
                var updatedPlan = await _unitOfWork.PlanRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(p => p.PlanId == planId);

                User? createdByUser = null;
                if (!string.IsNullOrEmpty(updatedPlan.CreatedByUserId))
                {
                    createdByUser = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == updatedPlan.CreatedByUserId);
                }

                User? aprovedUser = null;
                if (!string.IsNullOrEmpty(updatedPlan.AprovedUserId))
                {
                    aprovedUser = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == updatedPlan.AprovedUserId);
                }

                var specialtyInfo = await _unitOfWork.SpecialtyRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(s => s.SpecialtyId == updatedPlan.SpecialtyId);

                var planDto = new PlanDetailDto
                {
                    PlanId = updatedPlan.PlanId,
                    PlanName = updatedPlan.PlanName,
                    Description = updatedPlan.Description,
                    StartDate = updatedPlan.StartDate,
                    EndDate = updatedPlan.EndDate,
                    Status = updatedPlan.Status.ToString(),
                    CreatedByUserId = updatedPlan.CreatedByUserId,
                    CreatedByUserName = createdByUser?.FullName,
                    AprovedUserId = updatedPlan.AprovedUserId,
                    AprovedUserName = aprovedUser?.FullName,
                    ApprovedAt = updatedPlan.ApprovedAt,
                    SpecialtyId = updatedPlan.SpecialtyId,
                    SpecialtyName = specialtyInfo?.SpecialtyName,
                    CreatedAt = updatedPlan.CreatedAt
                };

                response.Success = true;
                response.Data = planDto;
                response.Message = "Plan updated successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to update plan: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(string planId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var plan = await _unitOfWork.PlanRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(p => p.PlanId == planId);

                if (plan == null)
                {
                    response.Success = false;
                    response.Message = "Plan not found";
                    return response;
                }
if (plan.Status == PlanStatus.Approved )
                {
                    response.Success = false;
                    response.Message = "Plan has been approved, please request to modify";
                    return response;
                }
                if (plan.Status == PlanStatus.finished )
                {
                    response.Success = false;
                    response.Message = "Plan has been finished";
                    return response;
                }

                await _unitOfWork.PlanRepository.DeleteAsync(plan);
                await _unitOfWork.SaveChangesAsync();

                response.Success = true;
                response.Data = true;
                response.Message = "Plan deleted successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Data = false;
                response.Message = $"Failed to delete plan: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<PlanDetailDto>> ApproveAsync(string planId, string aprovedByUserId)
        {
            var response = new ServiceResponse<PlanDetailDto>();
            try
            {
                var plan = await _unitOfWork.PlanRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(p => p.PlanId == planId);

                if (plan == null)
                {
                    response.Success = false;
                    response.Message = "Plan not found";
                    return response;
                }

                plan.Status = PlanStatus.Approved;
                plan.AprovedUserId = aprovedByUserId;
                plan.ApprovedAt = DateTime.UtcNow;

                await _unitOfWork.PlanRepository.UpdateAsync(plan);
                await _unitOfWork.SaveChangesAsync();

                // Load user info for response
                var createdByUser = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == plan.CreatedByUserId);

                var aprovedUser = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == plan.AprovedUserId);

                var specialty = await _unitOfWork.SpecialtyRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(s => s.SpecialtyId == plan.SpecialtyId);

                var planDto = new PlanDetailDto
                {
                    PlanId = plan.PlanId,
                    PlanName = plan.PlanName,
                    Description = plan.Description,
                    StartDate = plan.StartDate,
                    EndDate = plan.EndDate,
                    Status = plan.Status.ToString(),
                    CreatedByUserId = plan.CreatedByUserId,
                    CreatedByUserName = createdByUser?.FullName,
                    AprovedUserId = plan.AprovedUserId,
                    AprovedUserName = aprovedUser?.FullName,
                    ApprovedAt = plan.ApprovedAt,
                    SpecialtyId = plan.SpecialtyId,
                    SpecialtyName = specialty?.SpecialtyName,
                    CreatedAt = plan.CreatedAt
                };

                response.Success = true;
                response.Data = planDto;
                response.Message = "Plan approved successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to approve plan: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<PlanDetailDto>> RejectAsync(string planId, string aprovedByUserId)
        {
            var response = new ServiceResponse<PlanDetailDto>();
            try
            {
                var plan = await _unitOfWork.PlanRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(p => p.PlanId == planId);

                if (plan == null)
                {
                    response.Success = false;
                    response.Message = "Plan not found";
                    return response;
                }

                plan.Status = PlanStatus.Rejected;
                plan.AprovedUserId = aprovedByUserId;
                plan.ApprovedAt = DateTime.UtcNow;

                await _unitOfWork.PlanRepository.UpdateAsync(plan);
                await _unitOfWork.SaveChangesAsync();

                // Load user info for response
                var createdByUser = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == plan.CreatedByUserId);

                var aprovedUser = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == plan.AprovedUserId);

                var specialty = await _unitOfWork.SpecialtyRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(s => s.SpecialtyId == plan.SpecialtyId);

                var planDto = new PlanDetailDto
                {
                    PlanId = plan.PlanId,
                    PlanName = plan.PlanName,
                    Description = plan.Description,
                    StartDate = plan.StartDate,
                    EndDate = plan.EndDate,
                    Status = plan.Status.ToString(),
                    CreatedByUserId = plan.CreatedByUserId,
                    CreatedByUserName = createdByUser?.FullName,
                    AprovedUserId = plan.AprovedUserId,
                    AprovedUserName = aprovedUser?.FullName,
                    ApprovedAt = plan.ApprovedAt,
                    SpecialtyId = plan.SpecialtyId,
                    SpecialtyName = specialty?.SpecialtyName,
                    CreatedAt = plan.CreatedAt
                };

                response.Success = true;
                response.Data = planDto;
                response.Message = "Plan rejected successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to reject plan: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<List<PlanListDto>>> GetAllByStatusAsync(string status)
        {
            var response = new ServiceResponse<List<PlanListDto>>();
            try
            {
                if (!Enum.TryParse<PlanStatus>(status, true, out var planStatus))
                {
                    response.Success = false;
                    response.Message = $"Invalid status '{status}'. Valid values are: Pending, Approved, Rejected";
                    return response;
                }

                var plans = await _unitOfWork.PlanRepository
                    .GetByNullableExpressionWithOrderingAsync(
                        p => p.Status == planStatus,
                        p => p.OrderBy(x => x.PlanName)
                    );

                var planDtos = new List<PlanListDto>();
                foreach (var plan in plans)
                {
                    // Load CreatedByUser navigation property
                    User? createdByUser = null;
                    if (!string.IsNullOrEmpty(plan.CreatedByUserId))
                    {
                        createdByUser = await _unitOfWork.UserRepository
                            .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == plan.CreatedByUserId);
                    }

                    // Load Specialty navigation property
                    var specialty = await _unitOfWork.SpecialtyRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(s => s.SpecialtyId == plan.SpecialtyId);

                    planDtos.Add(new PlanListDto
                    {
                        PlanId = plan.PlanId,
                        PlanName = plan.PlanName,
                        Description = plan.Description,
                        StartDate = plan.StartDate,
                        EndDate = plan.EndDate,
                        Status = plan.Status.ToString(),
                        CreatedAt = plan.CreatedAt
                    });
                }

                response.Success = true;
                response.Data = planDtos;
                response.Message = $"Retrieved {planDtos.Count} plans with status '{status}' successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve plans by status: {ex.Message}";
            }
            return response;
        }
    }
}

