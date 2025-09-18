using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Slot
    {
        public int SlotId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }

        // TimeSpan is a better choice than DateTime to store just the time part
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public virtual ICollection<AllotedSlot> AllotedSlots { get; set; } = [];

    }
}
