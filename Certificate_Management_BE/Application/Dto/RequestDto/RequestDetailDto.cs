using System;
using System.Collections.Generic;

namespace Application.Dto.RequestDto
{
    public class RequestDetailDto
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
        public DateTime UpdatedAt { get; set; }
        public List<RequestEntityDto> RequestEntities { get; set; } = [];
    }
}
