using Application.Dto.SubjectDto;
using Application.IRepositories;
using Application.IServices;
using Application.ServiceResponse;
using Domain.Entities;

namespace Application.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubjectService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<List<SubjectListDto>>> GetAllAsync()
        {
            var response = new ServiceResponse<List<SubjectListDto>>();
            try
            {
                var subjects = await _unitOfWork.SubjectRepository.GetAll();

                var subjectDtos = subjects.Select(s => new SubjectListDto
                {
                    SubjectId = s.SubjectId,
                    SubjectName = s.SubjectName,
                    Description = s.Description,
                    MinTotalScore = s.MinTotalScore,
                    Status = s.Status.ToString()
                }).ToList();

                response.Success = true;
                response.Data = subjectDtos;
                response.Message = $"Retrieved {subjectDtos.Count} subjects successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve subjects: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<SubjectDetailDto>> GetByIdAsync(string subjectId)
        {
            var response = new ServiceResponse<SubjectDetailDto>();
            try
            {
                var subject = await _unitOfWork.SubjectRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(s => s.SubjectId == subjectId);

                if (subject == null)
                {
                    response.Success = false;
                    response.Message = $"Subject with ID '{subjectId}' not found";
                    return response;
                }

                // Load CreatedByUser navigation property if needed
                if (!string.IsNullOrEmpty(subject.CreatedByUserId))
                {
                    var user = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == subject.CreatedByUserId);
                    subject.CreatedByUser = user;
                }

                // Load AprovedUser navigation property if needed
                if (!string.IsNullOrEmpty(subject.AprovedUserId))
                {
                    var aprovedUser = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == subject.AprovedUserId);
                    subject.AprovedUser = aprovedUser;
                }

                var subjectDto = new SubjectDetailDto
                {
                    SubjectId = subject.SubjectId,
                    SubjectName = subject.SubjectName,
                    Description = subject.Description,
                    MinAttendance = subject.MinAttendance,
                    MinPracticeExamScore = subject.MinPracticeExamScore,
                    MinFinalExamScore = subject.MinFinalExamScore,
                    MinTotalScore = subject.MinTotalScore,
                    CreatedByUserId = subject.CreatedByUserId,
                    CreatedByUserName = subject.CreatedByUser?.FullName,
                    AprovedUserId = subject.AprovedUserId,
                    AprovedUserName = subject.AprovedUser?.FullName,
                    Status = subject.Status.ToString(),
                    ApprovedAt = subject.ApprovedAt,
                    CreatedAt = subject.CreatedAt,
                    UpdatedAt = subject.UpdatedAt
                };

                response.Success = true;
                response.Data = subjectDto;
                response.Message = "Subject retrieved successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve subject: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<SubjectDetailDto>> CreateAsync(CreateSubjectDto dto, string createdByUserId)
        {
            var response = new ServiceResponse<SubjectDetailDto>();
            try
            {
                // Validate DTO
                var validationErrors = dto.Validate();
                if (validationErrors.HasErrors)
                {
                    response.Success = false;
                    response.Message = validationErrors.GetErrorMessage();
                    return response;
                }

                // Check if subject ID already exists
                var existingSubject = await _unitOfWork.SubjectRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(s => s.SubjectId == dto.SubjectId);

                if (existingSubject != null)
                {
                    response.Success = false;
                    response.Message = $"Subject with ID '{dto.SubjectId}' already exists";
                    return response;
                }

                // Verify creator user exists
                var creatorUser = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == createdByUserId);

                if (creatorUser == null)
                {
                    response.Success = false;
                    response.Message = "Creator user not found";
                    return response;
                }

                // Auto-calculate MinTotalScore if not provided
                var minTotalScore = dto.MinTotalScore ?? dto.CalculateMinTotalScore();

                // Create new subject (Status = Pending by default)
                var subject = new Subject
                {
                    SubjectId = dto.SubjectId,
                    SubjectName = dto.SubjectName,
                    Description = dto.Description ?? string.Empty,
                    MinAttendance = dto.MinAttendance,
                    MinPracticeExamScore = dto.MinPracticeExamScore,
                    MinFinalExamScore = dto.MinFinalExamScore,
                    MinTotalScore = minTotalScore,
                    CreatedByUserId = createdByUserId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.SubjectRepository.AddAsync(subject);
                await _unitOfWork.SaveChangesAsync();

                // Return created subject details
                var subjectDto = new SubjectDetailDto
                {
                    SubjectId = subject.SubjectId,
                    SubjectName = subject.SubjectName,
                    Description = subject.Description,
                    MinAttendance = subject.MinAttendance,
                    MinPracticeExamScore = subject.MinPracticeExamScore,
                    MinFinalExamScore = subject.MinFinalExamScore,
                    MinTotalScore = subject.MinTotalScore,
                    CreatedByUserId = subject.CreatedByUserId,
                    CreatedByUserName = creatorUser.FullName,
                    AprovedUserId = null,
                    AprovedUserName = null,
                    Status = subject.Status.ToString(),
                    ApprovedAt = null,
                    CreatedAt = subject.CreatedAt,
                    UpdatedAt = subject.UpdatedAt
                };

                response.Success = true;
                response.Data = subjectDto;
                response.Message = "Subject created successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create subject: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<SubjectDetailDto>> UpdateAsync(string subjectId, UpdateSubjectDto dto)
        {
            var response = new ServiceResponse<SubjectDetailDto>();
            try
            {
                // Validate DTO
                var validationErrors = dto.Validate();
                if (validationErrors.HasErrors)
                {
                    response.Success = false;
                    response.Message = validationErrors.GetErrorMessage();
                    return response;
                }

                // Check if subject exists
                var subject = await _unitOfWork.SubjectRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(s => s.SubjectId == subjectId);

                if (subject == null)
                {
                    response.Success = false;
                    response.Message = $"Subject with ID '{subjectId}' not found";
                    return response;
                }

                // Auto-calculate MinTotalScore if not provided
                var minTotalScore = dto.MinTotalScore ?? dto.CalculateMinTotalScore();

                // Update subject properties
                subject.SubjectName = dto.SubjectName;
                subject.Description = dto.Description ?? string.Empty;
                subject.MinAttendance = dto.MinAttendance;
                subject.MinPracticeExamScore = dto.MinPracticeExamScore;
                subject.MinFinalExamScore = dto.MinFinalExamScore;
                subject.MinTotalScore = minTotalScore;
                subject.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.SubjectRepository.UpdateAsync(subject);
                await _unitOfWork.SaveChangesAsync();

                // Load CreatedByUser for response
                if (!string.IsNullOrEmpty(subject.CreatedByUserId))
                {
                    var user = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == subject.CreatedByUserId);
                    subject.CreatedByUser = user;
                }

                // Load AprovedUser for response
                if (!string.IsNullOrEmpty(subject.AprovedUserId))
                {
                    var aprovedUser = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == subject.AprovedUserId);
                    subject.AprovedUser = aprovedUser;
                }

                // Return updated subject details
                var subjectDto = new SubjectDetailDto
                {
                    SubjectId = subject.SubjectId,
                    SubjectName = subject.SubjectName,
                    Description = subject.Description,
                    MinAttendance = subject.MinAttendance,
                    MinPracticeExamScore = subject.MinPracticeExamScore,
                    MinFinalExamScore = subject.MinFinalExamScore,
                    MinTotalScore = subject.MinTotalScore,
                    CreatedByUserId = subject.CreatedByUserId,
                    CreatedByUserName = subject.CreatedByUser?.FullName,
                    AprovedUserId = subject.AprovedUserId,
                    AprovedUserName = subject.AprovedUser?.FullName,
                    Status = subject.Status.ToString(),
                    ApprovedAt = subject.ApprovedAt,
                    CreatedAt = subject.CreatedAt,
                    UpdatedAt = subject.UpdatedAt
                };

                response.Success = true;
                response.Data = subjectDto;
                response.Message = "Subject updated successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to update subject: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<string>> DeleteAsync(string subjectId)
        {
            var response = new ServiceResponse<string>();
            try
            {
                // Check if subject exists
                var subject = await _unitOfWork.SubjectRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(s => s.SubjectId == subjectId);

                if (subject == null)
                {
                    response.Success = false;
                    response.Message = $"Subject with ID '{subjectId}' not found";
                    return response;
                }

                // TODO: Check if subject is used in any courses, assignments, etc.
                // You may want to prevent deletion if subject is referenced

                await _unitOfWork.SubjectRepository.DeleteAsync(subject);
                await _unitOfWork.SaveChangesAsync();

                response.Success = true;
                response.Data = subjectId;
                response.Message = $"Subject '{subject.SubjectName}' deleted successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to delete subject: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<SubjectImportResultDto>> ImportSubjectsAsync(List<SubjectImportDto> subjects, string createdByUserId)
        {
            var response = new ServiceResponse<SubjectImportResultDto>();
            var result = new SubjectImportResultDto
            {
                TotalCount = subjects.Count
            };

            try
            {
                // Verify creator user exists
                var creatorUser = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == createdByUserId);

                if (creatorUser == null)
                {
                    response.Success = false;
                    response.Message = "Creator user not found";
                    return response;
                }

                // Get all existing subject IDs for duplicate checking
                var allExistingSubjects = await _unitOfWork.SubjectRepository.GetAll();
                var existingSubjectIds = new HashSet<string>(
                    allExistingSubjects.Select(s => s.SubjectId),
                    StringComparer.OrdinalIgnoreCase
                );

                var subjectsToAdd = new List<Subject>();
                var rowNumber = 1;

                foreach (var dto in subjects)
                {
                    var error = new SubjectImportErrorDto
                    {
                        SubjectId = dto.SubjectId,
                        SubjectName = dto.SubjectName,
                        RowNumber = rowNumber
                    };

                    // Validate DTO
                    var validationErrors = dto.Validate();
                    if (validationErrors.HasErrors)
                    {
                        error.ValidationErrors = validationErrors.Errors;
                        result.Errors.Add(error);
                        result.FailureCount++;
                        rowNumber++;
                        continue;
                    }

                    // Check for duplicates with existing subjects in database
                    if (existingSubjectIds.Contains(dto.SubjectId))
                    {
                        error.GeneralError = $"Subject ID '{dto.SubjectId}' already exists in the database";
                        result.Errors.Add(error);
                        result.FailureCount++;
                        rowNumber++;
                        continue;
                    }

                    // Check for duplicates within the import batch
                    if (subjectsToAdd.Any(s => s.SubjectId.Equals(dto.SubjectId, StringComparison.OrdinalIgnoreCase)))
                    {
                        error.GeneralError = $"Duplicate Subject ID '{dto.SubjectId}' found in import batch";
                        result.Errors.Add(error);
                        result.FailureCount++;
                        rowNumber++;
                        continue;
                    }

                    // Auto-calculate MinTotalScore if not provided
                    var calculatedMinTotalScore = dto.CalculateMinTotalScore();

                    // Create new subject entity (Status = Pending by default)
                    var subject = new Subject
                    {
                        SubjectId = dto.SubjectId,
                        SubjectName = dto.SubjectName,
                        Description = dto.Description ?? string.Empty,
                        MinAttendance = dto.MinAttendance,
                        MinPracticeExamScore = dto.MinPracticeExamScore,
                        MinFinalExamScore = dto.MinFinalExamScore,
                        MinTotalScore = calculatedMinTotalScore,
                        CreatedByUserId = createdByUserId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    subjectsToAdd.Add(subject);
                    result.SuccessfulSubjectIds.Add(dto.SubjectId);
                    result.SuccessCount++;
                    rowNumber++;
                }

                // Bulk insert all valid subjects with individual error handling
                if (subjectsToAdd.Any())
                {
                    var actualSuccessCount = 0;
                    var actualFailedSubjects = new List<string>();

                    foreach (var subject in subjectsToAdd)
                    {
                        try
                        {
                            await _unitOfWork.SubjectRepository.AddAsync(subject);
                            await _unitOfWork.SaveChangesAsync();
                            actualSuccessCount++;
                        }
                        catch (Exception saveEx)
                        {
                            // Remove from success list and add to errors
                            result.SuccessfulSubjectIds.Remove(subject.SubjectId);
                            result.SuccessCount--;
                            result.FailureCount++;
                            
                            var errorDto = new SubjectImportErrorDto
                            {
                                SubjectId = subject.SubjectId,
                                SubjectName = subject.SubjectName,
                                RowNumber = result.Errors.Count + 1,
                                GeneralError = $"Database save failed: {saveEx.InnerException?.Message ?? saveEx.Message}"
                            };
                            result.Errors.Add(errorDto);
                            actualFailedSubjects.Add(subject.SubjectId);
                        }
                    }
                }

                response.Success = true;
                response.Data = result;
                response.Message = $"Import completed: {result.SuccessCount} succeeded, {result.FailureCount} failed out of {result.TotalCount} total";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to import subjects: {ex.Message}";
                response.Data = result;
            }

            return response;
        }

        public async Task<ServiceResponse<SubjectDetailDto>> ApproveAsync(string subjectId, string aprovedByUserId)
        {
            var response = new ServiceResponse<SubjectDetailDto>();
            try
            {
                // Check if subject exists
                var subject = await _unitOfWork.SubjectRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(s => s.SubjectId == subjectId);

                if (subject == null)
                {
                    response.Success = false;
                    response.Message = $"Subject with ID '{subjectId}' not found";
                    return response;
                }

                // Check if already approved or rejected
                if (subject.Status != Domain.Enums.SubjectStatus.Pending)
                {
                    response.Success = false;
                    response.Message = $"Subject is already {subject.Status}. Only pending subjects can be approved.";
                    return response;
                }

                // Verify approver user exists
                var aprovedUser = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == aprovedByUserId);

                if (aprovedUser == null)
                {
                    response.Success = false;
                    response.Message = "Approver user not found";
                    return response;
                }

                // Update subject status
                subject.Status = Domain.Enums.SubjectStatus.Approved;
                subject.AprovedUserId = aprovedByUserId;
                subject.ApprovedAt = DateTime.UtcNow;
                subject.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.SubjectRepository.UpdateAsync(subject);
                await _unitOfWork.SaveChangesAsync();

                // Load creator user for response
                if (!string.IsNullOrEmpty(subject.CreatedByUserId))
                {
                    var creatorUser = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == subject.CreatedByUserId);
                    subject.CreatedByUser = creatorUser;
                }
                subject.AprovedUser = aprovedUser;

                var subjectDto = new SubjectDetailDto
                {
                    SubjectId = subject.SubjectId,
                    SubjectName = subject.SubjectName,
                    Description = subject.Description,
                    MinAttendance = subject.MinAttendance,
                    MinPracticeExamScore = subject.MinPracticeExamScore,
                    MinFinalExamScore = subject.MinFinalExamScore,
                    MinTotalScore = subject.MinTotalScore,
                    CreatedByUserId = subject.CreatedByUserId,
                    CreatedByUserName = subject.CreatedByUser?.FullName,
                    AprovedUserId = subject.AprovedUserId,
                    AprovedUserName = aprovedUser.FullName,
                    Status = subject.Status.ToString(),
                    ApprovedAt = subject.ApprovedAt,
                    CreatedAt = subject.CreatedAt,
                    UpdatedAt = subject.UpdatedAt
                };

                response.Success = true;
                response.Data = subjectDto;
                response.Message = $"Subject '{subject.SubjectName}' approved successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to approve subject: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<SubjectDetailDto>> RejectAsync(string subjectId, string aprovedByUserId)
        {
            var response = new ServiceResponse<SubjectDetailDto>();
            try
            {
                // Check if subject exists
                var subject = await _unitOfWork.SubjectRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(s => s.SubjectId == subjectId);

                if (subject == null)
                {
                    response.Success = false;
                    response.Message = $"Subject with ID '{subjectId}' not found";
                    return response;
                }

                // Check if already approved or rejected
                if (subject.Status != Domain.Enums.SubjectStatus.Pending)
                {
                    response.Success = false;
                    response.Message = $"Subject is already {subject.Status}. Only pending subjects can be rejected.";
                    return response;
                }

                // Verify approver user exists
                var aprovedUser = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == aprovedByUserId);

                if (aprovedUser == null)
                {
                    response.Success = false;
                    response.Message = "Approver user not found";
                    return response;
                }

                // Update subject status
                subject.Status = Domain.Enums.SubjectStatus.Rejected;
                subject.AprovedUserId = aprovedByUserId;
                subject.ApprovedAt = DateTime.UtcNow;
                subject.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.SubjectRepository.UpdateAsync(subject);
                await _unitOfWork.SaveChangesAsync();

                // Load creator user for response
                if (!string.IsNullOrEmpty(subject.CreatedByUserId))
                {
                    var creatorUser = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == subject.CreatedByUserId);
                    subject.CreatedByUser = creatorUser;
                }
                subject.AprovedUser = aprovedUser;

                var subjectDto = new SubjectDetailDto
                {
                    SubjectId = subject.SubjectId,
                    SubjectName = subject.SubjectName,
                    Description = subject.Description,
                    MinAttendance = subject.MinAttendance,
                    MinPracticeExamScore = subject.MinPracticeExamScore,
                    MinFinalExamScore = subject.MinFinalExamScore,
                    MinTotalScore = subject.MinTotalScore,
                    CreatedByUserId = subject.CreatedByUserId,
                    CreatedByUserName = subject.CreatedByUser?.FullName,
                    AprovedUserId = subject.AprovedUserId,
                    AprovedUserName = aprovedUser.FullName,
                    Status = subject.Status.ToString(),
                    ApprovedAt = subject.ApprovedAt,
                    CreatedAt = subject.CreatedAt,
                    UpdatedAt = subject.UpdatedAt
                };

                response.Success = true;
                response.Data = subjectDto;
                response.Message = $"Subject '{subject.SubjectName}' rejected successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to reject subject: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<List<SubjectListDto>>> GetAllByStatusAsync(string status)
        {
            var response = new ServiceResponse<List<SubjectListDto>>();
            try
            {
                // Parse status string to enum
                if (!Enum.TryParse<Domain.Enums.SubjectStatus>(status, true, out var subjectStatus))
                {
                    response.Success = false;
                    response.Message = $"Invalid status '{status}'. Valid values are: Pending, Approved, Rejected";
                    return response;
                }

                var subjects = await _unitOfWork.SubjectRepository
                    .GetByNullableExpressionWithOrderingAsync(
                        s => s.Status == subjectStatus,
                        s => s.OrderBy(x => x.CreatedAt)
                    );

                var subjectDtos = subjects.Select(s => new SubjectListDto
                {
                    SubjectId = s.SubjectId,
                    SubjectName = s.SubjectName,
                    Description = s.Description,
                    MinTotalScore = s.MinTotalScore,
                    Status = s.Status.ToString()
                }).ToList();

                response.Success = true;
                response.Data = subjectDtos;
                response.Message = $"Retrieved {subjectDtos.Count} subjects with status '{status}' successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve subjects by status: {ex.Message}";
            }

            return response;
        }
    }
}

