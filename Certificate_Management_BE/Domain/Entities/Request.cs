using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Request
    {
        [Key]
        public string RequestId { get; set; } = string.Empty;

        [ForeignKey("RequestUser")]
        public string RequestUserId { get; set; } = string.Empty;
        public virtual User RequestUser { get; set; } = null!;
        public RequestType RequestType { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
        public string Description { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        public RequestStatus Status { get; set; }

        [ForeignKey("ApprovedByUser")]
        public string? ApprovedByUserId { get; set; }
        public virtual User? ApprovedByUser { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
        public DateTime UpdatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
        public virtual ICollection<TraineeAssignation> TraineeAssignations { get; set; } = [];
    }
}
