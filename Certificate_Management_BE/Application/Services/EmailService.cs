using Application.IServices;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
       private readonly string frontendUrl;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            frontendUrl = "http://localhost:5173";
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetToken)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderPassword = _configuration["EmailSettings:SenderPassword"];
            var senderName = _configuration["EmailSettings:SenderName"];

            var resetLink = $"{frontendUrl}/reset-password?token={resetToken}";

            using var smtpClient = new SmtpClient(smtpServer, smtpPort)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderEmail, senderPassword)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail!, senderName),
                Subject = "Password Reset Request",
                Body = $@"
                    <html>
                    <body>
                        <h2>Password Reset Request</h2>
                        <p>Hello {userName},</p>
                        <p>We received a request to reset your password. Click the link below to reset your password:</p>
                        <p><a href='{resetLink}'>Reset Password</a></p>
                        <p>This link will expire in 1 hour.</p>
                        <p>If you did not request a password reset, please ignore this email.</p>
                        <br/>
                        <p>Best regards,<br/>Certificate Management Team</p>
                    </body>
                    </html>",
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendCredentialsEmailAsync(string toEmail, string userName, string username, string password)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderPassword = _configuration["EmailSettings:SenderPassword"];
            var senderName = _configuration["EmailSettings:SenderName"];

            using var smtpClient = new SmtpClient(smtpServer, smtpPort)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderEmail, senderPassword)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail!, senderName),
                Subject = "Your Account Credentials - Certificate Management System",
                Body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px;'>
                            <h2 style='color: #2c3e50; border-bottom: 2px solid #3498db; padding-bottom: 10px;'>Account Credentials</h2>
                            <p>Hello <strong>{userName}</strong>,</p>
                            <p>Your account has been created in the Certificate Management System. Below are your login credentials:</p>
                            
                            <div style='background-color: #f8f9fa; padding: 15px; border-left: 4px solid #3498db; margin: 20px 0;'>
                                <p style='margin: 5px 0;'><strong>Username:</strong> {username}</p>
                                <p style='margin: 5px 0;'><strong>Password:</strong> {password}</p>
                            </div>
                            
                            <p style='color: #e74c3c; font-weight: bold;'>⚠️ Important Security Notice:</p>
                            <ul style='color: #7f8c8d;'>
                                <li>Please change your password immediately after your first login</li>
                                <li>Do not share your credentials with anyone</li>
                                <li>Keep this email in a secure location</li>
                            </ul>
                            
                            <p style='margin-top: 30px;'>If you did not request this account or have any questions, please contact the administrator immediately.</p>
                            
                            <br/>
                            <p style='color: #7f8c8d; font-size: 14px;'>Best regards,<br/><strong>Certificate Management Team</strong></p>
                        </div>
                    </body>
                    </html>",
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"], int.Parse(_configuration["EmailSettings:SmtpPort"]))
            {
                Credentials = new NetworkCredential(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:FromEmail"], _configuration["EmailSettings:FromName"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}

