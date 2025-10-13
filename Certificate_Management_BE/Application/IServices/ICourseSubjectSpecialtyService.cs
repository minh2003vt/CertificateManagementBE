using Application.Dto.CourseSubjectSpecialtyDto;
using Application.ServiceResponse;

namespace Application.IServices
{
    public interface ICourseSubjectSpecialtyService
    {
        Task<ServiceResponse<List<CourseSubjectSpecialtyListDto>>> GetAllAsync();
        Task<ServiceResponse<CourseSubjectSpecialtyDetailDto>> GetByIdAsync(string specialtyId, string subjectId, string courseId);
        Task<ServiceResponse<List<CourseSubjectSpecialtyListDto>>> GetByCourseIdAsync(string courseId);
        Task<ServiceResponse<List<CourseSubjectSpecialtyListDto>>> GetBySpecialtyIdAsync(string specialtyId);
        Task<ServiceResponse<List<CourseSubjectSpecialtyListDto>>> GetBySubjectIdAsync(string subjectId);
        Task<ServiceResponse<CourseSubjectSpecialtyDetailDto>> CreateAsync(CreateCourseSubjectSpecialtyDto dto);
        Task<ServiceResponse<bool>> DeleteBySpecialtyIdAsync(string specialtyId);
        Task<ServiceResponse<bool>> DeleteByCourseIdAsync(string courseId);
    }
}

