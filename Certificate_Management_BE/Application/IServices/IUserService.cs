using Application.ServiceResponse;
using Application.Dto.UserDto;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IUserService
    {
        Task<ServiceResponse<UserProfileDto>> GetUserProfileAsync(string userId);
    }
}

