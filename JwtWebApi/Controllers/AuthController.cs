﻿using JwtWebApi.Dtos;
using JwtWebApi.Models;
using JwtWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

namespace JwtWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user = new User();
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        public AuthController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration; 
            _userService = userService;
        }
        [HttpGet, Authorize]
        public ActionResult<string> GetMyName()
        {
            return Ok(_userService.GetMyName());
            //var username = User?.Identity?.Name;
            //var roleClaims = User.FindAll(ClaimTypes.Role);    
            //var roles = roleClaims.Select(x => x.Value).ToList();
            //var role2 = User?.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            //return Ok(new { username, roles, role2 });
        }
        [HttpPost("register")]
        public ActionResult<User> Register(UserDto request)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            user.Username  = request.Username;
            user.PasswordHash = passwordHash;
            return Ok(user);
        }
        [HttpPost("login")]
        public ActionResult<User> Login(UserDto request)
        {
            if (user.Username != request.Username)
            {
                return BadRequest("User not found");
            }
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest("Password is invalid");
            }

            var token = CreateToken(user);
            return Ok(token);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.Role, "User")

            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!)); //Authentication:Schemes:Bearer:SigningKeys:0:Value

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

    }
}
