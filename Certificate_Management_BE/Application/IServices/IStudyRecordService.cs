using Application.Dto.StudyRecordDto;
using Application.ServiceResponse;

namespace Application.IServices
{
    public interface IStudyRecordService
    {
        Task<ServiceResponse<List<StudyRecordListDto>>> GetByPlanAndCourseAsync(string planId, string courseId);
        Task<ServiceResponse<List<StudyRecordListDto>>> CreateAsync(CreateStudyRecordDto dto, string createdByUserId);
        Task<ServiceResponse<bool>> DeleteByPlanIdAsync(string planId);
        Task<ServiceResponse<bool>> DeleteSpecificAsync(string planId, string courseId, string subjectId);
    }
}


