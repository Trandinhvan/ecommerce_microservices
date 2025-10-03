using BasketService.Application.DTOs;
using BasketService.Application.Interfaces;
using BasketService.Application.UseCase;
using BasketService.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BasketService.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketUseCase _basketUseCase;

        public BasketController(IBasketUseCase basketUseCase) => _basketUseCase = basketUseCase;

        //[HttpGet("{userId}")]
        //public async Task<IActionResult> GetBasket(string userId)
        //{
        //    var basket = await _basketUseCase.GetBasketAsync(userId);
        //    if (basket == null) return NotFound();
        //    return Ok(new BasketDto(basket.UserId, basket.Items.Select(i => new BasketItemDto(i.ProductId, i.ProductName, i.Price, i.Quantity)).ToList()));
        //}

        [HttpGet]
        [Authorize] // bảo vệ route
        public async Task<IActionResult> GetBasket()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var basket = await _basketUseCase.GetBasketAsync(userId);
                if (basket == null) return NotFound();

                // phòng null Items
                var items = basket.Items?.Select(i => new BasketItemDto(
                    i.ProductId, i.ProductName, i.Price, i.Quantity
                )).ToList() ?? new List<BasketItemDto>();

                return Ok(new BasketDto(items)); // không còn UserId nữa
            }
            catch (Exception ex)
            {
                // TODO: log ex chi tiết (Serilog, ILogger...)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddItemToBasket([FromBody] BasketItemDto itemDto)
        {
            if (itemDto == null)
                return BadRequest("Invalid item data");

            // Lấy userId từ token
            var userId = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var item = new BasketItem
            {
                ProductId = itemDto.ProductId,
                ProductName = itemDto.ProductName,
                Price = itemDto.Price,
                Quantity = itemDto.Quantity
            };

            await _basketUseCase.AddItemToBasketAsync(userId, item);
            return Ok();
        }

        [HttpPut("{productId}")]
        [Authorize]
        public async Task<IActionResult> UpdateItemQuantity(Guid productId, [FromBody] UpdateItemDto dto)
        {
            if (dto == null || dto.Quantity < 1) return BadRequest("Invalid quantity");

            var userId = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            await _basketUseCase.UpdateToBasketAsync(userId, productId, dto.Quantity);
            return Ok();
        }

        //[HttpDelete("{userId}")]
        //public async Task<IActionResult> DeleteBasket(string userId)
        //{
        //    await _basketUseCase.DeleteBasketAsync(userId);
        //    return NoContent();
        //}
        [HttpDelete("{productId}")]
        [Authorize]
        public async Task<IActionResult> RemoveItem(Guid productId)
        {
            var userId = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            await _basketUseCase.RemoveItemAsync(userId, productId);
            return Ok();
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> RemoveAll()
        {
            var userId = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            await _basketUseCase.DeleteBasketAsync(userId);
            return Ok(new { message = "Giỏ hàng đã được xóa" });
        }
    }
}
