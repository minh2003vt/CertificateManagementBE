using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Dto.ClassGroupDto
{
    public class UpdateClassGroupDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
        public string? Description { get; set; }
    }
}


