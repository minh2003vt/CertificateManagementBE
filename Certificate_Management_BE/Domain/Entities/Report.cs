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
    public class Report
    {
        [Key]
        public string ReportId { get; set; } = string.Empty;
        public string ReportName { get; set; } = string.Empty;
        public ReportType ReportType { get; set; }
        [ForeignKey("GeneratedByUser")]
        public string GeneratedByUserId { get; set; } = string.Empty;
        public virtual User GeneratedByUser { get; set; } = null!;
        public DateTime GenerateDate { get; set; } = DateTime.UtcNow;
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        public string Content { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
    }
}
