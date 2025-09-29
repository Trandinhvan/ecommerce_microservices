using BasketService.Application.Interfaces;
using BasketService.Domain.Entities;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BasketService.Infrastructure.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase _db;
        public BasketRepository(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task<Basket?> GetBasketAsync(string userId)
        {
            var data = await _db.StringGetAsync(userId);
            if (data.IsNullOrEmpty) return null;
            return JsonSerializer.Deserialize<Basket>(data!);
        }

        public async Task UpdateBasketAsync(Basket basket)
        {
            if (basket == null || string.IsNullOrWhiteSpace(basket.UserId))
                throw new ArgumentException("Invalid basket");

            var data = JsonSerializer.Serialize(basket);

            // Lưu vào Redis với TTL = 7 ngày
            await _db.StringSetAsync(
                basket.UserId,
                data,
                expiry: TimeSpan.FromDays(7)
            );

            //return basket;
        }

        public async Task DeleteBasketAsync(string userId)
        {
            await _db.KeyDeleteAsync(userId);
        }
    }
}
