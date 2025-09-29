using Microsoft.EntityFrameworkCore;
using OrderingService.Application.Interfaces;
using OrderingService.Domain.Entities;
using OrderingService.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderingService.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderingDbContext _context;
        public OrderRepository(OrderingDbContext context) => _context = context;

        //public async Task<Order?> GetByIdAsync(Guid id)
        //    => await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);

        //public async Task<IEnumerable<Order>> GetByUserAsync(string userId)
        //    => await _context.Orders.Where(o => o.UserId == userId).ToListAsync();

        //public async Task AddAsync(Order order)
        //{
        //    _context.Orders.Add(order);
        //    await _context.SaveChangesAsync();
        //}

        //public async Task UpdateAsync(Order order)
        //{
        //    _context.Orders.Update(order);
        //    await _context.SaveChangesAsync();
        //}
        public async Task<Order> AddAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
