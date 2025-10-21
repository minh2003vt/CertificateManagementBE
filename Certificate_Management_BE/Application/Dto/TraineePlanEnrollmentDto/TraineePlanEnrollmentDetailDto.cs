using System;

namespace Application.Dto.TraineePlanEnrollmentDto
{
    public class TraineePlanEnrollmentDetailDto
    {
        public string TraineePlanEnrollmentId { get; set; } = string.Empty;
        public string TraineeId { get; set; } = string.Empty;
        public string TraineeName { get; set; } = string.Empty;
        public string PlanId { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public string? EnrolledByUserId { get; set; }
        public string? EnrolledByUserName { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
