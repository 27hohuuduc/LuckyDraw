using System;
using System.Collections.Generic;

namespace LuckyDraw.Models
{
    public partial class GiftCode
    {
        public string Code { get; set; } = null!;
        public bool? IsActive { get; set; }
        public int? IdGift { get; set; }

        public virtual Gift? IdGiftNavigation { get; set; }
    }
}
