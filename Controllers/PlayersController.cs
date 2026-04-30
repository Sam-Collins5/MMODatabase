using Microsoft.AspNetCore.Mvc;
using MMOngo.Services.Interfaces;
using MMOngo.Models;

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

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Player());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Player player)
        {
            _playerService.AddPlayer(player);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(string username)
        {
            Player? player = _playerService.GetPlayerByUserName(username);

            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Player player)
        {
            _playerService.UpdatePlayer(player);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(string username)
        {
            Player? player = _playerService.GetPlayerByUserName(username);

            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string username)
        {
            _playerService.DeletePlayer(username);
            return RedirectToAction(nameof(Index));
        }
    }
}