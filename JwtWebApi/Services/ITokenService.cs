using JwtWebApi.Models;

namespace JwtWebApi.Services
{
    public interface ITokenService
    {
        RefreshToken GenerateRefreshToken(string token = null);
        void SetRefreshToken(RefreshToken newRefreshToken, User user);
        string CreateToken(User user);
        List<string> GetRoles(string token);
        string CreateNewToken(List<string> roles);






    }
}
