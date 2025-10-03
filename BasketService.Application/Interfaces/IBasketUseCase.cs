using BasketService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketService.Application.Interfaces
{
    public interface IBasketUseCase
    {
        Task<Basket?> GetBasketAsync(string userId);
        Task AddItemToBasketAsync(string userId, BasketItem item);
        Task UpdateToBasketAsync(string userId, Guid productId, int quantity);
        Task RemoveItemAsync(string userId, Guid productId);
        Task DeleteBasketAsync(string userId);
    }
}
