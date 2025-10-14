namespace Application.Dto.PlanDto
{
    /// <summary>
    /// DTO for listing plans
    /// </summary>
    public class PlanListDto
    {
        public string PlanId { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Status { get; set; } = string.Empty; // Pending, Approved, Rejected
        public DateTime CreatedAt { get; set; }
    }
}
