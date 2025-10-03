using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace IdentityService.Application.UseCase
{
    public class AuthUseCase : IAuthUseCase
    {
        private readonly IUserRepository _users;
        private readonly IJwtService _jwt;

        public AuthUseCase(IUserRepository users, IJwtService jwt)
        {
            _users = users;
            _jwt = jwt;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var existing = await _users.GetByUsernameAsync(request.Username);
            if (existing != null)
                throw new Exception("Username already exists");

            var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(request.Password)));

            var user = new User
            {
                UserName = request.Username,
                Email = request.Email,
                PasswordHash = hash,
                Role = "User" // mặc định
            };

            await _users.AddAsync(user);

            // Trả token luôn sau khi đăng ký
            var token = _jwt.GenerateAccessToken(user.UserName, user.Role);
            var refreshToken = _jwt.GenerateRefreshToken();

            return new AuthResponse(token, refreshToken);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _users.GetByUsernameAsync(request.Username);
            if (user == null)
                throw new Exception("Invalid credentials");

            var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(request.Password)));

            if (user.PasswordHash != hash)
                throw new Exception("Invalid credentials");

            var token = _jwt.GenerateAccessToken(user.UserName, user.Role);
            var refreshToken = _jwt.GenerateRefreshToken();

            return new AuthResponse(token, refreshToken);
        }

        public async Task<User?> GetUserInforAsync(string userName)
        {
            return await _users.GetByUsernameAsync(userName);
        }
    }
}
