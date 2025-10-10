using Application.Dto.SpecialtyDto;
using Application.IServices;
using Certificate_Management_BE.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Certificate_Management_BE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SpecialtyController : ControllerBase
    {
        private readonly ISpecialtyService _specialtyService;

        public SpecialtyController(ISpecialtyService specialtyService)
        {
            _specialtyService = specialtyService;
        }

        /// <summary>
        /// Get all specialties
        /// </summary>
        [HttpGet]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor", "Trainee")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _specialtyService.GetAllAsync();
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get specialty by ID
        /// </summary>
        [HttpGet("{specialtyId}")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor", "Trainee")]
        public async Task<IActionResult> GetById(string specialtyId)
        {
            var result = await _specialtyService.GetByIdAsync(specialtyId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Create a new specialty (Education Officer only)
        /// </summary>
        [HttpPost]
        [AuthorizeRoles("Education Officer")]
        public async Task<IActionResult> Create([FromBody] CreateSpecialtyDto dto)
        {
            // Get user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }
            var userId = userIdClaim.Value;

            var result = await _specialtyService.CreateAsync(dto, userId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return CreatedAtAction(nameof(GetById), new { specialtyId = result.Data.SpecialtyId }, result);
        }

        /// <summary>
        /// Update an existing specialty (Education Officer only)
        /// </summary>
        [HttpPut("{specialtyId}")]
        [AuthorizeRoles("Education Officer")]
        public async Task<IActionResult> Update(string specialtyId, [FromBody] UpdateSpecialtyDto dto)
        {
            // Get user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }
            var userId = userIdClaim.Value;

            var result = await _specialtyService.UpdateAsync(specialtyId, dto, userId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Delete a specialty (Education Officer only)
        /// </summary>
        [HttpDelete("{specialtyId}")]
        [AuthorizeRoles("Education Officer")]
        public async Task<IActionResult> Delete(string specialtyId)
        {
            var result = await _specialtyService.DeleteAsync(specialtyId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}

