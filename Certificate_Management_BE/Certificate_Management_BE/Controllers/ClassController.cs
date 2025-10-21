using Application.Dto.ClassDto;
using Application.Dto.GradeImportDto;
using Application.IServices;
using Certificate_Management_BE.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Certificate_Management_BE.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClassController : ControllerBase
    {
        private readonly IClassService _classService;

        public ClassController(IClassService classService)
        {
            _classService = classService;
        }

        [HttpGet("all")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _classService.GetAllAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{classId}")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor")]
        public async Task<IActionResult> GetById(int classId)
        {
            var result = await _classService.GetByIdAsync(classId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost]
        [AuthorizeRoles("Education Officer")]
        public async Task<IActionResult> Create([FromBody] CreateClassDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var result = await _classService.CreateAsync(dto, userId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{classId}")]
        [AuthorizeRoles("Education Officer")]
        public async Task<IActionResult> Update(int classId, [FromBody] UpdateClassDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var result = await _classService.UpdateAsync(classId, dto, userId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{classId}")]
        [AuthorizeRoles("Education Officer")]
        public async Task<IActionResult> Delete(int classId)
        {
            var result = await _classService.DeleteAsync(classId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("{classId}/import-grades")]
        [AuthorizeRoles("Education Officer", "Instructor")]
        public async Task<IActionResult> ImportGrades(int classId, IFormFile file)
        {
            var result = await _classService.ImportGradesAsync(classId, file);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{classId}/trainee-assignations")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor")]
        public async Task<IActionResult> GetTraineeAssignationsWithGrades(int classId)
        {
            var result = await _classService.GetTraineeAssignationsWithGradesAsync(classId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}


