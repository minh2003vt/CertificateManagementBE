using Application.Dto.CourseDto;
using Application.Dto.SubjectDto;
using Application.IServices;
using Application;
using Application.ServiceResponse;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<List<CourseListDto>>> GetAllAsync()
        {
            var response = new ServiceResponse<List<CourseListDto>>();
            try
            {
                var courses = await _unitOfWork.CourseRepository
                    .GetByNullableExpressionWithOrderingAsync(
                        null,
                        c => c.OrderBy(x => x.CreatedAt)
                    );

                var courseDtos = courses.Select(c => new CourseListDto
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    Description = c.Description,
                    Status = c.Status.ToString(),
                    CreatedAt = c.CreatedAt
                }).ToList();

                response.Success = true;
                response.Data = courseDtos;
                response.Message = $"Retrieved {courseDtos.Count} courses successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve courses: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<CourseDetailDto>> GetByIdAsync(string courseId)
        {
            var response = new ServiceResponse<CourseDetailDto>();
            try
            {
                var course = await _unitOfWork.CourseRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(c => c.CourseId == courseId);

                if (course == null)
                {
                    response.Success = false;
                    response.Message = "Course not found";
                    return response;
                }

                // Load CreatedByUser navigation property
                if (!string.IsNullOrEmpty(course.CreatedByUserId))
                {
                    var user = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == course.CreatedByUserId);
                    course.CreatedByUser = user;
                }

                // Load AprovedUser navigation property
                if (!string.IsNullOrEmpty(course.AprovedUserId))
                {
                    var aprovedUser = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == course.AprovedUserId);
                    course.AprovedUser = aprovedUser;
                }

                var courseDto = new CourseDetailDto
                {
                    CourseId = course.CourseId,
                    CourseName = course.CourseName,
                    Description = course.Description,
                    Status = course.Status.ToString(),
                    CreatedByUserId = course.CreatedByUserId,
                    CreatedByUserName = course.CreatedByUser?.FullName,
                    AprovedUserId = course.AprovedUserId,
                    AprovedUserName = course.AprovedUser?.FullName,
                    ApprovedAt = course.ApprovedAt,
                    CreatedAt = course.CreatedAt,
                    UpdatedAt = course.UpdatedAt
                };

                response.Success = true;
                response.Data = courseDto;
                response.Message = "Course retrieved successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve course: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<CourseDetailDto>> CreateAsync(CreateCourseDto dto, string createdByUserId)
        {
            var response = new ServiceResponse<CourseDetailDto>();
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

                // Check if course ID already exists
                var existingCourse = await _unitOfWork.CourseRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(c => c.CourseId == dto.CourseId);

                if (existingCourse != null)
                {
                    response.Success = false;
                    response.Message = $"Course with ID '{dto.CourseId}' already exists";
                    return response;
                }

                // Create new course with Pending status
                var course = new Course
                {
                    CourseId = dto.CourseId,
                    CourseName = dto.CourseName,
                    Description = dto.Description,
                    CreatedByUserId = createdByUserId,
                    Status = Domain.Enums.CourseStatus.Pending,
                    // AprovedUserId and ApprovedAt will be null by default
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    UpdatedAt = DateTime.UtcNow.AddHours(7)
                };

                await _unitOfWork.CourseRepository.AddAsync(course);
                await _unitOfWork.SaveChangesAsync();

                // Retrieve the created course with user info
                var createdCourse = await _unitOfWork.CourseRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(c => c.CourseId == course.CourseId);

                // Load CreatedByUser navigation property
                if (!string.IsNullOrEmpty(createdCourse.CreatedByUserId))
                {
                    var user = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == createdCourse.CreatedByUserId);
                    createdCourse.CreatedByUser = user;
                }

                var courseDto = new CourseDetailDto
                {
                    CourseId = createdCourse.CourseId,
                    CourseName = createdCourse.CourseName,
                    Description = createdCourse.Description,
                    Status = createdCourse.Status.ToString(),
                    CreatedByUserId = createdCourse.CreatedByUserId,
                    CreatedByUserName = createdCourse.CreatedByUser?.FullName,
                    AprovedUserId = null,
                    AprovedUserName = null,
                    ApprovedAt = null,
                    CreatedAt = createdCourse.CreatedAt,
                    UpdatedAt = createdCourse.UpdatedAt
                };

                response.Success = true;
                response.Data = courseDto;
                response.Message = "Course created successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create course: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<CourseDetailDto>> UpdateAsync(string courseId, UpdateCourseDto dto)
        {
            var response = new ServiceResponse<CourseDetailDto>();
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

                // Get existing course
                var course = await _unitOfWork.CourseRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(c => c.CourseId == courseId);

                if (course == null)
                {
                    response.Success = false;
                    response.Message = "Course not found";
                    return response;
                }

                // Update course
                course.CourseName = dto.CourseName;
                course.Description = dto.Description;
                course.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.CourseRepository.UpdateAsync(course);
                await _unitOfWork.SaveChangesAsync();

                // Retrieve updated course with user info
                var updatedCourse = await _unitOfWork.CourseRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(c => c.CourseId == courseId);

                // Load CreatedByUser navigation property
                if (!string.IsNullOrEmpty(updatedCourse.CreatedByUserId))
                {
                    var user = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == updatedCourse.CreatedByUserId);
                    updatedCourse.CreatedByUser = user;
                }

                // Load AprovedUser navigation property
                if (!string.IsNullOrEmpty(updatedCourse.AprovedUserId))
                {
                    var aprovedUser = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == updatedCourse.AprovedUserId);
                    updatedCourse.AprovedUser = aprovedUser;
                }

                var courseDto = new CourseDetailDto
                {
                    CourseId = updatedCourse.CourseId,
                    CourseName = updatedCourse.CourseName,
                    Description = updatedCourse.Description,
                    Status = updatedCourse.Status.ToString(),
                    CreatedByUserId = updatedCourse.CreatedByUserId,
                    CreatedByUserName = updatedCourse.CreatedByUser?.FullName,
                    AprovedUserId = updatedCourse.AprovedUserId,
                    AprovedUserName = updatedCourse.AprovedUser?.FullName,
                    ApprovedAt = updatedCourse.ApprovedAt,
                    CreatedAt = updatedCourse.CreatedAt,
                    UpdatedAt = updatedCourse.UpdatedAt
                };

                response.Success = true;
                response.Data = courseDto;
                response.Message = "Course updated successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to update course: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(string courseId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var course = await _unitOfWork.CourseRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(c => c.CourseId == courseId);

                if (course == null)
                {
                    response.Success = false;
                    response.Message = "Course not found";
                    return response;
                }

                // Check if course is approved - prevent deletion
                if (course.Status == Domain.Enums.CourseStatus.Approved)
                {
                    response.Success = false;
                    response.Message = "Course has been approved, please request to delete";
                    return response;
                }

                await _unitOfWork.CourseRepository.DeleteAsync(course);
                await _unitOfWork.SaveChangesAsync();

                response.Success = true;
                response.Data = true;
                response.Message = "Course deleted successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to delete course: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<CourseDetailDto>> ApproveAsync(string courseId, string aprovedByUserId)
        {
            var response = new ServiceResponse<CourseDetailDto>();
            try
            {
                var course = await _unitOfWork.CourseRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(c => c.CourseId == courseId);

                if (course == null)
                {
                    response.Success = false;
                    response.Message = "Course not found";
                    return response;
                }

                // Update status to Approved
                course.Status = Domain.Enums.CourseStatus.Approved;
                course.AprovedUserId = aprovedByUserId;
                course.ApprovedAt = DateTime.UtcNow;
                course.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.CourseRepository.UpdateAsync(course);
                await _unitOfWork.SaveChangesAsync();

                // Retrieve approved course with user info
                var approvedCourse = await _unitOfWork.CourseRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(c => c.CourseId == courseId);

                // Load CreatedByUser navigation property
                if (!string.IsNullOrEmpty(approvedCourse.CreatedByUserId))
                {
                    var creatorUser = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == approvedCourse.CreatedByUserId);
                    approvedCourse.CreatedByUser = creatorUser;
                }

                // Load AprovedUser navigation property
                var aprovedUser = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == aprovedByUserId);
                approvedCourse.AprovedUser = aprovedUser;

                var courseDto = new CourseDetailDto
                {
                    CourseId = approvedCourse.CourseId,
                    CourseName = approvedCourse.CourseName,
                    Description = approvedCourse.Description,
                    Status = approvedCourse.Status.ToString(),
                    CreatedByUserId = approvedCourse.CreatedByUserId,
                    CreatedByUserName = approvedCourse.CreatedByUser?.FullName,
                    AprovedUserId = approvedCourse.AprovedUserId,
                    AprovedUserName = approvedCourse.AprovedUser?.FullName,
                    ApprovedAt = approvedCourse.ApprovedAt,
                    CreatedAt = approvedCourse.CreatedAt,
                    UpdatedAt = approvedCourse.UpdatedAt
                };

                response.Success = true;
                response.Data = courseDto;
                response.Message = "Course approved successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to approve course: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<CourseDetailDto>> RejectAsync(string courseId, string aprovedByUserId)
        {
            var response = new ServiceResponse<CourseDetailDto>();
            try
            {
                var course = await _unitOfWork.CourseRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(c => c.CourseId == courseId);

                if (course == null)
                {
                    response.Success = false;
                    response.Message = "Course not found";
                    return response;
                }

                // Update status to Rejected
                course.Status = Domain.Enums.CourseStatus.Rejected;
                course.AprovedUserId = aprovedByUserId;
                course.ApprovedAt = DateTime.UtcNow;
                course.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.CourseRepository.UpdateAsync(course);
                await _unitOfWork.SaveChangesAsync();

                // Retrieve rejected course with user info
                var rejectedCourse = await _unitOfWork.CourseRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(c => c.CourseId == courseId);

                // Load CreatedByUser navigation property
                if (!string.IsNullOrEmpty(rejectedCourse.CreatedByUserId))
                {
                    var creatorUser = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == rejectedCourse.CreatedByUserId);
                    rejectedCourse.CreatedByUser = creatorUser;
                }

                // Load AprovedUser navigation property
                var aprovedUser = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == aprovedByUserId);
                rejectedCourse.AprovedUser = aprovedUser;

                var courseDto = new CourseDetailDto
                {
                    CourseId = rejectedCourse.CourseId,
                    CourseName = rejectedCourse.CourseName,
                    Description = rejectedCourse.Description,
                    Status = rejectedCourse.Status.ToString(),
                    CreatedByUserId = rejectedCourse.CreatedByUserId,
                    CreatedByUserName = rejectedCourse.CreatedByUser?.FullName,
                    AprovedUserId = rejectedCourse.AprovedUserId,
                    AprovedUserName = rejectedCourse.AprovedUser?.FullName,
                    ApprovedAt = rejectedCourse.ApprovedAt,
                    CreatedAt = rejectedCourse.CreatedAt,
                    UpdatedAt = rejectedCourse.UpdatedAt
                };

                response.Success = true;
                response.Data = courseDto;
                response.Message = "Course rejected successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to reject course: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<List<CourseListDto>>> GetAllByStatusAsync(string status)
        {
            var response = new ServiceResponse<List<CourseListDto>>();
            try
            {
                if (!Enum.TryParse<Domain.Enums.CourseStatus>(status, true, out var courseStatus))
                {
                    response.Success = false;
                    response.Message = $"Invalid status '{status}'. Valid values are: Pending, Approved, Rejected";
                    return response;
                }

                var courses = await _unitOfWork.CourseRepository
                    .GetByNullableExpressionWithOrderingAsync(
                        c => c.Status == courseStatus,
                        c => c.OrderBy(x => x.CreatedAt)
                    );

                var courseDtos = courses.Select(c => new CourseListDto
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    Description = c.Description,
                    Status = c.Status.ToString(),
                    CreatedAt = c.CreatedAt
                }).ToList();

                response.Success = true;
                response.Data = courseDtos;
                response.Message = $"Retrieved {courseDtos.Count} courses with status '{status}' successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve courses by status: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<List<SubjectListDto>>> GetAllSubjectsByCourseIdAsync(string courseId)
        {
            var response = new ServiceResponse<List<SubjectListDto>>();
            try
            {
                // Verify course exists
                var course = await _unitOfWork.CourseRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(c => c.CourseId == courseId);

                if (course == null)
                {
                    response.Success = false;
                    response.Message = $"Course with ID '{courseId}' not found";
                    return response;
                }

                // Get all subjects for this course through CourseSubjectSpecialty
                var subjects = await _unitOfWork.CourseRepository.GetAllSubjectsByCourseIdAsync(courseId);

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
                response.Message = $"Retrieved {subjectDtos.Count} subjects for course '{courseId}' successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve subjects for course: {ex.Message}";
            }

            return response;
        }
    }
}

