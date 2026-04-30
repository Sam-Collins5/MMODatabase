using Microsoft.AspNetCore.Mvc;
using MMOngo.Services.Interfaces;
using MMOngo.ViewModels;

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

        public IActionResult Create()
        {
            return View(_missionService.GetMissionCreateForm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MissionFormViewModel form)
        {
            if (!ModelState.IsValid)
            {
                return View(_missionService.GetMissionCreateForm());
            }

            _missionService.AddMission(form);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(string name)
        {
            var form = _missionService.GetMissionEditForm(name);

            if (form == null)
            {
                return NotFound();
            }

            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(MissionFormViewModel form)
        {
            if (!ModelState.IsValid)
            {
                var fixedForm = _missionService.GetMissionEditForm(form.OriginalMissionName);

                if (fixedForm == null)
                {
                    return NotFound();
                }

                return View(fixedForm);
            }

            _missionService.UpdateMission(form);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(string name)
        {
            var mission = _missionService.GetMissionByName(name);

            if (mission == null)
            {
                return NotFound();
            }

            return View(mission);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(string name)
        {
            _missionService.DeleteMission(name);
            return RedirectToAction(nameof(Index));
        }
    }
}