namespace Application.Dto.SubjectDto
{
    /// <summary>
    /// DTO for listing subjects (lightweight)
    /// </summary>
    public class SubjectListDto
    {
        public string SubjectId { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double MinTotalScore { get; set; }
        public string Status { get; set; } = string.Empty; // Pending, Approved, Rejected
    }
}

