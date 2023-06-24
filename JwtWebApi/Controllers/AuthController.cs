using JwtWebApi.Data;
using JwtWebApi.Dtos;
using JwtWebApi.Models;
using JwtWebApi.Services.AuthService;
using JwtWebApi.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<User>> EditRole(string userName, string role)
        {
            //var userName = _userService.GetMyName();
            var response = await _authService.EditRole(userName, role);
            if (response.Success)
                return Ok(response);

            return BadRequest(response.Message);

        }

        [HttpGet("GetAllUser")]
        public async  Task<IActionResult> GetAll()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }
    }
}
