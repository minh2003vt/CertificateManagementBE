using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Application.Dto.GeminiDto
{
    public class GeminiCheckRequestDto
    {
        [Required]
        public string JsonPayload { get; set; }
    }
}


