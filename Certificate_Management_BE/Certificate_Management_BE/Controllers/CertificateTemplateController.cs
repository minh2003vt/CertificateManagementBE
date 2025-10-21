using Application.Dto.CertificateTemplateDto;
using Application.IServices;
using Certificate_Management_BE.Attributes;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Certificate_Management_BE.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CertificateTemplateController : ControllerBase
    {
        private readonly ICertificateTemplateService _certificateTemplateService;
        public CertificateTemplateController(ICertificateTemplateService certificateTemplateService)
        {
            _certificateTemplateService = certificateTemplateService;
        }

        [HttpGet]
        [AuthorizeRoles("Administrator", "Education Officer")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _certificateTemplateService.GetAllAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{templateId}")]
        [AuthorizeRoles("Administrator", "Education Officer")]
        public async Task<IActionResult> GetById(string templateId)
        {
            var result = await _certificateTemplateService.GetByIdAsync(templateId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("kind/{certificateKind}")]
        [AuthorizeRoles("Administrator", "Education Officer")]
        public async Task<IActionResult> GetByCertificateKind(CertificateKind certificateKind)
        {
            var result = await _certificateTemplateService.GetByCertificateKindAsync(certificateKind);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost]
        [AuthorizeRoles("Administrator", "Education Officer")]
        public async Task<IActionResult> Create([FromBody] CreateCertificateTemplateDto dto)
        {
            var createdByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(createdByUserId))
            {
                return Unauthorized(new { Success = false, Message = "User ID not found in token" });
            }

            var result = await _certificateTemplateService.CreateAsync(dto, createdByUserId);
            if (!result.Success) return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { templateId = result.Data?.CertificateTemplateId }, result);
        }

        [HttpPost("{templateId}/import")]
        [AuthorizeRoles("Administrator", "Education Officer")]
        public async Task<IActionResult> ImportTemplate(string templateId, IFormFile templateFile)
        {
            var result = await _certificateTemplateService.ImportTemplateAsync(templateId, templateFile);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{templateId}")]
        [AuthorizeRoles("Administrator", "Education Officer")]
        public async Task<IActionResult> Update(string templateId, [FromBody] UpdateCertificateTemplateDto dto)
        {
            var updatedByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(updatedByUserId))
            {
                return Unauthorized(new { Success = false, Message = "User ID not found in token" });
            }

            var result = await _certificateTemplateService.UpdateAsync(templateId, dto, updatedByUserId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{templateId}")]
        [AuthorizeRoles("Administrator")]
        public async Task<IActionResult> Delete(string templateId)
        {
            var result = await _certificateTemplateService.DeleteAsync(templateId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{templateId}/approve")]
        [AuthorizeRoles("Administrator")]
        public async Task<IActionResult> Approve(string templateId)
        {
            var approvedByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(approvedByUserId))
            {
                return Unauthorized(new { Success = false, Message = "User ID not found in token" });
            }

            var result = await _certificateTemplateService.ApproveAsync(templateId, approvedByUserId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}
