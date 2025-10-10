namespace Application.Dto.CourseSubjectSpecialtyDto
{
    /// <summary>
    /// DTO for listing course-subject-specialty relationships
    /// </summary>
    public class CourseSubjectSpecialtyListDto
    {
        public string SpecialtyId { get; set; } = string.Empty;
        public string SpecialtyName { get; set; } = string.Empty;
        public string SubjectId { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string CourseId { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}

