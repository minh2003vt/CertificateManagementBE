using System;

namespace Application.Dto.RequestDto
{
    public class RequestEntityDto
    {
        public string RequestId { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string RequestType { get; set; } = string.Empty;
    }
}
