using Application.Dto.CertificateEligibilityDto;
using Application.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface ICertificateEligibilityService
    {
        Task<ServiceResponse<List<CertificateEligibilityDto>>> CheckTraineeCertificateEligibilityAsync(string traineeId);
        Task<ServiceResponse<bool>> ProcessTraineeSubjectCompletionAsync(string traineeId, string subjectId);
        Task<ServiceResponse<List<CertificateEligibilityDto>>> GetEligibleCertificatesForTraineeAsync(string traineeId);
    }
}
