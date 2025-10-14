using Application.Dto.CourseSubjectSpecialtyDto;
using Application.Dto.RequestDto;
using Application.IServices;
using Certificate_Management_BE.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Certificate_Management_BE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CourseSubjectSpecialtyController : ControllerBase
    {
        private readonly ICourseSubjectSpecialtyService _courseSubjectSpecialtyService;
        private readonly IRequestService _requestService;

        public CourseSubjectSpecialtyController(ICourseSubjectSpecialtyService courseSubjectSpecialtyService, IRequestService requestService)
        {
            _courseSubjectSpecialtyService = courseSubjectSpecialtyService;
            _requestService = requestService;
        }

        /// <summary>
        /// Get all course-subject-specialty relationships
        /// </summary>
        [HttpGet]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _courseSubjectSpecialtyService.GetAllAsync();
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get course-subject-specialty relationship by composite key
        /// </summary>
        [HttpGet("{specialtyId}/{subjectId}/{courseId}")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor")]
        public async Task<IActionResult> GetById(string specialtyId, string subjectId, string courseId)
        {
            var result = await _courseSubjectSpecialtyService.GetByIdAsync(specialtyId, subjectId, courseId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get all relationships for a specific course
        /// </summary>
        [HttpGet("course/{courseId}")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor")]
        public async Task<IActionResult> GetByCourseId(string courseId)
        {
            var result = await _courseSubjectSpecialtyService.GetByCourseIdAsync(courseId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get all relationships for a specific specialty
        /// </summary>
        [HttpGet("specialty/{specialtyId}")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor")]
        public async Task<IActionResult> GetBySpecialtyId(string specialtyId)
        {
            var result = await _courseSubjectSpecialtyService.GetBySpecialtyIdAsync(specialtyId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get all relationships for a specific subject
        /// </summary>
        [HttpGet("subject/{subjectId}")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor")]
        public async Task<IActionResult> GetBySubjectId(string subjectId)
        {
            var result = await _courseSubjectSpecialtyService.GetBySubjectIdAsync(subjectId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Create a new course-subject-specialty relationship (Education Officer only)
        /// </summary>
        [HttpPost]
        [AuthorizeRoles("Education Officer")]
        public async Task<IActionResult> Create([FromBody] CreateCourseSubjectSpecialtyDto dto)
        {
            var result = await _courseSubjectSpecialtyService.CreateAsync(dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return CreatedAtAction(nameof(GetById), 
                new { specialtyId = result.Data.SpecialtyId, subjectId = result.Data.SubjectId, courseId = result.Data.CourseId }, 
                result);
        }

        /// <summary>
        /// Delete all relationships for a specific specialty (Education Officer only)
        /// </summary>
        [HttpDelete("specialty/{specialtyId}")]
        [AuthorizeRoles("Education Officer")]
        public async Task<IActionResult> DeleteBySpecialtyId(string specialtyId)
        {
            var result = await _courseSubjectSpecialtyService.DeleteBySpecialtyIdAsync(specialtyId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Delete all relationships for a specific course (Education Officer only)
        /// </summary>
        [HttpDelete("course/{courseId}")]
        [AuthorizeRoles("Education Officer")]
        public async Task<IActionResult> DeleteByCourseId(string courseId)
        {
            var result = await _courseSubjectSpecialtyService.DeleteByCourseIdAsync(courseId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Send request for new course-subject-specialty relationship
        /// </summary>
        [HttpPost("{specialtyId}/{subjectId}/{courseId}/request/new")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor", "Trainee")]
        public async Task<IActionResult> SendNewRequest(string specialtyId, string subjectId, string courseId, [FromBody] CreateRequestDto dto)
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

            // Set the entity ID (using composite key format)
            var entityId = $"{specialtyId}{subjectId}{courseId}";

            var result = await _requestService.CreateRequestAsync(dto, userId, entityId, Domain.Enums.RequestType.NewMatrix);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Send request to modify course-subject-specialty relationship
        /// </summary>
        [HttpPost("{specialtyId}/{subjectId}/{courseId}/request/modify")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor", "Trainee")]
        public async Task<IActionResult> RemoveMatrixRequest(string specialtyId, string subjectId, string courseId, [FromBody] CreateRequestDto dto)
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

            // Set the entity ID (using composite key format)
            var entityId = $"{specialtyId}{subjectId}{courseId}";

            var result = await _requestService.CreateRequestAsync(dto, userId, entityId, Domain.Enums.RequestType.RemoveMatrix);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}

