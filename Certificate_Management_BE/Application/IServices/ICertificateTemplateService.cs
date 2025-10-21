using Application.Dto.CertificateTemplateDto;
using Application.ServiceResponse;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface ICertificateTemplateService
    {
        Task<ServiceResponse<List<CertificateTemplateListDto>>> GetAllAsync();
        Task<ServiceResponse<CertificateTemplateDetailDto>> GetByIdAsync(string templateId);
        Task<ServiceResponse<CertificateTemplateDetailDto>> CreateAsync(CreateCertificateTemplateDto dto, string createdByUserId);
        Task<ServiceResponse<CertificateTemplateDetailDto>> ImportTemplateAsync(string templateId, IFormFile templateFile);
        Task<ServiceResponse<CertificateTemplateDetailDto>> UpdateAsync(string templateId, UpdateCertificateTemplateDto dto, string updatedByUserId);
        Task<ServiceResponse<bool>> DeleteAsync(string templateId);
        Task<ServiceResponse<bool>> ApproveAsync(string templateId, string approvedByUserId);
        Task<ServiceResponse<List<CertificateTemplateListDto>>> GetByCertificateKindAsync(Domain.Enums.CertificateKind certificateKind);
    }
}
