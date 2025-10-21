using Application.Dto.CertificateEligibilityDto;
using Application.IServices;
using Certificate_Management_BE.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Certificate_Management_BE.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TraineeController : ControllerBase
    {
        private readonly ICertificateEligibilityService _certificateEligibilityService;

        public TraineeController(ICertificateEligibilityService certificateEligibilityService)
        {
            _certificateEligibilityService = certificateEligibilityService;
        }

        [HttpGet("my-certificate-eligibility")]
        [AuthorizeRoles("Trainee")]
        public async Task<IActionResult> GetMyCertificateEligibility()
        {
            var traineeId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(traineeId))
            {
                return Unauthorized(new { Success = false, Message = "Trainee ID not found in token" });
            }

            var result = await _certificateEligibilityService.GetEligibleCertificatesForTraineeAsync(traineeId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{traineeId}/certificate-eligibility")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor")]
        public async Task<IActionResult> GetTraineeCertificateEligibility(string traineeId)
        {
            var result = await _certificateEligibilityService.GetEligibleCertificatesForTraineeAsync(traineeId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}
