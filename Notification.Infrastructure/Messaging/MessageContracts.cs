using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Infrastructure.Messaging
{
    public record OrderCreatedMq(Guid OrderId, string UserId, decimal TotalPrice, DateTime CreatedAt);
    public record OrderPaidMq(Guid OrderId, string UserId, decimal Amount, DateTime PaidAt);
}
