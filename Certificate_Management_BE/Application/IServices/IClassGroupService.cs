using Application.Dto.ClassGroupDto;
using Application.Dto.ClassDto;
using Application.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IClassGroupService
    {
        Task<ServiceResponse<List<ClassGroupListDto>>> GetAllAsync();
        Task<ServiceResponse<ClassGroupDetailDto>> GetByIdAsync(int classGroupId);
        Task<ServiceResponse<List<ClassListDto>>> GetClassesByGroupIdAsync(int classGroupId);
        Task<ServiceResponse<ClassGroupDetailDto>> CreateAsync(CreateClassGroupDto dto);
        Task<ServiceResponse<ClassGroupDetailDto>> UpdateAsync(int classGroupId, UpdateClassGroupDto dto);
        Task<ServiceResponse<bool>> DeleteAsync(int classGroupId);
    }
}


