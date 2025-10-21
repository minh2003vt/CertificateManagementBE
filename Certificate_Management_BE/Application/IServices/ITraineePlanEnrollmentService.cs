using Application.Dto.TraineePlanEnrollmentDto;
using Application.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface ITraineePlanEnrollmentService
    {
        Task<ServiceResponse<TraineePlanEnrollmentDetailDto>> EnrollTraineeAsync(CreateTraineePlanEnrollmentDto dto, string enrolledByUserId);
        Task<ServiceResponse<List<TraineePlanEnrollmentDetailDto>>> GetTraineeEnrollmentsAsync(string traineeId);
        Task<ServiceResponse<List<TraineePlanEnrollmentDetailDto>>> GetPlanEnrollmentsAsync(string planId);
        Task<ServiceResponse<bool>> CompleteEnrollmentAsync(string enrollmentId, string completedByUserId);
        Task<ServiceResponse<bool>> DeactivateEnrollmentAsync(string enrollmentId, string deactivatedByUserId);
    }
}
