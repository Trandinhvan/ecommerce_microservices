using PaymentService.Application.DTOs;
using PaymentService.Application.Interfaces;
using PaymentService.Contracts.Events;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.UseCase
{
    public class PaymentUseCase : IPaymentUseCase
    {
        private readonly IPaymentRepository _respository;
        private readonly IPaymentPublisher _publisher;

        public PaymentUseCase(IPaymentRepository respository, IPaymentPublisher publisher)
        {
            _respository = respository;
            _publisher = publisher;
        }

        public async Task<PaymentDto> HandlePaymentAsync(CreatePaymentRequest request)
        {
            Console.WriteLine($"👉 Bắt đầu xử lý thanh toán: OrderId={request.OrderId}, UserId={request.UserId}, Amount={request.Amount}");
            var payment = new Payment
            {
                OrderId = request.OrderId,
                UserId = request.UserId,
                Amount = request.Amount,
                Status = PaymentStatus.Pending
            };

            await _respository.AddAsync(payment);

            // 🔹 Giả lập gọi MoMo API (sau này tích hợp thật)
            bool momoResult = new Random().Next(0, 2) == 1;
            
            if (momoResult)
            {
                payment.Status = PaymentStatus.Success;
                await _respository.UpdateAsync(payment);

                await _publisher.PublishPaymentSucceededAsync(new PaymentSucceeded(
                    payment.OrderId, payment.UserId, payment.Amount
                ));
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
                await _respository.UpdateAsync(payment);

                await _publisher.PublishPaymentFailedAsync(new PaymentFailed(
                    payment.OrderId, payment.UserId, payment.Amount
                ));
            }

            return new PaymentDto(payment.Id, payment.OrderId, payment.UserId, payment.Amount, payment.Status.ToString());
        }
    }
}
