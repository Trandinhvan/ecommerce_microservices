//using Microsoft.AspNetCore.Mvc;
//using OrderingService.Application.DTOs;
//using OrderingService.Application.Interfaces;
//using OrderingService.Domain.Entities;

//namespace OrderingService.WebApi.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class OrdersController : ControllerBase
//    {
//        private readonly IOrderRepository _repo;

//        public OrdersController(IOrderRepository repo) => _repo = repo;

//        [HttpPost]
//        public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
//        {
//            var order = new Order
//            {
//                Id = Guid.NewGuid(),
//                UserId = request.UserId,
//                Items = request.Items.Select(i => new OrderItem
//                {
//                    ProductId = i.ProductId,
//                    ProductName = i.ProductName,
//                    Price = i.Price,
//                    Quantity = i.Quantity
//                }).ToList(),
//                TotalPrice = request.Items.Sum(i => i.Price * i.Quantity),
//                Status = OrderStatus.Pending
//            };

//            await _repo.AddAsync(order);
//            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
//        }

//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetOrderById(Guid id)
//        {
//            var order = await _repo.GetByIdAsync(id);
//            if (order == null) return NotFound();

//            return Ok(new OrderDto(order.Id, order.UserId,
//                order.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.Price, i.Quantity)).ToList(),
//                order.TotalPrice, order.Status.ToString()));
//        }

//        [HttpGet("user/{userId}")]
//        public async Task<IActionResult> GetOrdersByUser(string userId)
//        {
//            var orders = await _repo.GetByUserAsync(userId);
//            var result = orders.Select(o => new OrderDto(o.Id, o.UserId,
//                o.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.Price, i.Quantity)).ToList(),
//                o.TotalPrice, o.Status.ToString()));

//            return Ok(result);
//        }
//    }
//}


using Microsoft.AspNetCore.Mvc;
using OrderingService.Application.Interfaces;
using Contracts.Events;
using OrderingService.Application.DTOs;
using OrderingService.Application.UseCase;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace OrderingService.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderUseCase _orderUseCase;

        public OrdersController(IOrderUseCase orderUseCase)
        {
            _orderUseCase = orderUseCase;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var orderId = await _orderUseCase.ExecuteAsync(
                userId,
                request.Items.Select(i => (i.ProductId, i.ProductName, i.Price, i.Quantity))
            );

            return Ok(new { OrderId = orderId });
        }

    }
}
