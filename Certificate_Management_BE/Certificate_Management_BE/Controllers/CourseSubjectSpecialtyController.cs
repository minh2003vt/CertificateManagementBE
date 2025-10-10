using Application.Dto.CourseSubjectSpecialtyDto;
using Application.IServices;
using Certificate_Management_BE.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Certificate_Management_BE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CourseSubjectSpecialtyController : ControllerBase
    {
        private readonly ICourseSubjectSpecialtyService _courseSubjectSpecialtyService;

        public CourseSubjectSpecialtyController(ICourseSubjectSpecialtyService courseSubjectSpecialtyService)
        {
            _courseSubjectSpecialtyService = courseSubjectSpecialtyService;
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
    }
}

