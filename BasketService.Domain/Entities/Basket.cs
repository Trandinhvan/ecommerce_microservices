using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketService.Domain.Entities
{
    public class BasketItem
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class Basket
    {
        public string UserId { get; set; } = default!;
        public List<BasketItem> Items { get; set; } = [];
    }
}
