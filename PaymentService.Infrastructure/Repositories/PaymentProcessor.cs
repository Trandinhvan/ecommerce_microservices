//using PaymentService.Application.Interfaces;
//using PaymentService.Domain.Entities;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PaymentService.Infrastructure.Repositories
//{
//    public class PaymentProcessor : IPaymentProcessor
//    {
//        public Task<PaymentStatus> ProcessPaymentAsync(Payment payment)
//        {
//            // Giả lập thanh toán: 90% success, 10% fail
//            var rnd = new Random();
//            var status = rnd.NextDouble() < 0.9 ? PaymentStatus.Success : PaymentStatus.Failed;
//            return Task.FromResult(status);
//        }
//    }
//}
