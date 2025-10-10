using Application.Dto.InstructorAssignationDto;
using Application.IServices;
using Application;
using Application.ServiceResponse;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services
{
    public class InstructorAssignationService : IInstructorAssignationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InstructorAssignationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<List<InstructorAssignationListDto>>> GetAllAsync()
        {
            var response = new ServiceResponse<List<InstructorAssignationListDto>>();
            try
            {
                var assignations = await _unitOfWork.InstructorAssignationRepository
                    .GetByNullableExpressionWithOrderingAsync(
                        null,
                        a => a.OrderBy(x => x.AssignDate)
                    );

                var assignationDtos = new List<InstructorAssignationListDto>();

                foreach (var assignation in assignations)
                {
                    // Load Subject
                    var subject = await _unitOfWork.SubjectRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(s => s.SubjectId == assignation.SubjectId);

                    // Load Instructor
                    var instructor = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == assignation.InstructorId);

                    assignationDtos.Add(new InstructorAssignationListDto
                    {
                        SubjectId = assignation.SubjectId,
                        SubjectName = subject?.SubjectName ?? string.Empty,
                        InstructorId = assignation.InstructorId,
                        InstructorName = instructor?.FullName ?? string.Empty,
                        RequestStatus = assignation.RequestStatus.ToString(),
                        AssignDate = assignation.AssignDate
                    });
                }

                response.Success = true;
                response.Data = assignationDtos;
                response.Message = $"Retrieved {assignationDtos.Count} instructor assignations successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve instructor assignations: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<InstructorAssignationDetailDto>> GetByIdAsync(string subjectId, string instructorId)
        {
            var response = new ServiceResponse<InstructorAssignationDetailDto>();
            try
            {
                var assignation = await _unitOfWork.InstructorAssignationRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(a => a.SubjectId == subjectId && a.InstructorId == instructorId);

                if (assignation == null)
                {
                    response.Success = false;
                    response.Message = "Instructor assignation not found";
                    return response;
                }

                // Load Subject
                var subject = await _unitOfWork.SubjectRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(s => s.SubjectId == assignation.SubjectId);

                // Load Instructor
                var instructor = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == assignation.InstructorId);

                // Load AssignedByUser
                var assignedByUser = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == assignation.AssignedByUserId);

                var assignationDto = new InstructorAssignationDetailDto
                {
                    SubjectId = assignation.SubjectId,
                    SubjectName = subject?.SubjectName ?? string.Empty,
                    InstructorId = assignation.InstructorId,
                    InstructorName = instructor?.FullName ?? string.Empty,
                    AssignedByUserId = assignation.AssignedByUserId,
                    AssignedByUserName = assignedByUser?.FullName ?? string.Empty,
                    AssignDate = assignation.AssignDate,
                    RequestStatus = assignation.RequestStatus.ToString(),
                    Notes = assignation.Notes
                };

                response.Success = true;
                response.Data = assignationDto;
                response.Message = "Instructor assignation retrieved successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve instructor assignation: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<List<InstructorAssignationListDto>>> GetBySubjectIdAsync(string subjectId)
        {
            var response = new ServiceResponse<List<InstructorAssignationListDto>>();
            try
            {
                var assignations = await _unitOfWork.InstructorAssignationRepository
                    .GetByNullableExpressionWithOrderingAsync(
                        a => a.SubjectId == subjectId,
                        a => a.OrderBy(x => x.AssignDate)
                    );

                var assignationDtos = new List<InstructorAssignationListDto>();

                foreach (var assignation in assignations)
                {
                    // Load Subject
                    var subject = await _unitOfWork.SubjectRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(s => s.SubjectId == assignation.SubjectId);

                    // Load Instructor
                    var instructor = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == assignation.InstructorId);

                    assignationDtos.Add(new InstructorAssignationListDto
                    {
                        SubjectId = assignation.SubjectId,
                        SubjectName = subject?.SubjectName ?? string.Empty,
                        InstructorId = assignation.InstructorId,
                        InstructorName = instructor?.FullName ?? string.Empty,
                        RequestStatus = assignation.RequestStatus.ToString(),
                        AssignDate = assignation.AssignDate
                    });
                }

                response.Success = true;
                response.Data = assignationDtos;
                response.Message = $"Retrieved {assignationDtos.Count} instructor assignations for subject successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve instructor assignations by subject: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<List<InstructorAssignationListDto>>> GetByInstructorIdAsync(string instructorId)
        {
            var response = new ServiceResponse<List<InstructorAssignationListDto>>();
            try
            {
                var assignations = await _unitOfWork.InstructorAssignationRepository
                    .GetByNullableExpressionWithOrderingAsync(
                        a => a.InstructorId == instructorId,
                        a => a.OrderBy(x => x.AssignDate)
                    );

                var assignationDtos = new List<InstructorAssignationListDto>();

                foreach (var assignation in assignations)
                {
                    // Load Subject
                    var subject = await _unitOfWork.SubjectRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(s => s.SubjectId == assignation.SubjectId);

                    // Load Instructor
                    var instructor = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == assignation.InstructorId);

                    assignationDtos.Add(new InstructorAssignationListDto
                    {
                        SubjectId = assignation.SubjectId,
                        SubjectName = subject?.SubjectName ?? string.Empty,
                        InstructorId = assignation.InstructorId,
                        InstructorName = instructor?.FullName ?? string.Empty,
                        RequestStatus = assignation.RequestStatus.ToString(),
                        AssignDate = assignation.AssignDate
                    });
                }

                response.Success = true;
                response.Data = assignationDtos;
                response.Message = $"Retrieved {assignationDtos.Count} instructor assignations for instructor successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve instructor assignations by instructor: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<InstructorAssignationDetailDto>> CreateAsync(CreateInstructorAssignationDto dto, string assignedByUserId)
        {
            var response = new ServiceResponse<InstructorAssignationDetailDto>();
            try
            {
                // Validate DTO
                var validationErrors = dto.Validate();
                if (validationErrors.HasErrors)
                {
                    response.Success = false;
                    response.Message = "Validation failed";
                    response.ErrorMessages = validationErrors.Errors;
                    return response;
                }

                // Check if assignation already exists
                var existingAssignation = await _unitOfWork.InstructorAssignationRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(a => a.SubjectId == dto.SubjectId && a.InstructorId == dto.InstructorId);

                if (existingAssignation != null)
                {
                    response.Success = false;
                    response.Message = $"Instructor assignation for Subject '{dto.SubjectId}' and Instructor '{dto.InstructorId}' already exists";
                    return response;
                }

                // Verify Subject exists
                var subject = await _unitOfWork.SubjectRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(s => s.SubjectId == dto.SubjectId);

                if (subject == null)
                {
                    response.Success = false;
                    response.Message = $"Subject with ID '{dto.SubjectId}' not found";
                    return response;
                }

                // Verify Instructor exists
                var instructor = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == dto.InstructorId);

                if (instructor == null)
                {
                    response.Success = false;
                    response.Message = $"Instructor with ID '{dto.InstructorId}' not found";
                    return response;
                }

                // Create new assignation with default values
                var assignation = new InstructorAssignation
                {
                    SubjectId = dto.SubjectId,
                    InstructorId = dto.InstructorId,
                    AssignedByUserId = assignedByUserId,
                    AssignDate = DateTime.UtcNow, // Set to current date/time
                    RequestStatus = RequestStatus.Pending, // Default to Pending
                    Notes = dto.Notes ?? string.Empty
                };

                await _unitOfWork.InstructorAssignationRepository.AddAsync(assignation);
                await _unitOfWork.SaveChangesAsync();

                // Load AssignedByUser
                var assignedByUser = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == assignedByUserId);

                var assignationDto = new InstructorAssignationDetailDto
                {
                    SubjectId = assignation.SubjectId,
                    SubjectName = subject.SubjectName,
                    InstructorId = assignation.InstructorId,
                    InstructorName = instructor.FullName,
                    AssignedByUserId = assignation.AssignedByUserId,
                    AssignedByUserName = assignedByUser?.FullName ?? string.Empty,
                    AssignDate = assignation.AssignDate,
                    RequestStatus = assignation.RequestStatus.ToString(),
                    Notes = assignation.Notes
                };

                response.Success = true;
                response.Data = assignationDto;
                response.Message = "Instructor assignation created successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create instructor assignation: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(string subjectId, string instructorId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var assignation = await _unitOfWork.InstructorAssignationRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(a => a.SubjectId == subjectId && a.InstructorId == instructorId);

                if (assignation == null)
                {
                    response.Success = false;
                    response.Message = "Instructor assignation not found";
                    return response;
                }

                await _unitOfWork.InstructorAssignationRepository.DeleteAsync(assignation);
                await _unitOfWork.SaveChangesAsync();

                response.Success = true;
                response.Data = true;
                response.Message = "Instructor assignation deleted successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to delete instructor assignation: {ex.Message}";
            }

            return response;
        }
    }
}

