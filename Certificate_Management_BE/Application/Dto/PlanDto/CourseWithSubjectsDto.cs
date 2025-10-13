namespace Application.Dto.PlanDto
{
    /// <summary>
    /// DTO for course with its subjects
    /// </summary>
    public class CourseWithSubjectsDto
    {
        public string CourseId { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<SubjectDto> Subjects { get; set; } = [];
    }
}
