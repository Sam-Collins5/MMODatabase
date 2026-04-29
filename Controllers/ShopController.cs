using Microsoft.AspNetCore.Mvc;
using MMOngo.Services.Interfaces;
using MMOngo.ViewModels;

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

        public IActionResult Create(string category)
        {
            return View(_shopService.GetItemCreateForm(category));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ShopItemFormViewModel form)
        {
            if (!ModelState.IsValid)
            {
                return View(_shopService.GetItemCreateForm(form.Category));
            }

            _shopService.AddItem(form);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(string category, int id)
        {
            var form = _shopService.GetItemEditForm(category, id);

            if (form == null)
            {
                return NotFound();
            }

            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ShopItemFormViewModel form)
        {
            if (!ModelState.IsValid)
            {
                var fixedForm = _shopService.GetItemEditForm(form.Category, form.ItemId);

                if (fixedForm == null)
                {
                    return NotFound();
                }

                return View(fixedForm);
            }

            _shopService.UpdateItem(form);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(string category, int id)
        {
            var item = _shopService.GetItemForDelete(category, id);

            if (item == null)
            {
                return NotFound();
            }

            ViewBag.Category = category;
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(string category, int id)
        {
            _shopService.DeleteItem(category, id);
            return RedirectToAction(nameof(Index));
        }
    }
}