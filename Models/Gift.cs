using System;
using System.Collections.Generic;

namespace LuckyDraw.Models
{
    public partial class Gift
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public int? Count { get; set; }
        public int? IdCampaign { get; set; }
        public int? IdProduct { get; set; }

        public virtual Campaign? IdCampaignNavigation { get; set; }
        public virtual Product? IdProductNavigation { get; set; }
    }
}
