using Application.Dto.CertificateEligibilityDto;
using Application.IServices;
using Certificate_Management_BE.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Certificate_Management_BE.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CertificateEligibilityController : ControllerBase
    {
        private readonly ICertificateEligibilityService _certificateEligibilityService;

        public CertificateEligibilityController(ICertificateEligibilityService certificateEligibilityService)
        {
            _certificateEligibilityService = certificateEligibilityService;
        }

        [HttpGet("trainee/{traineeId}/eligible-certificates")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor", "Trainee")]
        public async Task<IActionResult> GetEligibleCertificates(string traineeId)
        {
            var result = await _certificateEligibilityService.GetEligibleCertificatesForTraineeAsync(traineeId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("trainee/{traineeId}/subject/{subjectId}/completion")]
        [AuthorizeRoles("Education Officer", "Instructor")]
        public async Task<IActionResult> ProcessSubjectCompletion(string traineeId, string subjectId)
        {
            var result = await _certificateEligibilityService.ProcessTraineeSubjectCompletionAsync(traineeId, subjectId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}
