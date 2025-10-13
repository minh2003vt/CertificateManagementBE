using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetToken);
        Task SendCredentialsEmailAsync(string toEmail, string userName, string username, string password);
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}

