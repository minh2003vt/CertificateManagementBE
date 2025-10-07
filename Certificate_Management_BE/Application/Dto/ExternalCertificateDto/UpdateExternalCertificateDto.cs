using Application.Dto.UserDto;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Dto.ExternalCertificateDto
{
    public class UpdateExternalCertificateDto
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

            return errors;
        }
    }
}

