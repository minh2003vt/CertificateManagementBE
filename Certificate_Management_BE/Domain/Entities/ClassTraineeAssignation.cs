using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ClassTraineeAssignation
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Class")]
        public int ClassId { get; set; }
        public virtual Class Class { get; set; } = null!;

        [Key, Column(Order = 1)]
        [ForeignKey("TraineeAssignation")]
        public string TraineeAssignationId { get; set; } = string.Empty;
        public virtual TraineeAssignation TraineeAssignation { get; set; } = null!;
    }
}
