using Application.Dto.ClassDto;
using Application.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IClassService
    {
        Task<ServiceResponse<List<ClassListDto>>> GetAllAsync();
        Task<ServiceResponse<ClassDetailDto>> GetByIdAsync(int classId);
        Task<ServiceResponse<ClassDetailDto>> CreateAsync(CreateClassDto dto, string createdByUserId);
        Task<ServiceResponse<ClassDetailDto>> UpdateAsync(int classId, UpdateClassDto dto, string updatedByUserId);
        Task<ServiceResponse<bool>> DeleteAsync(int classId);
    }
}


