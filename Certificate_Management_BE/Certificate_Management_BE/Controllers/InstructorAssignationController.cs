using Application.Dto.InstructorAssignationDto;
using Application.IServices;
using Certificate_Management_BE.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Certificate_Management_BE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InstructorAssignationController : ControllerBase
    {
        private readonly IInstructorAssignationService _instructorAssignationService;

        public InstructorAssignationController(IInstructorAssignationService instructorAssignationService)
        {
            _instructorAssignationService = instructorAssignationService;
        }

        /// <summary>
        /// Get all instructor assignations
        /// </summary>
        [HttpGet]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _instructorAssignationService.GetAllAsync();
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get instructor assignation by composite key (SubjectId and InstructorId)
        /// </summary>
        [HttpGet("{subjectId}/{instructorId}")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor")]
        public async Task<IActionResult> GetById(string subjectId, string instructorId)
        {
            var result = await _instructorAssignationService.GetByIdAsync(subjectId, instructorId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get all assignations for a specific subject
        /// </summary>
        [HttpGet("subject/{subjectId}")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor")]
        public async Task<IActionResult> GetBySubjectId(string subjectId)
        {
            var result = await _instructorAssignationService.GetBySubjectIdAsync(subjectId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get all assignations for a specific instructor
        /// </summary>
        [HttpGet("instructor/{instructorId}")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor")]
        public async Task<IActionResult> GetByInstructorId(string instructorId)
        {
            var result = await _instructorAssignationService.GetByInstructorIdAsync(instructorId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Create a new instructor assignation (Education Officer only)
        /// </summary>
        [HttpPost]
        [AuthorizeRoles("Education Officer")]
        public async Task<IActionResult> Create([FromBody] CreateInstructorAssignationDto dto)
        {
            // Get user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }
            var userId = userIdClaim.Value;

            var result = await _instructorAssignationService.CreateAsync(dto, userId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return CreatedAtAction(nameof(GetById), new { subjectId = result.Data.SubjectId, instructorId = result.Data.InstructorId }, result);
        }

        /// <summary>
        /// Delete an instructor assignation (Education Officer only)
        /// </summary>
        [HttpDelete("{subjectId}/{instructorId}")]
        [AuthorizeRoles("Education Officer")]
        public async Task<IActionResult> Delete(string subjectId, string instructorId)
        {
            var result = await _instructorAssignationService.DeleteAsync(subjectId, instructorId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}

