using System.Collections.Generic;

namespace Application.Dto.UserDto
{
    public class ImportResultDto
    {
        public TraineeImportResultDto TraineeData { get; set; } = new();
        public ExternalCertificateImportResultDto ExternalCertificateData { get; set; } = new();
    }

    public class TraineeImportResultDto
    {
        public int TotalRows { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<ImportErrorDto> Errors { get; set; } = new();
    }

    public class ExternalCertificateImportResultDto
    {
        public int TotalRows { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<ImportErrorDto> Errors { get; set; } = new();
    }

    public class ImportErrorDto
    {
        public int RowNumber { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? CitizenId { get; set; }
        public string? CertificateCode { get; set; }
    }

    public class ValidationErrorsDto
    {
        public List<string> Errors { get; set; } = new();

        public bool HasErrors => Errors.Count > 0;

        public void Add(string error)
        {
            Errors.Add(error);
        }

        public string GetErrorMessage()
        {
            return string.Join("; ", Errors);
        }
    }
}

