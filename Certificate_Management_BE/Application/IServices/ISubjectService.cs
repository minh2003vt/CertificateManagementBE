using Application.Dto.SubjectDto;
using Application.ServiceResponse;

namespace Application.IServices
{
    public interface ISubjectService
    {
        Task<ServiceResponse<List<SubjectListDto>>> GetAllAsync();
        Task<ServiceResponse<SubjectDetailDto>> GetByIdAsync(string subjectId);
        Task<ServiceResponse<SubjectDetailDto>> CreateAsync(CreateSubjectDto dto, string createdByUserId);
        Task<ServiceResponse<SubjectDetailDto>> UpdateAsync(string subjectId, UpdateSubjectDto dto);
        Task<ServiceResponse<string>> DeleteAsync(string subjectId);
        Task<ServiceResponse<SubjectImportResultDto>> ImportSubjectsAsync(List<SubjectImportDto> subjects, string createdByUserId);
        Task<ServiceResponse<SubjectDetailDto>> ApproveAsync(string subjectId, string aprovedByUserId);
        Task<ServiceResponse<SubjectDetailDto>> RejectAsync(string subjectId, string aprovedByUserId);
        Task<ServiceResponse<List<SubjectListDto>>> GetAllByStatusAsync(string status);
    }
}

