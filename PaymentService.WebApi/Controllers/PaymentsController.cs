using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.DTOs;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entities;

namespace PaymentService.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentUseCase _useCase;

        public PaymentsController(IPaymentUseCase useCase)
        {
            _useCase = useCase;
        }

        //[HttpPost]
        //public async Task<IActionResult> Pay([FromBody] CreatePaymentRequest request)
        //{
        //    var result = await _useCase.HandlePaymentAsync(request);
        //    return Ok(result);
        //}
    }
}
