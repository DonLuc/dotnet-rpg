using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Models;
using dotnet_rpg.Data;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private static List<Character> characters = new List<Character> {
            new Character(),
            new Character {
                Id = 1,
                Name = "Black Panther"
            }
        };
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public CharacterService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public static List<Character> Characters { get => characters; set => characters = value; }

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            ServiceResponse<List<GetCharacterDto>> serviceReponse = new ServiceResponse<List<GetCharacterDto>>();
            Character character = _mapper.Map<Character>(newCharacter);
            await _context.Characters.AddAsync(character);
            await _context.SaveChangesAsync();
            serviceReponse.Data = (_context.Characters.Select(c => _mapper.Map<GetCharacterDto>(c))).ToList();
            return serviceReponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            ServiceResponse<List<GetCharacterDto>> serviceReponse = new ServiceResponse<List<GetCharacterDto>>();
            try {
                Character character = Characters.First(c => c.Id == id);
                Characters.Remove(character);
                serviceReponse.Data = (Characters.Select(c => _mapper.Map<GetCharacterDto>(c))).ToList();    
            } catch(Exception ex) {
                serviceReponse.Success = false;
                serviceReponse.Message = ex.Message;
            }
            return serviceReponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            ServiceResponse<List<GetCharacterDto>> serviceReponse = new ServiceResponse<List<GetCharacterDto>>();
            List<Character> dbCharacters = await _context.Characters.ToListAsync();
            serviceReponse.Data = (dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c))).ToList();
            return serviceReponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            ServiceResponse<GetCharacterDto> serviceReponse = new ServiceResponse<GetCharacterDto>();
            Character dbCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
            serviceReponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
            return serviceReponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            ServiceResponse<GetCharacterDto> serviceResponse = new ServiceResponse<GetCharacterDto>();
            try {
            Character character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);
            character.Name = updatedCharacter.Name;
            character.Class = updatedCharacter.Class;
            character.Defense = updatedCharacter.Defense;
            character.HitPoints = updatedCharacter.HitPoints;
            character.Intelligence = updatedCharacter.Intelligence;
            character.Strength = updatedCharacter.Strength;

            _context.Characters.Update(character);
            await _context.SaveChangesAsync();


            serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
            } catch(Exception ex) {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;

        }
    }
}