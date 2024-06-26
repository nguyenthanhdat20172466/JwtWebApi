﻿using CloudinaryDotNet.Actions;
using JwtWebApi.Data;
using JwtWebApi.Dtos;
using JwtWebApi.Models;
using JwtWebApi.Services.PhotoService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Numerics;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;
using AutoMapper;

namespace JwtWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly JsonSerializerSettings _options;
        private readonly IMapper _mapper;
        public CharacterController(DataContext dataContext, IMapper mapper )
        {
            _context = dataContext;
            _mapper = mapper;
            _options = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }


        [HttpPost("character")]
        public async Task<Character> AddCharacter(CharacterDto character)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            var name = _context.Characters.FirstOrDefault(x => x.Name.ToLower() == character.Name.ToLower());
            if(name != null)
            {
                throw new Exception("character is exist");
                //return BadRequest($"character is exist");
            }
            var newCharacter = new Character
            {
                Name = character.Name.ToLower(),
                Lane = character.Lane,
                Description= character.Description,
            };
           // newCharacter.Counters = _context.Counters.Where(x => x.CharacterId == newCharacter.Id).ToList();
            _context.Characters.Add(newCharacter);
            _context.SaveChangesAsync();

            return newCharacter;
        }

        [HttpGet("character")]
        public async Task<List<Character>> GetAllCharacter()
        {
            var character = await _context.Characters.Include(c => c.Counters).ToListAsync();
            //if(!character.Any())
            //{
            //    return BadRequest(ModelState);
            //}
            var json = JsonConvert.SerializeObject(character, _options);
            var deserializedcharacters = JsonConvert.DeserializeObject<List<Character>>(json);

            return deserializedcharacters;
        }

        [HttpPost("characterCounter")]
        public async Task<Counter> AddCharacterCounter(string characterCounter, string characterName)
        {
            var name = _context.Characters.Include(c => c.Counters).FirstOrDefault(x => x.Name.ToLower() == characterName.ToLower());
            if(name == null)
            {
                throw new Exception("tướng này chưa có");
                //return BadRequest("tướng này chưa có");
            }
           // var nameCounter = _context.Counters.FirstOrDefault(x => x.Name.ToLower() == characterCounter.ToLower());
            if (name.Counters.Any(c => c.Name.ToLower() == characterCounter.ToLower()))
            {
                throw new Exception("Tướng Counter đã có trong danh sách của Character");
                //return BadRequest("Tướng Counter đã có trong danh sách của Character");
            }
            if (characterCounter.ToLower() == characterName.ToLower())
            {
                throw new Exception("Tướng này không thể khắc chế chính nó");
                //return BadRequest("Tướng này không thể khắc chế chính nó");

            }
            //name.Counters = _context.Counters.Where(x => x.CharacterId == name.Id).ToList();
            var newCharacterCouter = new Counter 
            { 
                Name = characterCounter,
                CharacterId = name.Id,
                Character = name
            };

            _context.Counters.Add(newCharacterCouter);
            name.Counters.Add(newCharacterCouter);

            _context.SaveChangesAsync();

            var json = JsonConvert.SerializeObject(newCharacterCouter, _options);
            var deserializedCharacterCounter = JsonConvert.DeserializeObject<Counter>(json);

            return deserializedCharacterCounter;
        }
        [HttpPost("update")]
        public async Task<ActionResult<Character>> Update(CharacterUpdateDto model)
        {
            var entity = _mapper.Map<CharacterUpdateDto,Character>(model);
            var character = _context.Characters.FirstOrDefault(x => x.Id == model.Id);

            List<string> modifyParams = new List<string>() { "Name", "Description"};
            if (entity.ModifyParams is null) entity.ModifyParams = modifyParams;
            else entity.ModifyParams.AddRange(modifyParams);
            PartialConvert(character, entity);

            _context.Characters.Update(character);
            _context.SaveChangesAsync();

            return Ok(character);

        }

        public static  void PartialConvert(Character source, Character update)
        {
            if (source == null || update == null)
            {
                return;
            }
            var properties = source.GetType().GetProperties();
            var modifyFields = update.ModifyParams;

            foreach (var item in properties)
            {
                if (modifyFields != null && modifyFields.Contains(item.Name, StringComparer.CurrentCultureIgnoreCase))
                {
                    var value = item.GetValue(update);
                    item.SetValue(source, value);
                }
            }
        }


    }
}
