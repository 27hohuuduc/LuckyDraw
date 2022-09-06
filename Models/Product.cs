using System;
using System.Collections.Generic;

namespace LuckyDraw.Models
{
    public partial class Product
    {
        public Product()
        {
            Gifts = new HashSet<Gift>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Gift> Gifts { get; set; }
    }
}
