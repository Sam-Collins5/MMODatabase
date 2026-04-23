using Microsoft.AspNetCore.Mvc;
using MMOngo.Services.Interfaces;

namespace MMOngo.Controllers
{
    public class ShopController : Controller
    {
        private readonly IShopService _shopService;

        public ShopController(IShopService shopService)
        {
            _shopService = shopService;
        }

        public IActionResult Index()
        {
            return View(_shopService.GetShopData());
        }
    }
}