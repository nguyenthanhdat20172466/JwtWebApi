using JwtWebApi.Dtos;
using JwtWebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace JwtWebApi.Services.CharacterService
{
    public interface ICharacterService
    {
        Task<ActionResult<Character>> AddCharacter(CharacterDto character);
        Task<ActionResult<IEnumerable<Character>>> GetAllCharacter();
        Task<ActionResult<Counter>> AddCharacterCounter(string characterCounter, string characterName);

    }
}
