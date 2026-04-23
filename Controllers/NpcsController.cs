using Microsoft.AspNetCore.Mvc;
using MMOngo.Services.Interfaces;

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
    }
}