using OrderingService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderingService.Application.Interfaces
{
    public interface IOrderUseCase
    {
        Task<Guid> ExecuteAsync(string userId, IEnumerable<(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity)> basketItems);
        Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status);
    }
}
