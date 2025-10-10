namespace Application.Dto.CourseDto
{
    /// <summary>
    /// DTO for detailed course information
    /// </summary>
    public class CourseDetailDto
    {
        public string CourseId { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty; // Pending, Approved, Rejected
        
        public string CreatedByUserId { get; set; } = string.Empty;
        public string? CreatedByUserName { get; set; }
        
        public string? AprovedUserId { get; set; }
        public string? AprovedUserName { get; set; }
        public DateTime? ApprovedAt { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

