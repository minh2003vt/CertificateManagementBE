using Application.Dto.CourseDto;
using Application.IServices;
using Certificate_Management_BE.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Certificate_Management_BE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        /// <summary>
        /// Get all courses
        /// </summary>
        [HttpGet]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor", "Trainee")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _courseService.GetAllAsync();
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get course by ID
        /// </summary>
        [HttpGet("{courseId}")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor", "Trainee")]
        public async Task<IActionResult> GetById(string courseId)
        {
            var result = await _courseService.GetByIdAsync(courseId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get courses by status (Pending, Approved, Rejected)
        /// </summary>
        [HttpGet("status/{status}")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor", "Trainee")]
        public async Task<IActionResult> GetByStatus(string status)
        {
            var result = await _courseService.GetAllByStatusAsync(status);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get all subjects for a specific course
        /// </summary>
        [HttpGet("{courseId}/subjects")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor", "Trainee")]
        public async Task<IActionResult> GetAllSubjects(string courseId)
        {
            var result = await _courseService.GetAllSubjectsByCourseIdAsync(courseId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Create a new course (Education Officer only)
        /// </summary>
        [HttpPost]
        [AuthorizeRoles("Education Officer")]
        public async Task<IActionResult> Create([FromBody] CreateCourseDto dto)
        {
            // Get user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }
            var userId = userIdClaim.Value;

            var result = await _courseService.CreateAsync(dto, userId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return CreatedAtAction(nameof(GetById), new { courseId = result.Data.CourseId }, result);
        }

        /// <summary>
        /// Update an existing course (Education Officer only)
        /// </summary>
        [HttpPut("{courseId}")]
        [AuthorizeRoles("Education Officer")]
        public async Task<IActionResult> Update(string courseId, [FromBody] UpdateCourseDto dto)
        {
            var result = await _courseService.UpdateAsync(courseId, dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Delete a course (Education Officer only)
        /// </summary>
        [HttpDelete("{courseId}")]
        [AuthorizeRoles("Education Officer")]
        public async Task<IActionResult> Delete(string courseId)
        {
            var result = await _courseService.DeleteAsync(courseId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Approve a course (Administrator only)
        /// </summary>
        [HttpPut("{courseId}/approve")]
        [AuthorizeRoles("Administrator")]
        public async Task<IActionResult> Approve(string courseId)
        {
            // Get user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }
            var userId = userIdClaim.Value;

            var result = await _courseService.ApproveAsync(courseId, userId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Reject a course (Administrator only)
        /// </summary>
        [HttpPut("{courseId}/reject")]
        [AuthorizeRoles("Administrator")]
        public async Task<IActionResult> Reject(string courseId)
        {
            // Get user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }
            var userId = userIdClaim.Value;

            var result = await _courseService.RejectAsync(courseId, userId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}

