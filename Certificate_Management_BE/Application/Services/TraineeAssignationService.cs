using Application.Dto.TraineeAssignationDto;
using Application.IServices;
using Application.ServiceResponse;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class TraineeAssignationService : ITraineeAssignationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TraineeAssignationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<TraineeAssignationDetailDto>> CreateAsync(CreateTraineeAssignationDto dto, string assignedByUserId)
        {
            var response = new ServiceResponse<TraineeAssignationDetailDto>();
            try
            {
                // 1) Validate entities exist
                var user = await _unitOfWork.UserRepository.GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == dto.TraineeId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Trainee not found";
                    return response;
                }

                var @class = await _unitOfWork.ClassRepository.GetSingleOrDefaultByNullableExpressionAsync(c => c.ClassId == dto.ClassId);
                if (@class == null)
                {
                    response.Success = false;
                    response.Message = "Class not found";
                    return response;
                }

                // 2) Determine subject from class and check user's specialty matches a CSS mapping
                var subjectId = @class.SubjectId;
                // user specialties
                var userSpecialties = await _unitOfWork.UserSpecialtyRepository
                    .GetByNullableExpressionWithOrderingAsync(us => us.UserId == dto.TraineeId);
                var specialtyIds = userSpecialties.Select(us => us.SpecialtyId).ToHashSet();

                var cssExists = await _unitOfWork.CourseSubjectSpecialtyRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(x => x.SubjectId == subjectId && specialtyIds.Contains(x.SpecialtyId));
                if (cssExists == null)
                {
                    response.Success = false;
                    response.Message = "No Course-Subject-Specialty mapping found for trainee's specialty and class subject";
                    return response;
                }
                if(cssExists.ApprovedAt == null)
                {
                    response.Success = false;
                    response.Message = "Course-Subject-Specialty not approve";
                    return response;

                }
                // 3) Ensure not already assigned (same Trainee + Class)
                var existing = await _unitOfWork.TraineeAssignationRepository
                    .GetByNullableExpressionWithOrderingAsync(t => t.TraineeId == dto.TraineeId);

                var alreadyInClass = existing.Any(t => t.ClassTraineeAssignations.Any(ct => ct.ClassId == dto.ClassId));
                if (alreadyInClass)
                {
                    response.Success = false;
                    response.Message = "Trainee already assigned to this class";
                    return response;
                }

                // 4) Create TraineeAssignation and junction row
                var assignation = new TraineeAssignation
                {
                    TraineeAssignationId = Guid.NewGuid().ToString("N"),
                    TraineeId = dto.TraineeId,
                    AssignedByUserId = assignedByUserId,
                    AssignDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow
                };
                await _unitOfWork.TraineeAssignationRepository.AddAsync(assignation);

                var classLink = new ClassTraineeAssignation
                {
                    ClassId = dto.ClassId,
                    TraineeAssignationId = assignation.TraineeAssignationId
                };
                await _unitOfWork.ClassTraineeAssignationRepository.AddAsync(classLink);

                var detail = new TraineeAssignationDetailDto
                {
                    TraineeAssignationId = assignation.TraineeAssignationId,
                    TraineeId = assignation.TraineeId,
                    ClassId = dto.ClassId,
                    OverallGradeStatus = assignation.OverallGradeStatus,
                    AssignmentKind = assignation.AssignmentKind,
                    AssignDate = assignation.AssignDate,
                    GradeDate = assignation.GradeDate,
                    UpdateDate = assignation.UpdateDate
                };

                response.Success = true;
                response.Data = detail;
                response.Message = "Trainee assigned successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<TraineeAssignationDetailDto>> GetByIdAsync(string traineeAssignationId)
        {
            var response = new ServiceResponse<TraineeAssignationDetailDto>();
            try
            {
                var ta = await _unitOfWork.TraineeAssignationRepository.GetSingleOrDefaultByNullableExpressionAsync(t => t.TraineeAssignationId == traineeAssignationId);
                if (ta == null)
                {
                    response.Success = false;
                    response.Message = "TraineeAssignation not found";
                    return response;
                }

                // find first class link if any
                var link = ta.ClassTraineeAssignations.FirstOrDefault();
                var detail = new TraineeAssignationDetailDto
                {
                    TraineeAssignationId = ta.TraineeAssignationId,
                    TraineeId = ta.TraineeId,
                    ClassId = link?.ClassId ?? 0,
                    OverallGradeStatus = ta.OverallGradeStatus,
                    AssignmentKind = ta.AssignmentKind,
                    AssignDate = ta.AssignDate,
                    GradeDate = ta.GradeDate,
                    UpdateDate = ta.UpdateDate
                };
                response.Success = true;
                response.Data = detail;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<TraineeAssignationListDto>>> GetByClassAsync(int classId)
        {
            var response = new ServiceResponse<List<TraineeAssignationListDto>>();
            try
            {
                var list = await _unitOfWork.ClassTraineeAssignationRepository
                    .GetByNullableExpressionWithOrderingAsync(ct => ct.ClassId == classId);

                var result = list
                    .Select(ct => new TraineeAssignationListDto
                    {
                        TraineeAssignationId = ct.TraineeAssignationId,
                        TraineeId = ct.TraineeAssignation?.TraineeId ?? string.Empty,
                        ClassId = classId,
                        AssignDate = ct.TraineeAssignation?.AssignDate ?? DateTime.MinValue
                    })
                    .ToList();

                response.Success = true;
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<TraineeAssignationDetailDto>> UpdateAsync(string traineeAssignationId, UpdateTraineeAssignationDto dto)
        {
            var response = new ServiceResponse<TraineeAssignationDetailDto>();
            try
            {
                var ta = await _unitOfWork.TraineeAssignationRepository.GetSingleOrDefaultByNullableExpressionAsync(t => t.TraineeAssignationId == traineeAssignationId);
                if (ta == null)
                {
                    response.Success = false;
                    response.Message = "TraineeAssignation not found";
                    return response;
                }

                if (dto.OverallGradeStatus.HasValue) ta.OverallGradeStatus = dto.OverallGradeStatus.Value;
                if (dto.AssignmentKind.HasValue) ta.AssignmentKind = dto.AssignmentKind.Value;
                ta.UpdateDate = DateTime.UtcNow;

                await _unitOfWork.TraineeAssignationRepository.UpdateAsync(ta);

                var link = ta.ClassTraineeAssignations.FirstOrDefault();
                var detail = new TraineeAssignationDetailDto
                {
                    TraineeAssignationId = ta.TraineeAssignationId,
                    TraineeId = ta.TraineeId,
                    ClassId = link?.ClassId ?? 0,
                    OverallGradeStatus = ta.OverallGradeStatus,
                    AssignmentKind = ta.AssignmentKind,
                    AssignDate = ta.AssignDate,
                    GradeDate = ta.GradeDate,
                    UpdateDate = ta.UpdateDate
                };

                response.Success = true;
                response.Data = detail;
                response.Message = "TraineeAssignation updated";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<string>> DeleteAsync(string traineeAssignationId)
        {
            var response = new ServiceResponse<string>();
            try
            {
                var ta = await _unitOfWork.TraineeAssignationRepository.GetSingleOrDefaultByNullableExpressionAsync(t => t.TraineeAssignationId == traineeAssignationId);
                if (ta == null)
                {
                    response.Success = false;
                    response.Message = "TraineeAssignation not found";
                    return response;
                }

                await _unitOfWork.TraineeAssignationRepository.DeleteAsync(ta);
                response.Success = true;
                response.Data = traineeAssignationId;
                response.Message = "TraineeAssignation deleted";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}


