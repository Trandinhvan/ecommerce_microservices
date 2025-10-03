using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Events
{
    public record OrderCreated(
        Guid OrderId,
        string OrderNumber,
        string UserId,
        decimal TotalAmount,
        string Currency,
        string Status, // Pending, Paid, Shipped, Cancelled
        DateTime CreatedAt,
        IEnumerable<OrderCreatedItem> Items
    );
}
