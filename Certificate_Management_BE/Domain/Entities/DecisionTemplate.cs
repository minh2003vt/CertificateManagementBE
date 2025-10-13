using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    public class DecisionTemplate
    {
        [Key]
        public string DecisionTemplateId { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string TemplateName { get; set; } = string.Empty;

        [Required, Column(TypeName = "text")]
        public string TemplateContent { get; set; } = string.Empty; // HTML content

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("CreatedByUser")]
        public string CreatedByUserId { get; set; } = string.Empty;
        public virtual User CreatedByUser { get; set; } = null!;

        [ForeignKey("ApprovedByUser")]
        public string? ApprovedByUserId { get; set; }
        public virtual User? ApprovedByUser { get; set; }

        public DateTime? LastUpdatedAt { get; set; }

        public TemplateStatus TemplateStatus { get; set; }
        public virtual ICollection<Decision> Decisions { get; set; } = [];
    }
}
