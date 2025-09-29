using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Domain.Events
{
    public record OrderPaidEvent(Guid OrderId, string UserId, decimal Amount, DateTime PaidAt);
}
