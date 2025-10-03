using PaymentService.Contracts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.Interfaces
{
    public interface IPaymentPublisher
    {
        Task PublishPaymentSucceededAsync(PaymentSucceeded evt);
        Task PublishPaymentFailedAsync(PaymentFailed evt);
    }
}
