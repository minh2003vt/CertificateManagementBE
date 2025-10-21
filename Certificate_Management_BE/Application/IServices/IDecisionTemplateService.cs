using Application.Dto.DecisionTemplateDto;
using Application.ServiceResponse;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IDecisionTemplateService
    {
        Task<ServiceResponse<List<DecisionTemplateListDto>>> GetAllAsync();
        Task<ServiceResponse<DecisionTemplateDetailDto>> GetByIdAsync(string templateId);
        Task<ServiceResponse<DecisionTemplateDetailDto>> CreateAsync(CreateDecisionTemplateDto dto, string createdByUserId);
        Task<ServiceResponse<DecisionTemplateDetailDto>> ImportTemplateAsync(string templateId, IFormFile templateFile);
        Task<ServiceResponse<DecisionTemplateDetailDto>> UpdateAsync(string templateId, UpdateDecisionTemplateDto dto, string updatedByUserId);
        Task<ServiceResponse<bool>> DeleteAsync(string templateId);
        Task<ServiceResponse<bool>> ApproveAsync(string templateId, string approvedByUserId);
    }
}
