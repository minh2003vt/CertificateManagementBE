using System.ComponentModel.DataAnnotations;

namespace Application.Dto.RequestDto
{
    public class CreateRequestDto
    {
        [Required]
        public string Description { get; set; } = string.Empty;
        
        public string? Notes { get; set; }
    }
}
