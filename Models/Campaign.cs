using System;
using System.Collections.Generic;

namespace LuckyDraw.Models
{
    public partial class Campaign
    {
        public Campaign()
        {
            Codes = new HashSet<Code>();
            Gifts = new HashSet<Gift>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? Limit { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public virtual ICollection<Code> Codes { get; set; }
        public virtual ICollection<Gift> Gifts { get; set; }
    }
}
