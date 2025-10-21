using Application.Dto.TraineePlanEnrollmentDto;
using Application.IServices;
using Certificate_Management_BE.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Certificate_Management_BE.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TraineePlanEnrollmentController : ControllerBase
    {
        private readonly ITraineePlanEnrollmentService _traineePlanEnrollmentService;

        public TraineePlanEnrollmentController(ITraineePlanEnrollmentService traineePlanEnrollmentService)
        {
            _traineePlanEnrollmentService = traineePlanEnrollmentService;
        }

        [HttpPost("enroll")]
        [AuthorizeRoles("Education Officer")]
        public async Task<IActionResult> EnrollTrainee([FromBody] CreateTraineePlanEnrollmentDto dto)
        {
            var enrolledByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(enrolledByUserId))
            {
                return Unauthorized(new { Success = false, Message = "User ID not found in token" });
            }

            var result = await _traineePlanEnrollmentService.EnrollTraineeAsync(dto, enrolledByUserId);
            if (!result.Success) return BadRequest(result);
            return CreatedAtAction(nameof(GetTraineeEnrollments), new { traineeId = dto.TraineeId }, result);
        }

        [HttpGet("trainee/{traineeId}")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor", "Trainee")]
        public async Task<IActionResult> GetTraineeEnrollments(string traineeId)
        {
            var result = await _traineePlanEnrollmentService.GetTraineeEnrollmentsAsync(traineeId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("plan/{planId}")]
        [AuthorizeRoles("Administrator", "Education Officer")]
        public async Task<IActionResult> GetPlanEnrollments(string planId)
        {
            var result = await _traineePlanEnrollmentService.GetPlanEnrollmentsAsync(planId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{enrollmentId}/complete")]
        [AuthorizeRoles("Education Officer")]
        public async Task<IActionResult> CompleteEnrollment(string enrollmentId)
        {
            var completedByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(completedByUserId))
            {
                return Unauthorized(new { Success = false, Message = "User ID not found in token" });
            }

            var result = await _traineePlanEnrollmentService.CompleteEnrollmentAsync(enrollmentId, completedByUserId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{enrollmentId}/deactivate")]
        [AuthorizeRoles("Education Officer")]
        public async Task<IActionResult> DeactivateEnrollment(string enrollmentId)
        {
            var deactivatedByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(deactivatedByUserId))
            {
                return Unauthorized(new { Success = false, Message = "User ID not found in token" });
            }

            var result = await _traineePlanEnrollmentService.DeactivateEnrollmentAsync(enrollmentId, deactivatedByUserId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}
