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
    public class Grade
    {
        //[Key]
        //public string GradeId { get; set; } = string.Empty;

        //public double ParticipantScore { get; set; }
        //public double AssignmentScore    { get; set; }
        //public double FinalExamScore { get; set; }
        //public double? FinalResitScore { get; set; }
        //public double TotalScore { get; set; }
        //public GradeStatus GradeStatus { get; set; }
        //public string Remarks { get; set; } = string.Empty;

        //[ForeignKey("GradeUser")]
        //public string GradedByInstructorId { get; set; } = string.Empty;
        //public virtual User GradedByInstructor { get; set; } = null!;

        //public DateTime EvaluationDate { get; set; }= DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);
        //public DateTime UpdateDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);

        //[ForeignKey("TraineeAssignation")]
        //public string TraineeAssignationId { get; set; } = string.Empty;
        //public virtual TraineeAssignation TraineeAssignation { get; set; } = null!;

    }
}
