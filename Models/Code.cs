using System;
using System.Collections.Generic;

namespace LuckyDraw.Models
{
    public partial class Code
    {
        public int Id { get; set; }
        public string Code1 { get; set; } = null!;
        public int? Count { get; set; }
        public int? IdCampaign { get; set; }
        public int? IdCustomer { get; set; }

        public virtual Campaign? IdCampaignNavigation { get; set; }
        public virtual Customer? IdCustomerNavigation { get; set; }
    }
}
