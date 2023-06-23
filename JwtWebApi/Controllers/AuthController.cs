﻿using JwtWebApi.Data;
using JwtWebApi.Dtos;
using JwtWebApi.Models;
using JwtWebApi.Services;
using JwtWebApi.Services.AuthService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JwtWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        //public static User user = new User();
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly DataContext _context;
        private readonly IAuthService _authService;
        public AuthController(IConfiguration configuration, IUserService userService, DataContext context, IAuthService authService)
        {
            _configuration = configuration;
            _userService = userService;
            _context = context;
            _authService = authService;
        }

        [HttpGet, Authorize]
        public ActionResult<string> GetMe()
        {
            var userName = _userService.GetMyName();
            return Ok(userName);
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterUser(UserDto request)
        {
            var response = await _authService.RegisterUser(request);
            if (response.Success)
                return Ok(response);

            return BadRequest(response.Message);

        }

        [HttpPost("login")]
        public async  Task<ActionResult<User>> Login(UserDto request)
        {
            var response = await _authService.Login(request);
            if (response.Success)
                return Ok(response);

            return BadRequest(response.Message);
        }
        [HttpPost("EditRole")]
        [Authorize]
        public async Task<ActionResult<User>> EditRole()
        {
            var userName = _userService.GetMyName();
            var response = await _authService.EditRole(userName);
            if (response.Success)
                return Ok(response);

            return BadRequest(response.Message);

        }

        [HttpGet("Role"), Authorize(Roles = "Admin")]
        public ActionResult<string> Aloha()
        {
            return Ok("Aloha! You're authorized!");
        }
        [HttpGet("RoleUser"), Authorize(Roles = "Admin, User")]
        public ActionResult<string> GetRole()
        {
            return Ok("Role");
        }

    }
}
