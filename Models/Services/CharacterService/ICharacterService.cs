using System.Collections.Generic;
using System.Threading.Tasks;

namespace dotnet_rpg.Models.Services.CharacterService
{
    public interface ICharacterService
    {
         Task<ServiceResponse<List<Character>>> GetAllCharacters();
         Task<ServiceResponse<List<Character>>> AddCharacter(Character newCharacter);
         Task<ServiceResponse<Character>> GetCharacterById(int id);
    }
}