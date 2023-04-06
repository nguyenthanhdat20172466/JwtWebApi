﻿using JwtWebApi.Dtos;
using JwtWebApi.Models;

namespace JwtWebApi.Services.AuthService
{
    public interface IAuthService
    {
        Task<User> RegisterUser(UserDto request);
        Task<AuthResponseDto> Login(UserDto request);
        Task<AuthResponseDto> RefreshToken();
    }
}
