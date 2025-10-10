namespace Application.Dto.SubjectDto
{
    /// <summary>
    /// DTO for subject details (full information)
    /// </summary>
    public class SubjectDetailDto
    {
        public string SubjectId { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? MinAttendance { get; set; }
        public double? MinPracticeExamScore { get; set; }
        public double? MinFinalExamScore { get; set; }
        public double MinTotalScore { get; set; }
        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public string? AprovedUserId { get; set; }
        public string? AprovedUserName { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public DateTime? ApprovedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

