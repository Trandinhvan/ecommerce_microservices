using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Contracts.Events
{
    public record OrderCreatedItem
    (
        Guid ProductId,
        string ProductName,
        decimal UnitPrice,
        int Quantity,
        decimal Total
    );
}
