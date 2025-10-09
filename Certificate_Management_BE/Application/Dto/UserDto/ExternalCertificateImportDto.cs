using System;

namespace Application.Dto.UserDto
{
    public class ExternalCertificateImportDto
    {
        public int RowNumber { get; set; }
        public string CitizenId { get; set; } = string.Empty;
        public string CertificateCode { get; set; } = string.Empty;
        public string CertificateName { get; set; } = string.Empty;
        public string IssuingOrganization { get; set; } = string.Empty;
        public string IssueDateStr { get; set; } = string.Empty;
        public string ExpiredDateStr { get; set; } = string.Empty;
        public string CertificateImageBase64 { get; set; } = string.Empty;

        // Parsed values
        public DateTime IssueDate { get; private set; }
        public DateTime ExpiredDate { get; private set; }

        public ValidationErrorsDto Validate()
        {
            var errors = new ValidationErrorsDto();

            // Certificate Code
            if (string.IsNullOrEmpty(CertificateCode))
                errors.Add("Certificate code is required");

            // Certificate Name
            if (string.IsNullOrEmpty(CertificateName))
                errors.Add("Certificate name is required");

            // Issuing Organization
            if (string.IsNullOrEmpty(IssuingOrganization))
                errors.Add("Issuing organization is required");

            // Issue Date
            if (DateTime.TryParse(IssueDateStr, out var parsedIssueDate))
            {
                IssueDate = DateTime.SpecifyKind(parsedIssueDate, DateTimeKind.Utc);
            }
            else
            {
                errors.Add("Invalid issue date format");
                IssueDate = DateTime.UtcNow; // Default
            }

            // Expired Date
            if (DateTime.TryParse(ExpiredDateStr, out var parsedExpDate))
            {
                ExpiredDate = DateTime.SpecifyKind(parsedExpDate, DateTimeKind.Utc);
            }
            else
            {
                errors.Add("Invalid expiry date format");
                ExpiredDate = DateTime.UtcNow.AddYears(2); // Default
            }

            return errors;
        }
    }
}

