using Application.Dto.SubjectDto;
using Application.Dto.RequestDto;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Certificate_Management_BE.Attributes;

namespace Certificate_Management_BE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;
        private readonly IRequestService _requestService;

        public SubjectController(ISubjectService subjectService, IRequestService requestService)
        {
            _subjectService = subjectService;
            _requestService = requestService;
        }

        /// <summary>
        /// Get all subjects (lightweight list)
        /// </summary>
        [HttpGet("all")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor", "Trainee")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _subjectService.GetAllAsync();

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get subject by ID (full details)
        /// </summary>
        [HttpGet("{subjectId}")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor", "Trainee")]
        public async Task<IActionResult> GetById(string subjectId)
        {
            var result = await _subjectService.GetByIdAsync(subjectId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Create a new subject
        /// </summary>
        [HttpPost]
        [AuthorizeRoles("Administrator", "Education Officer")]
        public async Task<IActionResult> Create([FromBody] CreateSubjectDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get current user ID from JWT token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }

            var result = await _subjectService.CreateAsync(dto, userId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetById), new { subjectId = result.Data?.SubjectId }, result);
        }

        /// <summary>
        /// Update an existing subject
        /// </summary>
        [HttpPut("{subjectId}")]
        [AuthorizeRoles("Administrator", "Education Officer")]
        public async Task<IActionResult> Update(string subjectId, [FromBody] UpdateSubjectDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _subjectService.UpdateAsync(subjectId, dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Delete a subject
        /// </summary>
        [HttpDelete("{subjectId}")]
        [AuthorizeRoles("Administrator", "Education Officer")]
        public async Task<IActionResult> Delete(string subjectId)
        {
            var result = await _subjectService.DeleteAsync(subjectId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Import multiple subjects from JSON array
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /Subject/import
        ///     [
        ///         {
        ///             "subjectId": "DGRFP008",
        ///             "subjectName": "Dangerous Goods Regulations for Personnel",
        ///             "description": "Training module covering dangerous goods regulations",
        ///             "minAttendance": null,
        ///             "minPracticeExamScore": 5,
        ///             "minFinalExamScore": 7,
        ///             "minTotalScore": 6.3
        ///         },
        ///         {
        ///             "subjectId": "ASFGS010",
        ///             "subjectName": "Aviation Security for Ground Service",
        ///             "description": "Training module covering aviation security",
        ///             "minAttendance": 6,
        ///             "minPracticeExamScore": 6,
        ///             "minFinalExamScore": 6,
        ///             "minTotalScore": 6.1
        ///         }
        ///     ]
        /// </remarks>
        /// <param name="subjects">List of subjects to import</param>
        /// <returns>Import result with success/failure counts and detailed errors</returns>
        [HttpPost("import")]
        [AuthorizeRoles("Administrator", "Education Officer")]
        public async Task<IActionResult> ImportSubjects([FromBody] List<SubjectImportDto> subjects)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (subjects == null || !subjects.Any())
            {
                return BadRequest(new { success = false, message = "No subjects provided for import" });
            }

            // Get current user ID from JWT token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }

            var result = await _subjectService.ImportSubjectsAsync(subjects, userId);

            // Return 200 OK even if some imports failed (partial success scenario)
            // The response will contain detailed information about successes and failures
            return Ok(result);
        }

        /// <summary>
        /// Get all subjects by status
        /// </summary>
        /// <param name="status">Status filter: Pending, Approved, or Rejected</param>
        [HttpGet("status/{status}")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor", "Trainee")]
        public async Task<IActionResult> GetByStatus(string status)
        {
            var result = await _subjectService.GetAllByStatusAsync(status);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Approve a pending subject
        /// </summary>
        [HttpPut("{subjectId}/approve")]
        [AuthorizeRoles("Administrator")]
        public async Task<IActionResult> Approve(string subjectId)
        {
            // Get current user ID from JWT token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }

            var result = await _subjectService.ApproveAsync(subjectId, userId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Reject a pending subject
        /// </summary>
        [HttpPut("{subjectId}/reject")]
        [AuthorizeRoles("Administrator")]
        public async Task<IActionResult> Reject(string subjectId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }

            var result = await _subjectService.RejectAsync(subjectId, userId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Send request for new subject
        /// </summary>
        [HttpPost("{subjectId}/request/new")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor", "Trainee")]
        public async Task<IActionResult> SendNewRequest(string subjectId, [FromBody] CreateRequestDto dto)
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

            var result = await _requestService.CreateRequestAsync(dto, userId, subjectId, Domain.Enums.RequestType.newSubject);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Send request to modify subject
        /// </summary>
        [HttpPost("{subjectId}/request/modify")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor", "Trainee")]
        public async Task<IActionResult> SendModifyRequest(string subjectId, [FromBody] CreateRequestDto dto)
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

            var result = await _requestService.CreateRequestAsync(dto, userId, subjectId, Domain.Enums.RequestType.modifySubject);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}

