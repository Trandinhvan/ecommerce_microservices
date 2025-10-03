using PaymentService.Domain.Entities;
using PaymentService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.Interfaces
{
    public interface IPaymentGateway
    {
        PaymentMethod Method { get; }
        Task<bool> ProcessPaymentAsync(Payment payment);
    }
}
