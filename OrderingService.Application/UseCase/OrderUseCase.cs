using OrderingService.Application.Interfaces;
using OrderingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Events;
using OrderingService.Domain.Enums;

namespace OrderingService.Application.UseCase
{
    public class OrderUseCase : IOrderUseCase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMessageBus _messageBus;

        public OrderUseCase(IOrderRepository orderRepository, IMessageBus messageBus)
        {
            _orderRepository = orderRepository;
            _messageBus = messageBus;
        }

        public async Task<Guid> ExecuteAsync(
            string userId,
            IEnumerable<(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity)> basketItems)
        {
            if (basketItems == null || !basketItems.Any())
                throw new ArgumentException("Giỏ hàng trống.");

            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = $"ORD-{DateTime.UtcNow.Ticks}",
                UserId = userId,
                Items = basketItems.Select(i => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    Total = i.UnitPrice * i.Quantity
                }).ToList()
            };

            order.TotalAmount = order.Items.Sum(i => i.Total);

            // Lưu vào database
            await _orderRepository.AddAsync(order);

            // Map sang Contract Event
            var evt = new OrderCreated(
                order.Id,
                order.OrderNumber,
                order.UserId,
                order.TotalAmount,
                order.Currency,
                order.Status.ToString(),   // 🔑 đổi sang string
                order.CreatedAt,
                order.Items.Select(i => new OrderCreatedItem(
                    i.ProductId,
                    i.ProductName,
                    i.UnitPrice,
                    i.Quantity,
                    i.Total
                ))
            );

            //await _messageBus.PublishAsync("order-created", evt);
            await _messageBus.PublishAsync(evt, "order.created");

            return order.Id;
        }

        public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status)
        {
            // Lấy order từ DB
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                Console.WriteLine($"[OrderUseCase] Order {orderId} not found.");
                return;
            }

            // Update trạng thái
            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);

            Console.WriteLine($"[OrderUseCase] Order {orderId} status updated to {status}");
        }
    }
}
