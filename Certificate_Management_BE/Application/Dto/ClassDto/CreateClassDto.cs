using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Dto.ClassDto
{
    public class CreateClassDto
    {
        [Required]
        public string InstructorId { get; set; } = string.Empty;

        [Required]
        public int ClassGroupId { get; set; }

        [Required]
        public string SubjectId { get; set; } = string.Empty;
    }
}


