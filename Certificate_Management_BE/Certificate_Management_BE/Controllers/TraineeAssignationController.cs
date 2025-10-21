using Application.Dto.TraineeAssignationDto;
using Application.IServices;
using Application.ServiceResponse;
using Certificate_Management_BE.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Certificate_Management_BE.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TraineeAssignationController : ControllerBase
    {
        private readonly ITraineeAssignationService _service;

        public TraineeAssignationController(ITraineeAssignationService service)
        {
            _service = service;
        }

        [HttpPost]
        [AuthorizeRoles("Education Officer", "Administrator")]
        public async Task<IActionResult> Create([FromBody] CreateTraineeAssignationDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var result = await _service.CreateAsync(dto, currentUserId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AuthorizeRoles()]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _service.GetByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpGet("class/{classId:int}")]
        [AuthorizeRoles()]
        public async Task<IActionResult> GetByClass(int classId)
        {
            var result = await _service.GetByClassAsync(classId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [AuthorizeRoles("Education Officer", "Administrator")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateTraineeAssignationDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [AuthorizeRoles("Education Officer", "Administrator")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
    }
}


