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
                return View(_characterService.GetCharacterCreateForm());
            }

            _characterService.AddCharacter(form);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var form = _characterService.GetCharacterEditForm(id);

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
                var fixedForm = _characterService.GetCharacterEditForm(form.CharacterId);

                if (fixedForm == null)
                {
                    return NotFound();
                }

                return View(fixedForm);
            }

            _characterService.UpdateCharacter(form);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var character = _characterService.GetCharacterById(id);

            if (character == null)
            {
                return NotFound();
            }

            return View(character);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int characterId)
        {
            _characterService.DeleteCharacter(characterId);
            return RedirectToAction(nameof(Index));
        }
    }
}