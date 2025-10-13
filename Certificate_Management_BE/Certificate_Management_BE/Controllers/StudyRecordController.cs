using Application.Dto.StudyRecordDto;
using Application.IServices;
using Application.ServiceResponse;
using Certificate_Management_BE.Attributes;
using Certificate_Management_BE.Extensions;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Certificate_Management_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StudyRecordController : ControllerBase
    {
        private readonly IStudyRecordService _studyRecordService;

        public StudyRecordController(IStudyRecordService studyRecordService)
        {
            _studyRecordService = studyRecordService;
        }

        /// <summary>
        /// Get study records by plan and course
        /// </summary>
        [HttpGet("plan/{planId}/course/{courseId}")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor")]
        public async Task<IActionResult> GetByPlanAndCourse(string planId, string courseId)
        {
            var result = await _studyRecordService.GetByPlanAndCourseAsync(planId, courseId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Create study records for a plan and course (auto-adds subjects based on specialty)
        /// </summary>
        [HttpPost]
        [AuthorizeRoles("Education Officer")]
        public async Task<IActionResult> Create([FromBody] CreateStudyRecordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var  userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token");
            }

            var result = await _studyRecordService.CreateAsync(dto, userId);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Delete all study records by plan ID
        /// </summary>
        [HttpDelete("plan/{planId}")]
        [AuthorizeRoles("Education Officer")]
        public async Task<IActionResult> DeleteByPlanId(string planId)
        {
            var result = await _studyRecordService.DeleteByPlanIdAsync(planId);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Delete a specific study record by plan, course, and subject IDs
        /// </summary>
        [HttpDelete("plan/{planId}/course/{courseId}/subject/{subjectId}")]
        [AuthorizeRoles("Education Officer")]
        public async Task<IActionResult> DeleteSpecific(string planId, string courseId, string subjectId)
        {
            var result = await _studyRecordService.DeleteSpecificAsync(planId, courseId, subjectId);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
