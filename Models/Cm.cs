using System;
using System.Collections.Generic;

namespace LuckyDraw.Models
{
    public partial class Cm
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;
        public DateTime? AcceptanceTime { get; set; }
    }
}
