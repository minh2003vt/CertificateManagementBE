using Application.Dto.TraineePlanEnrollmentDto;
using Application.IRepositories;
using Application.IServices;
using Application.ServiceResponse;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class TraineePlanEnrollmentService : ITraineePlanEnrollmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TraineePlanEnrollmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<TraineePlanEnrollmentDetailDto>> EnrollTraineeAsync(CreateTraineePlanEnrollmentDto dto, string enrolledByUserId)
        {
            var response = new ServiceResponse<TraineePlanEnrollmentDetailDto>();
            try
            {
                // Check if trainee exists
                var trainee = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == dto.TraineeId);
                if (trainee == null)
                {
                    response.Success = false;
                    response.Message = "Trainee not found.";
                    return response;
                }

                // Check if plan exists
                var plan = await _unitOfWork.PlanRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(p => p.PlanId == dto.PlanId);
                if (plan == null)
                {
                    response.Success = false;
                    response.Message = "Plan not found.";
                    return response;
                }

                // Check if trainee is already enrolled in this plan
                var existingEnrollment = await _unitOfWork.TraineePlanEnrollmentRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(tpe => 
                        tpe.TraineeId == dto.TraineeId && 
                        tpe.PlanId == dto.PlanId && 
                        tpe.IsActive);
                
                if (existingEnrollment != null)
                {
                    response.Success = false;
                    response.Message = "Trainee is already enrolled in this plan.";
                    return response;
                }

                // Create enrollment
                var enrollment = new TraineePlanEnrollment
                {
                    TraineePlanEnrollmentId = Guid.NewGuid().ToString(),
                    TraineeId = dto.TraineeId,
                    PlanId = dto.PlanId,
                    EnrolledByUserId = enrolledByUserId,
                    EnrollmentDate = DateTime.UtcNow.AddHours(7),
                    IsActive = true,
                    Notes = dto.Notes,
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    UpdatedAt = DateTime.UtcNow.AddHours(7)
                };

                await _unitOfWork.TraineePlanEnrollmentRepository.AddAsync(enrollment);
                await _unitOfWork.SaveChangesAsync();

                // Get enrolled by user info
                var enrolledByUser = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == enrolledByUserId);

                var detail = new TraineePlanEnrollmentDetailDto
                {
                    TraineePlanEnrollmentId = enrollment.TraineePlanEnrollmentId,
                    TraineeId = enrollment.TraineeId,
                    TraineeName = trainee.FullName,
                    PlanId = enrollment.PlanId,
                    PlanName = plan.PlanName,
                    EnrolledByUserId = enrollment.EnrolledByUserId,
                    EnrolledByUserName = enrolledByUser?.FullName ?? "",
                    EnrollmentDate = enrollment.EnrollmentDate,
                    CompletionDate = enrollment.CompletionDate,
                    IsActive = enrollment.IsActive,
                    Notes = enrollment.Notes,
                    CreatedAt = enrollment.CreatedAt,
                    UpdatedAt = enrollment.UpdatedAt
                };

                response.Success = true;
                response.Data = detail;
                response.Message = "Trainee enrolled in plan successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to enroll trainee: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<List<TraineePlanEnrollmentDetailDto>>> GetTraineeEnrollmentsAsync(string traineeId)
        {
            var response = new ServiceResponse<List<TraineePlanEnrollmentDetailDto>>();
            try
            {
                var enrollments = await _unitOfWork.TraineePlanEnrollmentRepository
                    .GetByNullableExpressionWithOrderingAsync(tpe => tpe.TraineeId == traineeId);

                var details = new List<TraineePlanEnrollmentDetailDto>();
                foreach (var enrollment in enrollments)
                {
                    var trainee = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == enrollment.TraineeId);
                    var plan = await _unitOfWork.PlanRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(p => p.PlanId == enrollment.PlanId);
                    var enrolledByUser = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == enrollment.EnrolledByUserId);

                    details.Add(new TraineePlanEnrollmentDetailDto
                    {
                        TraineePlanEnrollmentId = enrollment.TraineePlanEnrollmentId,
                        TraineeId = enrollment.TraineeId,
                        TraineeName = trainee?.FullName ?? "",
                        PlanId = enrollment.PlanId,
                        PlanName = plan?.PlanName ?? "",
                        EnrolledByUserId = enrollment.EnrolledByUserId,
                        EnrolledByUserName = enrolledByUser?.FullName ?? "",
                        EnrollmentDate = enrollment.EnrollmentDate,
                        CompletionDate = enrollment.CompletionDate,
                        IsActive = enrollment.IsActive,
                        Notes = enrollment.Notes,
                        CreatedAt = enrollment.CreatedAt,
                        UpdatedAt = enrollment.UpdatedAt
                    });
                }

                response.Success = true;
                response.Data = details;
                response.Message = "Trainee enrollments retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve trainee enrollments: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<List<TraineePlanEnrollmentDetailDto>>> GetPlanEnrollmentsAsync(string planId)
        {
            var response = new ServiceResponse<List<TraineePlanEnrollmentDetailDto>>();
            try
            {
                var enrollments = await _unitOfWork.TraineePlanEnrollmentRepository
                    .GetByNullableExpressionWithOrderingAsync(tpe => tpe.PlanId == planId);

                var details = new List<TraineePlanEnrollmentDetailDto>();
                foreach (var enrollment in enrollments)
                {
                    var trainee = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == enrollment.TraineeId);
                    var plan = await _unitOfWork.PlanRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(p => p.PlanId == enrollment.PlanId);
                    var enrolledByUser = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == enrollment.EnrolledByUserId);

                    details.Add(new TraineePlanEnrollmentDetailDto
                    {
                        TraineePlanEnrollmentId = enrollment.TraineePlanEnrollmentId,
                        TraineeId = enrollment.TraineeId,
                        TraineeName = trainee?.FullName ?? "",
                        PlanId = enrollment.PlanId,
                        PlanName = plan?.PlanName ?? "",
                        EnrolledByUserId = enrollment.EnrolledByUserId,
                        EnrolledByUserName = enrolledByUser?.FullName ?? "",
                        EnrollmentDate = enrollment.EnrollmentDate,
                        CompletionDate = enrollment.CompletionDate,
                        IsActive = enrollment.IsActive,
                        Notes = enrollment.Notes,
                        CreatedAt = enrollment.CreatedAt,
                        UpdatedAt = enrollment.UpdatedAt
                    });
                }

                response.Success = true;
                response.Data = details;
                response.Message = "Plan enrollments retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve plan enrollments: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> CompleteEnrollmentAsync(string enrollmentId, string completedByUserId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var enrollment = await _unitOfWork.TraineePlanEnrollmentRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(tpe => tpe.TraineePlanEnrollmentId == enrollmentId);

                if (enrollment == null)
                {
                    response.Success = false;
                    response.Message = "Enrollment not found.";
                    return response;
                }

                enrollment.CompletionDate = DateTime.UtcNow.AddHours(7);
                enrollment.IsActive = false;
                enrollment.UpdatedAt = DateTime.UtcNow.AddHours(7);

                await _unitOfWork.TraineePlanEnrollmentRepository.UpdateAsync(enrollment);
                await _unitOfWork.SaveChangesAsync();

                response.Success = true;
                response.Data = true;
                response.Message = "Enrollment completed successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to complete enrollment: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> DeactivateEnrollmentAsync(string enrollmentId, string deactivatedByUserId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var enrollment = await _unitOfWork.TraineePlanEnrollmentRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(tpe => tpe.TraineePlanEnrollmentId == enrollmentId);

                if (enrollment == null)
                {
                    response.Success = false;
                    response.Message = "Enrollment not found.";
                    return response;
                }

                enrollment.IsActive = false;
                enrollment.UpdatedAt = DateTime.UtcNow.AddHours(7);

                await _unitOfWork.TraineePlanEnrollmentRepository.UpdateAsync(enrollment);
                await _unitOfWork.SaveChangesAsync();

                response.Success = true;
                response.Data = true;
                response.Message = "Enrollment deactivated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to deactivate enrollment: {ex.Message}";
            }

            return response;
        }
    }
}
