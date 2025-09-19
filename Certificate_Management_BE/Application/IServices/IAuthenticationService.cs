using Application.ServiceResponse;
using Application.ViewModels.UserDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IAuthenticationService
    {
        Task<LoginResponse> LoginAsync(LoginModel loginModel);
    }
}
