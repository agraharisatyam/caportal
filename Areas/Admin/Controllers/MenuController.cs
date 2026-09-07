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

        // POST /Admin/Menu/ToggleActive
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleActive(int id)
        {
            var item = MenuRepository.GetById(id);
            if (item != null)
            {
                item.IsActive = !item.IsActive;
                MenuRepository.Update(item);
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, isActive = item.IsActive });
                }
            }
            return RedirectToAction("Index");
        }

        // ── Child Dropdown Items Endpoints ──

        // POST /Admin/Menu/AddDropdownItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddDropdownItem(int menuId, NavbarDropdownItem item)
        {
            if (string.IsNullOrWhiteSpace(item.Title) || string.IsNullOrWhiteSpace(item.Url))
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Title and URL are required." });
                }
                TempData["Error"] = "Title and URL are required.";
                return RedirectToAction("Edit", new { id = menuId });
            }

            var created = MenuRepository.AddDropdownItem(menuId, item);
            if (created != null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, item = created, message = "Dropdown link added successfully!" });
                }
                TempData["Success"] = "Dropdown link added successfully!";
            }
            else
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Failed to add dropdown item. Menu not found." });
                }
                TempData["Error"] = "Failed to add dropdown item.";
            }

            return RedirectToAction("Edit", new { id = menuId });
        }

        // POST /Admin/Menu/UpdateDropdownItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateDropdownItem(int menuId, NavbarDropdownItem item)
        {
            var ok = MenuRepository.UpdateDropdownItem(menuId, item);
            if (ok)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Dropdown link updated successfully!" });
                }
                TempData["Success"] = "Dropdown link updated successfully!";
            }
            else
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Dropdown item not found." });
                }
                TempData["Error"] = "Failed to update dropdown link.";
            }

            return RedirectToAction("Edit", new { id = menuId });
        }

        // POST /Admin/Menu/DeleteDropdownItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteDropdownItem(int menuId, int itemId)
        {
            var ok = MenuRepository.DeleteDropdownItem(menuId, itemId);
            if (ok)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Dropdown link deleted!" });
                }
                TempData["Success"] = "Dropdown link deleted successfully!";
            }
            else
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Failed to delete item." });
                }
                TempData["Error"] = "Failed to delete dropdown item.";
            }

            return RedirectToAction("Edit", new { id = menuId });
        }
    }
}
