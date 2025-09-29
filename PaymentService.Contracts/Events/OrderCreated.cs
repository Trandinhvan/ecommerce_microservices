using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Contracts.Events
{
    // Sau này chỉnh lại dùng 1 cái với cái OrderingService thôi.
    public record OrderCreated(
        Guid OrderId,
        string OrderNumber,
        string UserId,
        decimal TotalAmount,
        string Currency,
        string Status,
        DateTime CreatedAt,
        IEnumerable<OrderCreatedItem> Items
    );
}
