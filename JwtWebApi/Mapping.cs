using AutoMapper;
using JwtWebApi.Dtos;
using JwtWebApi.Models;

namespace JwtWebApi
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CharacterUpdateDto, Character>();

        }
    }
}
