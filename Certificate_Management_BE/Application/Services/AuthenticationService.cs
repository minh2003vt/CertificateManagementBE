using Application.IRepositories;
using Application.IServices;
using Application.ServiceResponse;
using Application.ViewModels.UserDtos;
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

namespace Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtTokenHelper _jwtTokenHelper;

        public AuthenticationService(IUserRepository userRepository, IUnitOfWork unitOfWork, JwtTokenHelper jwtTokenHelper)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _jwtTokenHelper = jwtTokenHelper;
        }

        #region Login
        public async Task<LoginResponse> LoginAsync(LoginModel loginModel)
        {
            var response = new LoginResponse();
            try
            {
                var validationContext = new ValidationContext(loginModel);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(loginModel, validationContext, validationResults, true))
                {
                    var errorMessages = validationResults.Select(r => !string.IsNullOrWhiteSpace(r.ErrorMessage) ? (string)r.ErrorMessage : string.Empty).ToList();
                    response.Success = false;
                    response.Message = string.Join("; ", errorMessages);
                    return response;
                }

                var user = await _userRepository.GetSingleOrDefaultByNullableExpressionAsNoTrackingAsync(u => u.Username.Equals(loginModel.Username, StringComparison.OrdinalIgnoreCase));

                if (user == null || !PasswordHashHelper.VerifyPassword(loginModel.Password, user.PasswordHash))
                {
                    response.Success = false;
                    response.Message = "Invalid username or password.";
                    return response;
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
    }
}
