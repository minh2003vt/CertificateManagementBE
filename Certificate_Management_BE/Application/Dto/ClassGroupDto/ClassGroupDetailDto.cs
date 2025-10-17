using System;

namespace Application.Dto.ClassGroupDto
{
    public class ClassGroupDetailDto
    {
        public int ClassGroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}


