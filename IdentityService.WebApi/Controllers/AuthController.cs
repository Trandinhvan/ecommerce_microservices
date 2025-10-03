using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces;
using IdentityService.Application.UseCase;
using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IdentityService.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthUseCase _authUseCase;

        public AuthController(IAuthUseCase authUseCase)
        {
            _authUseCase = authUseCase;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                var result = await _authUseCase.RegisterAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var result = await _authUseCase.LoginAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyInfo()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var user = await _authUseCase.GetUserInforAsync(username);
            if (user == null) return NotFound();

            return Ok(new
            {
                user.UserName,
                user.Email,
                user.Role
            });
        }
    }
}
