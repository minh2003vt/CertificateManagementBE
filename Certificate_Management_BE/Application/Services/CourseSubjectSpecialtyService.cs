using Application.Dto.CourseSubjectSpecialtyDto;
using Application.IServices;
using Application;
using Application.ServiceResponse;
using Domain.Entities;

namespace Application.Services
{
    public class CourseSubjectSpecialtyService : ICourseSubjectSpecialtyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseSubjectSpecialtyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<List<CourseSubjectSpecialtyListDto>>> GetAllAsync()
        {
            var response = new ServiceResponse<List<CourseSubjectSpecialtyListDto>>();
            try
            {
                var relationships = await _unitOfWork.CourseSubjectSpecialtyRepository
                    .GetByNullableExpressionWithOrderingAsync(
                        null,
                        r => r.OrderBy(x => x.CreatedAt)
                    );

                var relationshipDtos = new List<CourseSubjectSpecialtyListDto>();

                foreach (var relationship in relationships)
                {
                    // Load Specialty
                    var specialty = await _unitOfWork.SpecialtyRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(s => s.SpecialtyId == relationship.SpecialtyId);

                    // Load Subject
                    var subject = await _unitOfWork.SubjectRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(s => s.SubjectId == relationship.SubjectId);

                    // Load Course
                    var course = await _unitOfWork.CourseRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(c => c.CourseId == relationship.CourseId);

                    relationshipDtos.Add(new CourseSubjectSpecialtyListDto
                    {
                        SpecialtyId = relationship.SpecialtyId,
                        SpecialtyName = specialty?.SpecialtyName ?? string.Empty,
                        SubjectId = relationship.SubjectId,
                        SubjectName = subject?.SubjectName ?? string.Empty,
                        CourseId = relationship.CourseId,
                        CourseName = course?.CourseName ?? string.Empty,
                        CreatedAt = relationship.CreatedAt
                    });
                }

                response.Success = true;
                response.Data = relationshipDtos;
                response.Message = $"Retrieved {relationshipDtos.Count} course-subject-specialty relationships successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve course-subject-specialty relationships: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<CourseSubjectSpecialtyDetailDto>> GetByIdAsync(string specialtyId, string subjectId, string courseId)
        {
            var response = new ServiceResponse<CourseSubjectSpecialtyDetailDto>();
            try
            {
                var relationship = await _unitOfWork.CourseSubjectSpecialtyRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(r => 
                        r.SpecialtyId == specialtyId && 
                        r.SubjectId == subjectId && 
                        r.CourseId == courseId);

                if (relationship == null)
                {
                    response.Success = false;
                    response.Message = "Course-subject-specialty relationship not found";
                    return response;
                }

                // Load Specialty
                var specialty = await _unitOfWork.SpecialtyRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(s => s.SpecialtyId == relationship.SpecialtyId);

                // Load Subject
                var subject = await _unitOfWork.SubjectRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(s => s.SubjectId == relationship.SubjectId);

                // Load Course
                var course = await _unitOfWork.CourseRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(c => c.CourseId == relationship.CourseId);

                var relationshipDto = new CourseSubjectSpecialtyDetailDto
                {
                    SpecialtyId = relationship.SpecialtyId,
                    SpecialtyName = specialty?.SpecialtyName ?? string.Empty,
                    SubjectId = relationship.SubjectId,
                    SubjectName = subject?.SubjectName ?? string.Empty,
                    CourseId = relationship.CourseId,
                    CourseName = course?.CourseName ?? string.Empty,
                    CreatedAt = relationship.CreatedAt
                };

                response.Success = true;
                response.Data = relationshipDto;
                response.Message = "Course-subject-specialty relationship retrieved successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve course-subject-specialty relationship: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<List<CourseSubjectSpecialtyListDto>>> GetByCourseIdAsync(string courseId)
        {
            var response = new ServiceResponse<List<CourseSubjectSpecialtyListDto>>();
            try
            {
                var relationships = await _unitOfWork.CourseSubjectSpecialtyRepository
                    .GetByNullableExpressionWithOrderingAsync(
                        r => r.CourseId == courseId,
                        r => r.OrderBy(x => x.CreatedAt)
                    );

                var relationshipDtos = new List<CourseSubjectSpecialtyListDto>();

                foreach (var relationship in relationships)
                {
                    // Load Specialty
                    var specialty = await _unitOfWork.SpecialtyRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(s => s.SpecialtyId == relationship.SpecialtyId);

                    // Load Subject
                    var subject = await _unitOfWork.SubjectRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(s => s.SubjectId == relationship.SubjectId);

                    // Load Course
                    var course = await _unitOfWork.CourseRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(c => c.CourseId == relationship.CourseId);

                    relationshipDtos.Add(new CourseSubjectSpecialtyListDto
                    {
                        SpecialtyId = relationship.SpecialtyId,
                        SpecialtyName = specialty?.SpecialtyName ?? string.Empty,
                        SubjectId = relationship.SubjectId,
                        SubjectName = subject?.SubjectName ?? string.Empty,
                        CourseId = relationship.CourseId,
                        CourseName = course?.CourseName ?? string.Empty,
                        CreatedAt = relationship.CreatedAt
                    });
                }

                response.Success = true;
                response.Data = relationshipDtos;
                response.Message = $"Retrieved {relationshipDtos.Count} relationships for course successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve relationships by course: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<List<CourseSubjectSpecialtyListDto>>> GetBySpecialtyIdAsync(string specialtyId)
        {
            var response = new ServiceResponse<List<CourseSubjectSpecialtyListDto>>();
            try
            {
                var relationships = await _unitOfWork.CourseSubjectSpecialtyRepository
                    .GetByNullableExpressionWithOrderingAsync(
                        r => r.SpecialtyId == specialtyId,
                        r => r.OrderBy(x => x.CreatedAt)
                    );

                var relationshipDtos = new List<CourseSubjectSpecialtyListDto>();

                foreach (var relationship in relationships)
                {
                    // Load Specialty
                    var specialty = await _unitOfWork.SpecialtyRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(s => s.SpecialtyId == relationship.SpecialtyId);

                    // Load Subject
                    var subject = await _unitOfWork.SubjectRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(s => s.SubjectId == relationship.SubjectId);

                    // Load Course
                    var course = await _unitOfWork.CourseRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(c => c.CourseId == relationship.CourseId);

                    relationshipDtos.Add(new CourseSubjectSpecialtyListDto
                    {
                        SpecialtyId = relationship.SpecialtyId,
                        SpecialtyName = specialty?.SpecialtyName ?? string.Empty,
                        SubjectId = relationship.SubjectId,
                        SubjectName = subject?.SubjectName ?? string.Empty,
                        CourseId = relationship.CourseId,
                        CourseName = course?.CourseName ?? string.Empty,
                        CreatedAt = relationship.CreatedAt
                    });
                }

                response.Success = true;
                response.Data = relationshipDtos;
                response.Message = $"Retrieved {relationshipDtos.Count} relationships for specialty successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve relationships by specialty: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<List<CourseSubjectSpecialtyListDto>>> GetBySubjectIdAsync(string subjectId)
        {
            var response = new ServiceResponse<List<CourseSubjectSpecialtyListDto>>();
            try
            {
                var relationships = await _unitOfWork.CourseSubjectSpecialtyRepository
                    .GetByNullableExpressionWithOrderingAsync(
                        r => r.SubjectId == subjectId,
                        r => r.OrderBy(x => x.CreatedAt)
                    );

                var relationshipDtos = new List<CourseSubjectSpecialtyListDto>();

                foreach (var relationship in relationships)
                {
                    // Load Specialty
                    var specialty = await _unitOfWork.SpecialtyRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(s => s.SpecialtyId == relationship.SpecialtyId);

                    // Load Subject
                    var subject = await _unitOfWork.SubjectRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(s => s.SubjectId == relationship.SubjectId);

                    // Load Course
                    var course = await _unitOfWork.CourseRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(c => c.CourseId == relationship.CourseId);

                    relationshipDtos.Add(new CourseSubjectSpecialtyListDto
                    {
                        SpecialtyId = relationship.SpecialtyId,
                        SpecialtyName = specialty?.SpecialtyName ?? string.Empty,
                        SubjectId = relationship.SubjectId,
                        SubjectName = subject?.SubjectName ?? string.Empty,
                        CourseId = relationship.CourseId,
                        CourseName = course?.CourseName ?? string.Empty,
                        CreatedAt = relationship.CreatedAt
                    });
                }

                response.Success = true;
                response.Data = relationshipDtos;
                response.Message = $"Retrieved {relationshipDtos.Count} relationships for subject successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve relationships by subject: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<CourseSubjectSpecialtyDetailDto>> CreateAsync(CreateCourseSubjectSpecialtyDto dto)
        {
            var response = new ServiceResponse<CourseSubjectSpecialtyDetailDto>();
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

                // Check if relationship already exists
                var existingRelationship = await _unitOfWork.CourseSubjectSpecialtyRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(r => 
                        r.SpecialtyId == dto.SpecialtyId && 
                        r.SubjectId == dto.SubjectId && 
                        r.CourseId == dto.CourseId);

                if (existingRelationship != null)
                {
                    response.Success = false;
                    response.Message = "Course-subject-specialty relationship already exists";
                    return response;
                }

                // Verify Specialty exists
                var specialty = await _unitOfWork.SpecialtyRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(s => s.SpecialtyId == dto.SpecialtyId);

                if (specialty == null)
                {
                    response.Success = false;
                    response.Message = $"Specialty with ID '{dto.SpecialtyId}' not found";
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

                // Verify Course exists
                var course = await _unitOfWork.CourseRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(c => c.CourseId == dto.CourseId);

                if (course == null)
                {
                    response.Success = false;
                    response.Message = $"Course with ID '{dto.CourseId}' not found";
                    return response;
                }

                // Create new relationship
                var relationship = new CourseSubjectSpecialty
                {
                    SpecialtyId = dto.SpecialtyId,
                    SubjectId = dto.SubjectId,
                    CourseId = dto.CourseId,
                    CreatedAt = DateTime.UtcNow.AddHours(7)
                };

                await _unitOfWork.CourseSubjectSpecialtyRepository.AddAsync(relationship);
                await _unitOfWork.SaveChangesAsync();

                var relationshipDto = new CourseSubjectSpecialtyDetailDto
                {
                    SpecialtyId = relationship.SpecialtyId,
                    SpecialtyName = specialty.SpecialtyName,
                    SubjectId = relationship.SubjectId,
                    SubjectName = subject.SubjectName,
                    CourseId = relationship.CourseId,
                    CourseName = course.CourseName,
                    CreatedAt = relationship.CreatedAt
                };

                response.Success = true;
                response.Data = relationshipDto;
                response.Message = "Course-subject-specialty relationship created successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create course-subject-specialty relationship: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteBySpecialtyIdAsync(string specialtyId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var relationships = await _unitOfWork.CourseSubjectSpecialtyRepository
                    .GetByNullableExpressionWithOrderingAsync(r => r.SpecialtyId == specialtyId, null);

                if (!relationships.Any())
                {
                    response.Success = false;
                    response.Message = $"No relationships found for specialty '{specialtyId}'";
                    return response;
                }

                var deletedCount = await _unitOfWork.CourseSubjectSpecialtyRepository
                    .DeleteByNullableExpressionAsync(r => r.SpecialtyId == specialtyId);
                
                await _unitOfWork.SaveChangesAsync();

                response.Success = true;
                response.Data = true;
                response.Message = $"Deleted {deletedCount} relationship(s) for specialty '{specialtyId}'";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to delete relationships by specialty: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteByCourseIdAsync(string courseId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var relationships = await _unitOfWork.CourseSubjectSpecialtyRepository
                    .GetByNullableExpressionWithOrderingAsync(r => r.CourseId == courseId, null);

                if (!relationships.Any())
                {
                    response.Success = false;
                    response.Message = $"No relationships found for course '{courseId}'";
                    return response;
                }

                var deletedCount = await _unitOfWork.CourseSubjectSpecialtyRepository
                    .DeleteByNullableExpressionAsync(r => r.CourseId == courseId);
                
                await _unitOfWork.SaveChangesAsync();

                response.Success = true;
                response.Data = true;
                response.Message = $"Deleted {deletedCount} relationship(s) for course '{courseId}'";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to delete relationships by course: {ex.Message}";
            }

            return response;
        }
    }
}

