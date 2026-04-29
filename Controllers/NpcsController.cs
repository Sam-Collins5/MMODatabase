using Microsoft.AspNetCore.Mvc;
using MMOngo.Services.Interfaces;
using MMOngo.ViewModels;

namespace MMOngo.Controllers
{
    public class NpcsController : Controller
    {
        private readonly INpcService _npcService;

        public NpcsController(INpcService npcService)
        {
            _npcService = npcService;
        }

        public IActionResult Index()
        {
            return View(_npcService.GetAllNpcs());
        }

        public IActionResult Details(string name)
        {
            var npc = _npcService.GetNpcByName(name);

            if (npc == null)
            {
                return NotFound();
            }

            return View(npc);
        }

        public IActionResult Create()
        {
            return View(_npcService.GetNpcCreateForm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(NpcFormViewModel form)
        {
            if (!ModelState.IsValid)
            {
                return View(_npcService.GetNpcCreateForm());
            }

            _npcService.AddNpc(form);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(string name)
        {
            var form = _npcService.GetNpcEditForm(name);

            if (form == null)
            {
                return NotFound();
            }

            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(NpcFormViewModel form)
        {
            if (!ModelState.IsValid)
            {
                var fixedForm = _npcService.GetNpcEditForm(form.OriginalNpcName);

                if (fixedForm == null)
                {
                    return NotFound();
                }

                return View(fixedForm);
            }

            _npcService.UpdateNpc(form);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(string name)
        {
            var npc = _npcService.GetNpcByName(name);

            if (npc == null)
            {
                return NotFound();
            }

            return View(npc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(string name)
        {
            _npcService.DeleteNpc(name);
            return RedirectToAction(nameof(Index));
        }
    }
}