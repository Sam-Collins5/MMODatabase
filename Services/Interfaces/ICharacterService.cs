using MMOngo.Models;
using MMOngo.ViewModels;

namespace MMOngo.Services.Interfaces
{
    public interface ICharacterService
    {
        List<PlayerCharacter> GetAllCharacters();
        PlayerCharacter? GetCharacterById(int id);
        CharacterDetailsViewModel? GetCharacterDetails(int id);
        CharacterFormViewModel GetCharacterCreateForm();
        CharacterFormViewModel? GetCharacterEditForm(int id);
        void AddCharacter(CharacterFormViewModel form);
        void UpdateCharacter(CharacterFormViewModel form);
        void DeleteCharacter(int id);
    }
}
