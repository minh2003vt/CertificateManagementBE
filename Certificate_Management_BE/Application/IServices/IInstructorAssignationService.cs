using Application.Dto.InstructorAssignationDto;
using Application.ServiceResponse;

namespace Application.IServices
{
    public interface IInstructorAssignationService
    {
        Task<ServiceResponse<List<InstructorAssignationListDto>>> GetAllAsync();
        Task<ServiceResponse<InstructorAssignationDetailDto>> GetByIdAsync(string subjectId, string instructorId);
        Task<ServiceResponse<List<InstructorAssignationListDto>>> GetBySubjectIdAsync(string subjectId);
        Task<ServiceResponse<List<InstructorAssignationListDto>>> GetByInstructorIdAsync(string instructorId);
        Task<ServiceResponse<InstructorAssignationDetailDto>> CreateAsync(CreateInstructorAssignationDto dto, string assignedByUserId);
        Task<ServiceResponse<bool>> DeleteAsync(string subjectId, string instructorId);
    }
}

