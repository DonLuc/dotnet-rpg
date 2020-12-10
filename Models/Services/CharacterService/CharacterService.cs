using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.Models.Services.CharacterService
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
        public async Task<ServiceResponse<List<Character>>> AddCharacter(Character newCharacter)
        {
            ServiceResponse<List<Character>> serviceReponse = new ServiceResponse<List<Character>>();
            characters.Add(newCharacter);
            serviceReponse.Data = characters;
            return serviceReponse;
        }

        public async Task<ServiceResponse<List<Character>>> GetAllCharacters()
        {
            ServiceResponse<List<Character>> serviceReponse = new ServiceResponse<List<Character>>();
            serviceReponse.Data = characters;
            return serviceReponse;
        }

        public async Task<ServiceResponse<Character>> GetCharacterById(int id)
        {
            ServiceResponse<Character> serviceReponse = new ServiceResponse<Character>();
            serviceReponse.Data = characters.FirstOrDefault(c => c.Id == id);
            return serviceReponse;
        }
    }
}