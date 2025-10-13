using Application.ServiceResponse;
using Application.Dto.UserDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IAuthenticationService
    {
        Task<LoginResponse> LoginAsync(LoginDto loginDto);
        Task<ServiceResponse<string>> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<ServiceResponse<string>> ResetPasswordAsync(string token, ResetPasswordDto dto);
        Task<ServiceResponse<UserProfileDto>> CreateManualAccountAsync(CreateManualAccountDto dto, string createdByUserId);
    }
}
