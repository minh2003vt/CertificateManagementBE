using Application.Dto.SpecialtyDto;
using Application.ServiceResponse;

namespace Application.IServices
{
    public interface ISpecialtyService
    {
        Task<ServiceResponse<List<SpecialtyListDto>>> GetAllAsync();
        Task<ServiceResponse<SpecialtyDetailDto>> GetByIdAsync(string specialtyId);
        Task<ServiceResponse<SpecialtyDetailDto>> CreateAsync(CreateSpecialtyDto dto, string createdByUserId);
        Task<ServiceResponse<SpecialtyDetailDto>> UpdateAsync(string specialtyId, UpdateSpecialtyDto dto, string updatedByUserId);
        Task<ServiceResponse<bool>> DeleteAsync(string specialtyId);
    }
}

