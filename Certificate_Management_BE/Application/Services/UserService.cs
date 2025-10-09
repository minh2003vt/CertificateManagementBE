using Application.IRepositories;
using Application.IServices;
using Application.ServiceResponse;
using Application.Dto.UserDto;
using Application.Helpers;
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

        #region UpdateProfile
        public async Task<ServiceResponse<UserProfileDto>> UpdateUserProfileAsync(string userId, UserUpdateProfileDto profileDto)
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

                // Check if CitizenId is already used by another user
                var existingUserWithCitizenId = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.CitizenId == profileDto.CitizenId && u.UserId != userId);
                
                if (existingUserWithCitizenId != null)
                {
                    response.Success = false;
                    response.Message = "Citizen ID is already in use by another user.";
                    return response;
                }

                // Check if Email is already used by another user
                var existingUserWithEmail = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.Email == profileDto.Email && u.UserId != userId);
                
                if (existingUserWithEmail != null)
                {
                    response.Success = false;
                    response.Message = "Email is already in use by another user.";
                    return response;
                }

                // Update allowed fields
                user.FullName = profileDto.FullName;
                user.Email = profileDto.Email;
                user.PhoneNumber = profileDto.PhoneNumber ?? string.Empty;
                user.Sex = profileDto.Sex;
                user.DateOfBirth = profileDto.DateOfBirth;
                user.CitizenId = profileDto.CitizenId;
                user.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.UserRepository.UpdateAsync(user);

                // Return updated profile
                var updated = new UserProfileDto
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
                response.Data = updated;
                response.Message = "Profile updated successfully.";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }
        #endregion

        #region ChangePassword
        public async Task<ServiceResponse<string>> ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            var response = new ServiceResponse<string>();
            try
            {
                var user = await _unitOfWork.UserRepository.GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == userId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }

                // Verify current password
                if (!PasswordHashHelper.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
                {
                    response.Success = false;
                    response.Message = "Current password is incorrect.";
                    return response;
                }

                // Hash and update new password
                user.PasswordHash = PasswordHashHelper.HashPassword(dto.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.UserRepository.UpdateAsync(user);

                response.Success = true;
                response.Message = "Password changed successfully.";
                response.Data = "Password updated";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to change password.";
                response.Message = ex.Message;
            }

            return response;
        }
        #endregion
    }
}

