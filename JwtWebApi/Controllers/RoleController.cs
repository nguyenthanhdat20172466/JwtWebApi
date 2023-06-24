using JwtWebApi.Data;
using JwtWebApi.Services.AuthService;
using JwtWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using JwtWebApi.Models;
using System.Data;

namespace JwtWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class RoleController : ControllerBase
    {

        private readonly DataContext _context;
        public RoleController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllRole()
        {
            var role = _context.Roles.ToList();
            return Ok(role);
;       }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<RoleUser> AddUser(string role)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var roles = new RoleUser
            {
                Role = role
            };
            _context.Roles.Add(roles);
            _context.SaveChanges();

            return Ok(role);
        }
    }
}
