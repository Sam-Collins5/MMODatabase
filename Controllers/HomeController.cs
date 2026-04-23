using Microsoft.AspNetCore.Mvc;
using MMOngo.Services.Interfaces;

namespace MMOngo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        public IActionResult Index()
        {
            return View(_homeService.GetHomeData());
        }
    }
}