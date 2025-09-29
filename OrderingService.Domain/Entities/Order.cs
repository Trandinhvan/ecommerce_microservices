using OrderingService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderingService.Domain.Entities
{

    public class Order
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "VND";
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? PaymentReference { get; set; }
        public string? IdempotencyKey { get; set; }

        public List<OrderItem> Items { get; set; } = [];
    }
}
