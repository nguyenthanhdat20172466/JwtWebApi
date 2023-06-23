using Azure;
using Azure.Core;
using JwtWebApi.Data;
using JwtWebApi.Dtos;
using JwtWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using JwtWebApi.Services;


namespace JwtWebApi.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _context;
        //private readonly DataContext _contextRefreshToken;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthService(DataContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ITokenService tokenService)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
        }
        public async Task<AuthResponseDto> Login(UserDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user.Username != request.Username)
            {
                return new AuthResponseDto { Message = "User not found." };
            }
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new AuthResponseDto { Message = "Wrong Password." };
            }

            var token = _tokenService.CreateToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            _tokenService.SetRefreshToken(refreshToken, user);
            return new AuthResponseDto
            {
                Success = true,
                Token = token,
                RefreshToken = refreshToken.Token,
                TokenExpires = refreshToken.Expires,
                Role = user.Role
            };
        }

        public async Task<AuthResponseDto> RegisterUser(UserDto request)
        {
            var userExist = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if(userExist != null)
            {
                return new AuthResponseDto { Message = "User is exist." };
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Success = true,
            }; 

        }
        public async Task<AuthResponseDto> EditRole(string name)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == name);
            if (user == null)
            {
                return new AuthResponseDto { Message = "User not found." };
            }
            var tokenString = _httpContextAccessor?.HttpContext?.Request.Headers["Authorization"].FirstOrDefault(); 
            var existingRoles = _tokenService.GetRoles(tokenString.Split(' ')[1]);

            existingRoles.Add("Admin");
            var newToken = _tokenService.CreateNewToken(existingRoles);
            user.Role = _tokenService.GetRoles(newToken).FirstOrDefault(role => role == "Admin");
            await _context.SaveChangesAsync();

            var refreshToken = _tokenService.GenerateRefreshToken(newToken);
            _tokenService.SetRefreshToken(refreshToken, user);
            return new AuthResponseDto
            {
                Success = true,
                Token = newToken,
                RefreshToken = refreshToken.Token,
                TokenExpires = refreshToken.Expires,
                Role = user.Role
            };
        }
    }
}
