using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class AllotedSlot
    {
        [ForeignKey("Slot")]
        public int SlotId { get; set; }

        [ForeignKey("Class")]
        public int ClassId { get; set; }
        public virtual Slot Slot { get; set; } = null!;
        public virtual Class Class { get; set; } = null!;
    }
}
