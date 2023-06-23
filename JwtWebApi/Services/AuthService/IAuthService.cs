using JwtWebApi.Dtos;
using JwtWebApi.Models;

namespace JwtWebApi.Services.AuthService
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterUser(UserDto request);
        Task<AuthResponseDto> Login(UserDto request);
        Task<AuthResponseDto> EditRole(string name);
        //Task<AuthResponseDto> RefreshToken();
    }
}
