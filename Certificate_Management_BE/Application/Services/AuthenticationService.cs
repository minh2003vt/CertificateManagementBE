using Application.IRepositories;
using Application.IServices;
using Application.ServiceResponse;
using Application.Dto.UserDto;
using Application.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtTokenHelper _jwtTokenHelper;
        private readonly IEmailService _emailService;

        public AuthenticationService(IUnitOfWork unitOfWork, JwtTokenHelper jwtTokenHelper, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _jwtTokenHelper = jwtTokenHelper;
            _emailService = emailService;
        }

        #region Login
        public async Task<LoginResponse> LoginAsync(LoginDto loginDto)
        {
            var response = new LoginResponse();
            try
            {
                var validationContext = new ValidationContext(loginDto);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(loginDto, validationContext, validationResults, true))
                {
                    var errorMessages = validationResults.Select(r => !string.IsNullOrWhiteSpace(r.ErrorMessage) ? (string)r.ErrorMessage : string.Empty).ToList();
                    response.Success = false;
                    response.Message = string.Join("; ", errorMessages);
                    return response;
                }
                var user = await _unitOfWork.UserRepository.GetByUsernameAsync(loginDto.Username);
                if (user == null || !PasswordHashHelper.VerifyPassword(loginDto.Password, user.PasswordHash))
                {
                    response.Success = false;
                    response.Message = "Invalid username or password.";
                    return response;
                }

                if (user.Role == null)
                {
                    var roleEntity = await _unitOfWork.RoleRepository.GetSingleOrDefaultByNullableExpressionAsync(r => r.RoleId == user.RoleId);
                    user.Role = roleEntity;
                }

                var roles = new List<string> { user.Role?.RoleName ?? "User" };
                var token = _jwtTokenHelper.GenerateToken(user, roles);

                var jti = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims
                    .First(c => c.Type == "jti").Value;

                var session = new Session
                {
                    SessionId = jti,
                    UserId = user.UserId,
                    LoginTime = DateTime.UtcNow.AddHours(7),
                    SessionExpiry = DateTime.UtcNow.AddHours(7).AddMinutes(50)
                };
                await _unitOfWork.SessionRepository.AddAsync(session);
                response.UserId = user.UserId;
                response.Roles = roles;
                response.Token = token;
                response.Success = true;
                return response;
            }
            catch (DbException ex)
            {
                response.Success = false;
                response.Message = "Database error occurred.";
                response.Message = ex.Message;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Login failed.";
                response.Message = ex.Message;
            }

            return response;
        }
        #endregion

        #region ForgotPassword
        public async Task<ServiceResponse<string>> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var response = new ServiceResponse<string>();
            try
            {
                // Find user by email or username - query directly instead of GetAll()
                var user = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u =>
                        u.Email == dto.EmailOrUsername || u.Username == dto.EmailOrUsername);

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found with the provided email or username.";
                    return response;
                }

                // Generate JWT reset token (expires in 5 minutes)
                var resetToken = _jwtTokenHelper.GeneratePasswordResetToken(user.UserId, user.Email);

                // Send email
                await _emailService.SendPasswordResetEmailAsync(user.Email, user.FullName, resetToken);

                response.Success = true;
                response.Message = "Password reset link has been sent to your email.";
                response.Data = "Email sent successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to process password reset request.";
                response.Message = ex.Message;
            }

            return response;
        }
        #endregion

        #region ResetPassword
        public async Task<ServiceResponse<string>> ResetPasswordAsync(string token, ResetPasswordDto dto)
        {
            var response = new ServiceResponse<string>();
            try
            {
                var validationContext = new ValidationContext(dto);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(dto, validationContext, validationResults, true))
                {
                    var errorMessages = validationResults.Select(r => !string.IsNullOrWhiteSpace(r.ErrorMessage) ? (string)r.ErrorMessage : string.Empty).ToList();
                    response.Success = false;
                    response.Message = string.Join("; ", errorMessages);
                    return response;
                }

                // Validate JWT token
                var principal = _jwtTokenHelper.ValidatePasswordResetToken(token);
                if (principal == null)
                {
                    response.Success = false;
                    response.Message = "Invalid or expired reset token.";
                    return response;
                }

                // Extract user ID from token
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    response.Success = false;
                    response.Message = "Invalid token claims.";
                    return response;
                }

                // Get user
                var user = await _unitOfWork.UserRepository.GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == userId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }

                // Hash new password
                user.PasswordHash = PasswordHashHelper.HashPassword(dto.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.UserRepository.UpdateAsync(user);

                response.Success = true;
                response.Message = "Password has been reset successfully.";
                response.Data = "Password updated";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to reset password.";
                response.Message = ex.Message;
            }

            return response;
        }
        #endregion
    }
}
