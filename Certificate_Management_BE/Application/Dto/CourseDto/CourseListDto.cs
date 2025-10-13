namespace Application.Dto.CourseDto
{
    /// <summary>
    /// DTO for listing courses
    /// </summary>
    public class CourseListDto
    {
        public string CourseId { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty; // Pending, Approved, Rejected
        public DateTime CreatedAt { get; set; }
    }
}

