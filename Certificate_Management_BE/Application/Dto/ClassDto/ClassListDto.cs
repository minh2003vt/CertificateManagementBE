using System;

namespace Application.Dto.ClassDto
{
    public class ClassListDto
    {
        public int ClassId { get; set; }
        public string SubjectId { get; set; } = string.Empty;
        public string InstructorId { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public int ClassGroupId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}


