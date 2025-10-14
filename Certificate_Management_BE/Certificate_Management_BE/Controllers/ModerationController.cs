using Application.Dto.GeminiDto;
using Application.IServices;
using Certificate_Management_BE.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Certificate_Management_BE.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ModerationController : ControllerBase
    {
        private readonly IGeminiService _geminiService;

        public ModerationController(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        /// <summary>
        /// Check JSON payload for sensitive content. Returns only "Qualified" or "Unqualified" in Data.
        /// </summary>
        [HttpPost("check-json")]
        [AuthorizeRoles()] // any authenticated user; adjust if needed
        public async Task<IActionResult> CheckJson([FromBody] GeminiCheckRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Serialize back to a compact JSON string so FE can send raw JSON
            var compactJson = System.Text.Json.JsonSerializer.Serialize(dto.JsonPayload);
            var result = await _geminiService.CheckSensitiveAsync(compactJson);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}


