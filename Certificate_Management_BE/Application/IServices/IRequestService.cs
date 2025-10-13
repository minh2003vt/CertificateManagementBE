using Application.Dto.RequestDto;
using Application.ServiceResponse;
using Domain.Enums;

namespace Application.IServices
{
    public interface IRequestService
    {
        Task<ServiceResponse<RequestDetailDto>> CreateRequestAsync(CreateRequestDto dto, string requestUserId, string entityId, RequestType requestType);
        Task<ServiceResponse<List<RequestListDto>>> GetAllRequestsAsync();
        Task<ServiceResponse<RequestDetailDto>> GetRequestByIdAsync(string requestId);
        Task<ServiceResponse<List<RequestListDto>>> GetRequestsByUserAsync(string userId);
        Task<ServiceResponse<bool>> ApproveRequestAsync(string requestId, string approvedByUserId);
        Task<ServiceResponse<bool>> RejectRequestAsync(string requestId, string approvedByUserId);
    }
}
