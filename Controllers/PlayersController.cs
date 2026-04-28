using Microsoft.AspNetCore.Mvc;
using MMOngo.Services.Interfaces;

namespace MMOngo.Controllers
{
    public class PlayersController : Controller
    {
        private readonly IPlayerService _playerService;

        public PlayersController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        public IActionResult Index()
        {
            return View(_playerService.GetAllPlayers());
        }

        public IActionResult Details(string username)
        {
            var vm = _playerService.GetPlayerDetails(username);

            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }
    }
}