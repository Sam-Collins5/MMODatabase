using Microsoft.AspNetCore.Mvc;
using MMOngo.Services.Interfaces;

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
    }
}