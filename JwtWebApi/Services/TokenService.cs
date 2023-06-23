using JwtWebApi.Data;
using JwtWebApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JwtWebApi.Services
{
    public class TokenService: ITokenService
    {
        private readonly DataContext _contextRefreshToken;

        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TokenService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, DataContext contextRefreshToken)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _contextRefreshToken = contextRefreshToken;
        }

        public RefreshToken GenerateRefreshToken(string token = null)
        {
            var refreshToken = new RefreshToken
            {
                Token = token == null ? Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)) : token,
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };
            return refreshToken;
        }

        public async void SetRefreshToken(RefreshToken newRefreshToken, User user)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires,
            };
            _httpContextAccessor?.HttpContext?.Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            user.RefreshToken = newRefreshToken.Token;
            user.TokenCreated = newRefreshToken.Created;
            user.TokenExpires = newRefreshToken.Expires;
            _contextRefreshToken.Users.Update(user);
            await _contextRefreshToken.SaveChangesAsync();
        }


        public string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                //new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.Role, user.Role)

            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!)); //Authentication:Schemes:Bearer:SigningKeys:0:Value

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public List<string> GetRoles(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var roles = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

            return roles;
        }


        public string CreateNewToken(List<string> roles)
        {
            var claims = new List<Claim>();
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

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
