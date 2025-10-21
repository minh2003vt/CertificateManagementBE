using Application.Dto.DecisionTemplateDto;
using Application.IServices;
using Certificate_Management_BE.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Certificate_Management_BE.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DecisionTemplateController : ControllerBase
    {
        private readonly IDecisionTemplateService _decisionTemplateService;

        public DecisionTemplateController(IDecisionTemplateService decisionTemplateService)
        {
            _decisionTemplateService = decisionTemplateService;
        }

        [HttpGet]
        [AuthorizeRoles("Administrator", "Education Officer")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _decisionTemplateService.GetAllAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{templateId}")]
        [AuthorizeRoles("Administrator", "Education Officer")]
        public async Task<IActionResult> GetById(string templateId)
        {
            var result = await _decisionTemplateService.GetByIdAsync(templateId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost]
        [AuthorizeRoles("Administrator", "Education Officer")]
        public async Task<IActionResult> Create([FromBody] CreateDecisionTemplateDto dto)
        {
            var createdByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(createdByUserId))
            {
                return Unauthorized(new { Success = false, Message = "User ID not found in token" });
            }

            var result = await _decisionTemplateService.CreateAsync(dto, createdByUserId);
            if (!result.Success) return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { templateId = result.Data?.DecisionTemplateId }, result);
        }

        [HttpPost("{templateId}/import")]
        [AuthorizeRoles("Administrator", "Education Officer")]
        public async Task<IActionResult> ImportTemplate(string templateId, [FromForm] IFormFile templateFile)
        {
            var result = await _decisionTemplateService.ImportTemplateAsync(templateId, templateFile);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{templateId}")]
        [AuthorizeRoles("Administrator", "Education Officer")]
        public async Task<IActionResult> Update(string templateId, [FromBody] UpdateDecisionTemplateDto dto)
        {
            var updatedByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(updatedByUserId))
            {
                return Unauthorized(new { Success = false, Message = "User ID not found in token" });
            }

            var result = await _decisionTemplateService.UpdateAsync(templateId, dto, updatedByUserId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{templateId}")]
        [AuthorizeRoles("Administrator")]
        public async Task<IActionResult> Delete(string templateId)
        {
            var result = await _decisionTemplateService.DeleteAsync(templateId);
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

            var result = await _decisionTemplateService.ApproveAsync(templateId, approvedByUserId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}
