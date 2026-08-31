using caportal.Filters;
using caportal.Models.Entities;
using caportal.Services.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace caportal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class MenuController : Controller
    {
        // GET /Admin/Menu
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            var items = MenuRepository.GetAll();
            return View(items);
        }

        // GET /Admin/Menu/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            var maxOrder = MenuRepository.GetAll().Count > 0 ? MenuRepository.GetAll().Max(m => m.Order) : 0;
            var newItem = new NavbarMenuItem { Order = maxOrder + 1, IsActive = true };
            return View(newItem);
        }

        // POST /Admin/Menu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(NavbarMenuItem item)
        {
            if (item != null)
            {
                MenuRepository.Add(item);
                TempData["Success"] = "Menu item added successfully!";
            }
            return RedirectToAction("Index");
        }

        // GET /Admin/Menu/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            var item = MenuRepository.GetById(id);
            if (item == null)
            {
                TempData["Error"] = "Menu item not found.";
                return RedirectToAction("Index");
            }
            return View(item);
        }

        // POST /Admin/Menu/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(NavbarMenuItem item)
        {
            if (item != null && item.Id > 0)
            {
                MenuRepository.Update(item);
                TempData["Success"] = "Menu item updated successfully!";
            }
            return RedirectToAction("Index");
        }

        // POST /Admin/Menu/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            MenuRepository.Delete(id);
            TempData["Success"] = "Menu item deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
