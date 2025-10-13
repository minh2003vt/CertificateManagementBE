using Application.Dto.CourseDto;
using Application.Dto.SubjectDto;
using Application.ServiceResponse;

namespace Application.IServices
{
    public interface ICourseService
    {
        Task<ServiceResponse<List<CourseListDto>>> GetAllAsync();
        Task<ServiceResponse<CourseDetailDto>> GetByIdAsync(string courseId);
        Task<ServiceResponse<CourseDetailDto>> CreateAsync(CreateCourseDto dto, string createdByUserId);
        Task<ServiceResponse<CourseDetailDto>> UpdateAsync(string courseId, UpdateCourseDto dto);
        Task<ServiceResponse<bool>> DeleteAsync(string courseId);
        Task<ServiceResponse<CourseDetailDto>> ApproveAsync(string courseId, string aprovedByUserId);
        Task<ServiceResponse<CourseDetailDto>> RejectAsync(string courseId, string aprovedByUserId);
        Task<ServiceResponse<List<CourseListDto>>> GetAllByStatusAsync(string status);
        Task<ServiceResponse<List<SubjectListDto>>> GetAllSubjectsByCourseIdAsync(string courseId);
    }
}

