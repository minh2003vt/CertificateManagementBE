using Application.ServiceResponse;
using Application.Dto.UserDto;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IUserService
    {
        Task<ServiceResponse<UserProfileDto>> GetUserProfileAsync(string userId);
        Task<ServiceResponse<ImportResultDto>> ImportTraineesAsync(IFormFile file);
    }
}

