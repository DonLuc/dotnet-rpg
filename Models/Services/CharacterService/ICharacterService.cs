using System.Collections.Generic;

namespace dotnet_rpg.Models.Services.CharacterService
{
    public interface ICharacterService
    {
         List<Character> GetAllCharacters();
         List<Character> AddCharacter(Character newCharacter);
         Character GetCharacterById(int id);
    }
}