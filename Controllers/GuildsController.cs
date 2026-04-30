using Microsoft.AspNetCore.Mvc;
using MMOngo.Services.Interfaces;
using MMOngo.ViewModels;

namespace MMOngo.Controllers
{
    public class GuildsController : Controller
    {
        private readonly IGuildService _guildService;

        public GuildsController(IGuildService guildService)
        {
            _guildService = guildService;
        }

        public IActionResult Index()
        {
            return View(_guildService.GetAllGuilds());
        }

        public IActionResult Details(string name)
        {
            var vm = _guildService.GetGuildDetails(name);

            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        public IActionResult Create()
        {
            return View(_guildService.GetGuildCreateForm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(GuildFormViewModel form)
        {
            if (!ModelState.IsValid)
            {
                return View(_guildService.GetGuildCreateForm());
            }

            _guildService.AddGuild(form);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(string name)
        {
            var form = _guildService.GetGuildEditForm(name);

            if (form == null)
            {
                return NotFound();
            }

            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(GuildFormViewModel form)
        {
            if (!ModelState.IsValid)
            {
                var fixedForm = _guildService.GetGuildEditForm(form.OriginalGuildName);

                if (fixedForm == null)
                {
                    return NotFound();
                }

                return View(fixedForm);
            }

            _guildService.UpdateGuild(form);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(string name)
        {
            var guild = _guildService.GetGuildByName(name);

            if (guild == null)
            {
                return NotFound();
            }

            return View(guild);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(string name)
        {
            _guildService.DeleteGuild(name);
            return RedirectToAction(nameof(Index));
        }
    }
}