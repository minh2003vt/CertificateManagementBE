using Application.Dto.PlanDto;
using Application.ServiceResponse;

namespace Application.IServices
{
    public interface IPlanService
    {
        Task<ServiceResponse<List<PlanListDto>>> GetAllAsync();
        Task<ServiceResponse<PlanDetailDto>> GetByIdAsync(string planId);
        Task<ServiceResponse<PlanWithCoursesDto>> GetByIdWithCoursesAsync(string planId);
        Task<ServiceResponse<PlanDetailDto>> CreateAsync(CreatePlanDto dto, string createdByUserId);
        Task<ServiceResponse<PlanDetailDto>> UpdateAsync(string planId, UpdatePlanDto dto);
        Task<ServiceResponse<bool>> DeleteAsync(string planId);
        Task<ServiceResponse<PlanDetailDto>> ApproveAsync(string planId, string aprovedByUserId);
        Task<ServiceResponse<PlanDetailDto>> RejectAsync(string planId, string aprovedByUserId);
        Task<ServiceResponse<List<PlanListDto>>> GetAllByStatusAsync(string status);
    }
}
