using Application.ServiceResponse;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IGeminiService
    {
        Task<ServiceResponse<string>> CheckSensitiveAsync(string jsonPayload);
    }
}


