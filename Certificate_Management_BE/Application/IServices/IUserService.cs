using Application.ServiceResponse;
using Application.Dto.UserDto;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IUserService
    {
        Task<ServiceResponse<UserProfileDto>> GetUserProfileAsync(string userId);

        Task<ServiceResponse<UserProfileDto>> UpdateUserProfileAsync(string userId, UserUpdateProfileDto profileDto);
        Task<ServiceResponse<string>> ChangePasswordAsync(string userId, ChangePasswordDto dto);
        Task<ServiceResponse<ImportResultDto>> ImportTraineesAsync(IFormFile file, string performedByUsername);
        Task<ServiceResponse<string>> UpdateUserStatusAsync(UserStatusDto dto);
        Task<ServiceResponse<string>> SendCredentialsEmailAsync(string userId);
    }
}

