using System;

namespace Application.Dto.RequestDto
{
    public class RequestListDto
    {
        public string RequestId { get; set; } = string.Empty;
        public string RequestUserId { get; set; } = string.Empty;
        public string RequestUserName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string? ApprovedByUserId { get; set; }
        public string? ApprovedByUserName { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int RequestEntitiesCount { get; set; }
    }
}
