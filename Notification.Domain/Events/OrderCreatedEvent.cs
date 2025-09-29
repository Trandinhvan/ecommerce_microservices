using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Domain.Events
{
    public record OrderCreatedEvent(Guid OrderId, string UserId, decimal TotalPrice, DateTime CreatedAt);
}
