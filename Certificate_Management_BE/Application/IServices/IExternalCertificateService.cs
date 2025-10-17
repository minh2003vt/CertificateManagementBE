using Application.Dto.ExternalCertificateDto;
using Application.ServiceResponse;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IExternalCertificateService
    {
        Task<ServiceResponse<List<ExternalCertificateListDto>>> GetAllAsync();
        Task<ServiceResponse<List<ExternalCertificateListDto>>> GetAllByUserIdAsync(string userId);
        Task<ServiceResponse<ExternalCertificateDetailDto>> GetByIdAsync(int id);
        Task<ServiceResponse<ExternalCertificateDetailDto>> CreateAsync(string userId, CreateExternalCertificateDto dto);
        Task<ServiceResponse<ExternalCertificateDetailDto>> UpdateAsync(int id, UpdateExternalCertificateDto dto);
        Task<ServiceResponse<ExternalCertificateDetailDto>> UpdateCertificateFileAsync(int id, IFormFile file);
        Task<ServiceResponse<string>> DeleteAsync(int id);
        Task<ServiceResponse<string>> DeleteCertificateImageAsync(int id);
    }
}

