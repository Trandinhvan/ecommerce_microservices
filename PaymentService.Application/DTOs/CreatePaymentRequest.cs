using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.DTOs
{
    public record CreatePaymentRequest(Guid OrderId, string UserId, decimal Amount, string Method);
}
