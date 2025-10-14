using Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class RequestEntity
    {
        [ForeignKey("Request")]
        public string RequestId { get; set; } = string.Empty;
        public virtual Request Request { get; set; } = null!;

        public string EntityId { get; set; } = string.Empty;
        public RequestType RequestType { get; set; }
    }
}


