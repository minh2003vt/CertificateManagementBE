namespace Application.Dto.PlanDto
{
    /// <summary>
    /// DTO for subject information
    /// </summary>
    public class SubjectDto
    {
        public string SubjectId { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
