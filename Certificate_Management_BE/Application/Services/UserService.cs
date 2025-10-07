using Application.IRepositories;
using Application.IServices;
using Application.ServiceResponse;
using Application.Dto.UserDto;
using System;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region GetProfile
        public async Task<ServiceResponse<UserProfileDto>> GetUserProfileAsync(string userId)
        {
            var response = new ServiceResponse<UserProfileDto>();
            try
            {
                var user = await _unitOfWork.UserRepository.GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == userId);

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }

                var profile = new UserProfileDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Sex = user.Sex,
                    DateOfBirth = user.DateOfBirth,
                    CitizenId = user.CitizenId,
                };

                response.Success = true;
                response.Data = profile;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to retrieve user profile.";
                response.Message = ex.Message;
            }

            return response;
        }
        #endregion
    }
}

