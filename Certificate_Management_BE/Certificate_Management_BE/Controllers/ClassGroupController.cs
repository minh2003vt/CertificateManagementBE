using Application.Dto.ClassGroupDto;
using Application.IServices;
using Certificate_Management_BE.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Certificate_Management_BE.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClassGroupController : ControllerBase
    {
        private readonly IClassGroupService _service;

        public ClassGroupController(IClassGroupService service)
        {
            _service = service;
        }

        [HttpGet("all")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{classGroupId}")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor")]
        public async Task<IActionResult> GetById(int classGroupId)
        {
            var result = await _service.GetByIdAsync(classGroupId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{classGroupId}/classes")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor")]
        public async Task<IActionResult> GetClassesByGroupId(int classGroupId)
        {
            var result = await _service.GetClassesByGroupIdAsync(classGroupId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost]
        [AuthorizeRoles("Education Officer")]
        public async Task<IActionResult> Create([FromBody] CreateClassGroupDto dto)
        {
            var result = await _service.CreateAsync(dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{classGroupId}")]
        [AuthorizeRoles("Education Officer")]
        public async Task<IActionResult> Update(int classGroupId, [FromBody] UpdateClassGroupDto dto)
        {
            var result = await _service.UpdateAsync(classGroupId, dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{classGroupId}")]
        [AuthorizeRoles("Education Officer")]
        public async Task<IActionResult> Delete(int classGroupId)
        {
            var result = await _service.DeleteAsync(classGroupId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}


