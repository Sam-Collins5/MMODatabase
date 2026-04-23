using Microsoft.AspNetCore.Mvc;
using MMOngo.Services.Interfaces;
using MMOngo.ViewModels;

namespace MMOngo.Controllers
{
    public class CharactersController : Controller
    {
        private readonly ICharacterService _characterService;

        public CharactersController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        public IActionResult Index()
        {
            return View(_characterService.GetAllCharacters());
        }

        public IActionResult Details(int id)
        {
            var vm = _characterService.GetCharacterDetails(id);

            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(_characterService.GetCharacterCreateForm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CharacterFormViewModel form)
        {
            if (!ModelState.IsValid)
            {
                RehydrateForm(form);
                return View(form);
            }

            _characterService.AddCharacter(form);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            CharacterFormViewModel? form = _characterService.GetCharacterEditForm(id);

            if (form == null)
            {
                return NotFound();
            }

            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CharacterFormViewModel form)
        {
            if (!ModelState.IsValid)
            {
                RehydrateForm(form);
                return View(form);
            }

            _characterService.UpdateCharacter(form);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var character = _characterService.GetCharacterById(id);

            if (character == null)
            {
                return NotFound();
            }

            return View(character);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _characterService.DeleteCharacter(id);
            return RedirectToAction(nameof(Index));
        }

        private void RehydrateForm(CharacterFormViewModel form)
        {
            CharacterFormViewModel source = form.CharacterId > 0
                ? _characterService.GetCharacterEditForm(form.CharacterId) ?? _characterService.GetCharacterCreateForm()
                : _characterService.GetCharacterCreateForm();

            form.PlayerOptions = source.PlayerOptions;
            form.AllyOptions = source.AllyOptions;
            form.WeaponOptions = source.WeaponOptions;
            form.ArmorOptions = source.ArmorOptions;
            form.ToolOptions = source.ToolOptions;
            form.CurrentMissionOptions = source.CurrentMissionOptions;
            form.CompletedMissionOptions = source.CompletedMissionOptions;
            form.SpellOptions = source.SpellOptions;
            form.GuildOptions = source.GuildOptions;
        }
    }
}
