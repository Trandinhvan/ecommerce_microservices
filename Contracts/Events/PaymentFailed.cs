using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderingService.Contracts.Events
{
    public record PaymentFailed(Guid OrderId, string UserId, decimal Amount);
}
