using System;

namespace Application.Dto.PlanDto
{
    /// <summary>
    /// DTO for plan with courses and subjects information
    /// </summary>
    public class PlanWithCoursesDto
    {
        public string PlanId { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Status { get; set; } = string.Empty; // Pending, Approved, Rejected
        
        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        
        public string? AprovedUserId { get; set; }
        public string? AprovedUserName { get; set; }
        public DateTime? ApprovedAt { get; set; }
        
        public string SpecialtyId { get; set; } = string.Empty;
        public string? SpecialtyName { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public List<CourseWithSubjectsDto> Courses { get; set; } = [];
    }
}
