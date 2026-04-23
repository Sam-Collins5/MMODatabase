using Microsoft.AspNetCore.Mvc;
using MMOngo.Services.Interfaces;

namespace MMOngo.Controllers
{
    public class MissionsController : Controller
    {
        private readonly IMissionService _missionService;

        public MissionsController(IMissionService missionService)
        {
            _missionService = missionService;
        }

        public IActionResult Index()
        {
            return View(_missionService.GetAllMissions());
        }

        public IActionResult Details(string name)
        {
            var mission = _missionService.GetMissionByName(name);

            if (mission == null)
            {
                return NotFound();
            }

            return View(mission);
        }
    }
}