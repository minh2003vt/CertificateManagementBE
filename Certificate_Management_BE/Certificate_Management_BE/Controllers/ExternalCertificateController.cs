using Application.Dto.ExternalCertificateDto;
using Application.IServices;
using Certificate_Management_BE.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Certificate_Management_BE.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExternalCertificateController : ControllerBase
    {
        private readonly IExternalCertificateService _externalCertificateService;

        public ExternalCertificateController(IExternalCertificateService externalCertificateService)
        {
            _externalCertificateService = externalCertificateService;
        }

        /// <summary>
        /// Get all external certificates (id, code, name only)
        /// </summary>
        [HttpGet("all")]
        [AuthorizeRoles()] // All authenticated users
        public async Task<IActionResult> GetAll()
        {
            var result = await _externalCertificateService.GetAllAsync();
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get all external certificates by User ID (full details)
        /// </summary>
        [HttpGet("user/{userId}")]
        [AuthorizeRoles()] // All authenticated users
        public async Task<IActionResult> GetAllByUserId(string userId)
        {
            var result = await _externalCertificateService.GetAllByUserIdAsync(userId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get external certificate by ID (full details)
        /// </summary>
        [HttpGet("{id}")]
        [AuthorizeRoles()] // All authenticated users
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _externalCertificateService.GetByIdAsync(id);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Create new external certificate for a user
        /// </summary>
        [HttpPost]
        [AuthorizeRoles("Administrator", "Education Officer", "Trainee")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromQuery] string userId, [FromForm] CreateExternalCertificateDto dto)
        {
            var result = await _externalCertificateService.CreateAsync(userId, dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return CreatedAtAction(nameof(GetById), new { id = result.Data?.ExternalCertificateId }, result);
        }

        /// <summary>
        /// Update external certificate info 
        /// </summary>
        [HttpPut("{id}")]
        [AuthorizeRoles("Administrator", "Education Officer", "Trainee")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateExternalCertificateDto dto)
        {
            var result = await _externalCertificateService.UpdateAsync(id, dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Update certificate file (upload new image to Cloudinary)
        /// </summary>
        [HttpPut("{id}/file")]
        [AuthorizeRoles("Administrator", "Education Officer", "Trainee")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateFile(int id, IFormFile file)
        {
            var result = await _externalCertificateService.UpdateCertificateFileAsync(id, file);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Delete external certificate
        /// </summary>
        [HttpDelete("{id}")]
        [AuthorizeRoles("Administrator", "Education Officer")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _externalCertificateService.DeleteAsync(id);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}

