using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class AuditLog
    {
        [Key]
        public int AuditLogId { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;
        public virtual User User { get; set; } = null!;

        [ForeignKey("Session")]
        public string SessionId { get; set; } = string.Empty;
        public virtual Session Session { get; set; } = null!;

        public string Action { get; set; } = string.Empty;
        public string ActionDetails { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
    }
}