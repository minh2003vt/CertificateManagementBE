namespace Application.Dto.CourseSubjectSpecialtyDto
{
    /// <summary>
    /// DTO for detailed course-subject-specialty relationship information
    /// </summary>
    public class CourseSubjectSpecialtyDetailDto
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

