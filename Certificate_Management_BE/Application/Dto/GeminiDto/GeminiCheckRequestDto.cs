using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Application.Dto.GeminiDto
{
    public class GeminiCheckRequestDto
    {
        [Required]
        public JsonElement JsonPayload { get; set; }
    }
}


