using System;
using System.Collections.Generic;

namespace LuckyDraw.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Codes = new HashSet<Code>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public DateTime Birth { get; set; }
        public string? Position { get; set; }
        public string? TypeBusiness { get; set; }
        public string? Location { get; set; }
        public bool? IsBlock { get; set; }
        public byte[] Password { get; set; } = null!;
        public DateTime? AcceptanceTime { get; set; }

        public virtual ICollection<Code> Codes { get; set; }
    }
}
