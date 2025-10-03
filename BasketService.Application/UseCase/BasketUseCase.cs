using BasketService.Application.Interfaces;
using BasketService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketService.Application.UseCase
{
    public class BasketUseCase : IBasketUseCase
    {
        private readonly IBasketRepository _basketRepository;

        public BasketUseCase(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }

        /// <summary>
        /// Lấy giỏ hàng của user, Nếu chưa có, trả về null
        /// </summary>
        public async Task<Basket?> GetBasketAsync(string userId)
        {
            return await _basketRepository.GetBasketAsync(userId);
        }
        public async Task AddItemToBasketAsync(string userId, BasketItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            // Lấy giỏ hàng từ Redis
            var basket = await _basketRepository.GetBasketAsync(userId);

            // Nếu chưa có thì tạo mới
            if (basket == null)
            {
                basket = new Basket
                {
                    UserId = userId,
                    Items = new List<BasketItem>()
                };
            }

            // Kiểm tra nếu sản phẩm đã có trong giỏ thì tăng số lượng
            var existingItem = basket.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                basket.Items.Add(item);
            }

            // Lưu lại vào Redis
            await _basketRepository.UpdateBasketAsync(basket);
        }

        public async Task UpdateToBasketAsync(string userId, Guid productId, int quantity)
        {
            var basket = await _basketRepository.GetBasketAsync(userId);
            if (basket == null) return;

            var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity;
                await _basketRepository.UpdateBasketAsync(basket);
            }
        }

        public async Task RemoveItemAsync(string userId, Guid productId)
        {
            var basket = await _basketRepository.GetBasketAsync(userId);
            if (basket == null) return;

            basket.Items.RemoveAll(i => i.ProductId == productId);
            await _basketRepository.UpdateBasketAsync(basket);
        }


        /// <summary>
        /// Xóa sản phẩm khỏi giỏ hàng
        /// </summary>
        //public async Task RemoveItemFromBasketAsync(string userId, Guid productId)
        //{
        //    var basket = await _basketRepository.GetBasketAsync(userId);
        //    if (basket == null)
        //        throw new InvalidOperationException("Basket not found!");
        //    var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
        //    if (item != null)
        //    {
        //        basket.Items.Remove(item);
        //        await _basketRepository.UpdateBasketAsync(basket);
        //    }
        //}

        /// <summary>
        /// Xóa toàn bộ giỏ hàng
        /// </summary>
        public async Task DeleteBasketAsync(string userId)
        {
            await _basketRepository.DeleteBasketAsync(userId);
        }
    }
}
