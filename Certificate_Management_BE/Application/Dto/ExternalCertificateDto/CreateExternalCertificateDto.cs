using Application.Dto.UserDto;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Dto.ExternalCertificateDto
{
    public class CreateExternalCertificateDto
    {
        [Required(ErrorMessage = "Certificate code is required")]
        public string CertificateCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Certificate name is required")]
        public string CertificateName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Issuing organization is required")]
        public string IssuingOrganization { get; set; } = string.Empty;

        [Required(ErrorMessage = "Issue date is required")]
        public DateTime IssueDate { get; set; }

        [Required(ErrorMessage = "Expiry date is required")]
        public DateTime Exp_date { get; set; }

        [Required(ErrorMessage = "Certificate file is required")]
        public IFormFile CertificateFile { get; set; } = null!;

        public ValidationErrorsDto Validate()
        {
            var errors = new ValidationErrorsDto();

            if (string.IsNullOrEmpty(CertificateCode))
                errors.Add("Certificate code is required");

            if (string.IsNullOrEmpty(CertificateName))
                errors.Add("Certificate name is required");

            if (string.IsNullOrEmpty(IssuingOrganization))
                errors.Add("Issuing organization is required");

            if (Exp_date <= IssueDate)
                errors.Add("Expiry date must be after issue date");

            // Validate file
            if (CertificateFile == null || CertificateFile.Length == 0)
            {
                errors.Add("Certificate file is required");
            }
            else
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = System.IO.Path.GetExtension(CertificateFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    errors.Add("Only JPG, JPEG, and PNG files are allowed");
                }

                // Max 10MB
                if (CertificateFile.Length > 10 * 1024 * 1024)
                {
                    errors.Add("File size must not exceed 10MB");
                }
            }

            return errors;
        }
    }
}

