using Application.Dto.PlanDto;
using Application.Dto.RequestDto;
using Application.IServices;
using Certificate_Management_BE.Extensions;
using Certificate_Management_BE.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Certificate_Management_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlanController : ControllerBase
    {
        private readonly IPlanService _planService;
        private readonly IRequestService _requestService;

        public PlanController(IPlanService planService, IRequestService requestService)
        {
            _planService = planService;
            _requestService = requestService;
        }

        /// <summary>
        /// Get all plans
        /// </summary>
        [HttpGet]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor", "Trainee")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _planService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get plan by ID
        /// </summary>
        [HttpGet("{planId}")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor", "Trainee")]
        public async Task<IActionResult> GetById(string planId)
        {
            var result = await _planService.GetByIdAsync(planId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get plan by ID with courses and subjects
        /// </summary>
        [HttpGet("{planId}/with-courses")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor", "Trainee")]
        public async Task<IActionResult> GetByIdWithCourses(string planId)
        {
            var result = await _planService.GetByIdWithCoursesAsync(planId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get plans by status (Pending, Approved, Rejected)
        /// </summary>
        [HttpGet("status/{status}")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor", "Trainee")]
        public async Task<IActionResult> GetByStatus(string status)
        {
            var result = await _planService.GetAllByStatusAsync(status);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Create a new plan (Education Officer only)
        /// </summary>
        [HttpPost]
        [AuthorizeRoles("Education Officer")]
        public async Task<IActionResult> Create([FromBody] CreatePlanDto dto)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var result = await _planService.CreateAsync(dto, userId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return CreatedAtAction(nameof(GetById), new { planId = result.Data.PlanId }, result);
        }

        /// <summary>
        /// Update an existing plan (Education Officer only)
        /// </summary>
        [HttpPut("{planId}")]
        [AuthorizeRoles("Education Officer")]
        public async Task<IActionResult> Update(string planId, [FromBody] UpdatePlanDto dto)
        {
            var result = await _planService.UpdateAsync(planId, dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Delete a plan (Education Officer only)
        /// </summary>
        [HttpDelete("{planId}")]
        [AuthorizeRoles("Education Officer")]
        public async Task<IActionResult> Delete(string planId)
        {
            var result = await _planService.DeleteAsync(planId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Approve a plan (Administrator only)
        /// </summary>
        [HttpPost("{planId}/approve")]
        [AuthorizeRoles("Administrator")]
        public async Task<IActionResult> Approve(string planId)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var result = await _planService.ApproveAsync(planId, userId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Reject a plan (Administrator only)
        /// </summary>
        [HttpPost("{planId}/reject")]
        [AuthorizeRoles("Administrator")]
        public async Task<IActionResult> Reject(string planId)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var result = await _planService.RejectAsync(planId, userId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Send request for new plan
        /// </summary>
        [HttpPost("{planId}/request/new")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor", "Trainee")]
        public async Task<IActionResult> SendNewRequest(string planId, [FromBody] CreateRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token");
            }

            var result = await _requestService.CreateRequestAsync(dto, userId, planId, Domain.Enums.RequestType.NewPlan);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Send request to modify plan
        /// </summary>
        [HttpPost("{planId}/request/modify")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor", "Trainee")]
        public async Task<IActionResult> SendModifyRequest(string planId, [FromBody] CreateRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token");
            }

            var result = await _requestService.CreateRequestAsync(dto, userId, planId, Domain.Enums.RequestType.ModifyPlan);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
