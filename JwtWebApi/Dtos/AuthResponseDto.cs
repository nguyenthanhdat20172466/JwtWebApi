﻿using JwtWebApi.Models;

namespace JwtWebApi.Dtos
{
    public class AuthResponseDto
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenExpires { get; set; }
        //public string Role { get; set; } = string.Empty;
        public User User { get; set; }
    }
}
