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
    public class TraineeAssignation
    {
        [Key]
        public string TraineeAssignationId { get; set; } = string.Empty;
        [ForeignKey("TraineeUser")]
        public string TraineeId { get; set; } = string.Empty; //assign trainee to course 
        public virtual User Trainee { get; set; } = null!;
        public RequestStatus RequestStatus { get; set; }

        [ForeignKey("AssignedByUser")]
        public string? AssignedByUserId { get; set; }
        public User? AssignedByUser { get; set; }
        public DateTime AssignDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
        [ForeignKey("ApprovedByUser")]
        public string? ApprovedByUserId { get; set; }
        public User? ApprovedByUser { get; set; }
        public DateTime? ApprovalDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
        [ForeignKey("Request")]
        public string RequestId { get; set; } = string.Empty;
        public virtual Request Request { get; set; } = null!;

        [ForeignKey("Subject")]
        public string SubjectId { get; set; } = string.Empty;
        public virtual Subject Subject { get; set; } = null!;
        public string Notes { get; set; } = string.Empty;
        [Required]
        [Range(0, 10, ErrorMessage = "Value must be between 0 and 10.")]
        public double TotalScore { get; set; } = 0;
        public int? Attendance { get; set; }

        [Range(0, 10, ErrorMessage = "Value must be between 0 and 10.")]
        public double? PracticeExamScore { get; set; }

        [Range(0, 10, ErrorMessage = "Value must be between 0 and 10.")]
        public double? FinalExamScore { get; set; }

        [Range(0, 10, ErrorMessage = "Value must be between 0 and 10.")]
        public double? ResitPracticeExamScore { get; set; }

        [Range(0, 10, ErrorMessage = "Value must be between 0 and 10.")]
        public double? ResitFinalExamScore { get; set; }
        public GradeStatus GradeStatus { get; set; } = GradeStatus.Pending;
        public GradeKind Gradekind { get; set; } = GradeKind.Initial;

        [ForeignKey("GradeUser")]
        public string? GradedByInstructorId { get; set; }
        public virtual User? GradedByInstructor { get; set; }

        public DateTime EvaluationDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
        public DateTime UpdateDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
        public virtual ICollection<ClassTraineeAssignation> ClassTraineeAssignations { get; set; } = [];
        //public virtual Grade Grade { get; set; } = null!;
    }
}
