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

        [ForeignKey("AssignedByUser")]
        public string? AssignedByUserId { get; set; }
        public User? AssignedByUser { get; set; }
        public DateTime AssignDate { get; set; } = DateTime.UtcNow;
        
        public OverallGradeStatus OverallGradeStatus { get; set; } = OverallGradeStatus.Pending;
        public AssignmentKind AssignmentKind { get; set; } = AssignmentKind.Initial;
        public DateTime GradeDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdateDate { get; set; } = DateTime.UtcNow;
        public virtual ICollection<TraineeAssignationGrade> Grades { get; set; } = [];
        public virtual ICollection<ClassTraineeAssignation> ClassTraineeAssignations { get; set; } = [];
    }
}
