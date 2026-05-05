using Microsoft.AspNetCore.Mvc;
using MMOngo.Services.Interfaces;

namespace MMOngo.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public IActionResult Index()
        {
            return View(_dashboardService.GetDashboardData());
        }
    }
}
