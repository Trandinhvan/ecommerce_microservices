using OrderingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderingService.Application.Interfaces
{
    public interface IOrderRepository
    {
        //Task<Order?> GetByIdAsync(Guid id);
        //Task<IEnumerable<Order>> GetByUserAsync(string userId);
        //Task AddAsync(Order order);
        //Task UpdateAsync(Order order);
        Task<Order> AddAsync(Order order);
        Task<Order?> GetByIdAsync(Guid id);
        Task UpdateAsync(Order order);
    }
}
