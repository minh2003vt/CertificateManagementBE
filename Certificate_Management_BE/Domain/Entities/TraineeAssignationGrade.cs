using Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class TraineeAssignationGrade
    {
        [Key]
        public string TraineeAssignationGradeId { get; set; } = string.Empty;

        [ForeignKey("TraineeAssignation")]
        public string TraineeAssignationId { get; set; } = string.Empty;
        public virtual TraineeAssignation TraineeAssignation { get; set; } = null!;

        public GradeKind GradeKind { get; set; } = GradeKind.TotalScore;

        [Range(0, 10, ErrorMessage = "Value must be between 0 and 10.")]
        public decimal Grade { get; set; } = 0m;

        public GradeStatus GradeStatus { get; set; } = GradeStatus.Fail;

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
