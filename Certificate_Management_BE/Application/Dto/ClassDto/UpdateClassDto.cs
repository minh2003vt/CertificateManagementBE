using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Dto.ClassDto
{
    public class UpdateClassDto
    {
        [Required]
        public int ClassGroupId { get; set; }

        [Required]
        public string SubjectId { get; set; } = string.Empty;
    }
}


