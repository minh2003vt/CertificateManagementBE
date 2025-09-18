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
    public class Decision
    {
        [Key]
        public string DecisionId { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string DecisionCode { get; set; } = String.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public DateTime IssueDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);

        [ForeignKey("IssuedByUser")]
        public string IssuedByUserId { get; set; } = string.Empty;
        public virtual User IssuedByUser { get; set; } = null!;

        [ForeignKey("Certificate")]
        public string CertificateId { get; set; } = string.Empty;
        public virtual Certificate Certificate { get; set; } = null!;

        [ForeignKey("DecisionTemplate")]
        public string DecisionTemplateId { get; set; } = string.Empty;
        public virtual DecisionTemplate DecisionTemplate { get; set; } = null!;


        [Required]
        public DateTime SignDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);

        public DecisionStatus DecisionStatus { get; set; }
    }
}
