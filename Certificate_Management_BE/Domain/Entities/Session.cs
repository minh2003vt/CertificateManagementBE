using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Session
    {
        [Key]
        public string SessionId { get; set; } = string.Empty; // ID phiên, dùng jti từ JWT

        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;
        public virtual User User { get; set; } = null!;

        public DateTime LoginTime { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
        public DateTime? SessionExpiry { get; set; }  // Thời gian hết hạn token

        public virtual ICollection<AuditLog> AuditLogs { get; set; } = [];// Liên kết với AuditLog
    }
}