using caportal.Models;
using caportal.Services;
using Microsoft.AspNetCore.Mvc;

namespace caportal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SettingsController : Controller
    {
        private const string SessionKey = "AdminLoggedIn";
        private readonly SiteSettingsService _settingsService;

        public SettingsController(SiteSettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        private IActionResult? RequireAuth()
        {
            if (HttpContext.Session.GetString(SessionKey) != "true")
                return RedirectToAction("Login", "Auth", new { area = "Admin" });
            return null;
        }

        // GET /Admin/Settings
        public IActionResult Index()
        {
            var guard = RequireAuth();
            if (guard != null) return guard;

            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            var settings = _settingsService.Get();
            return View(settings);
        }

        // POST /Admin/Settings
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(SiteSettings settings)
        {
            var guard = RequireAuth();
            if (guard != null) return guard;

            _settingsService.Save(settings);
            TempData["Success"] = "Settings saved successfully! Changes are live on the website.";
            return RedirectToAction(nameof(Index));
        }

        // GET /Admin/Settings/Preview — returns generated CSS for live preview
        [HttpGet]
        public ContentResult Preview()
        {
            return Content(_settingsService.GenerateCss(), "text/css");
        }

        // POST /Admin/Settings/Reset
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reset()
        {
            var guard = RequireAuth();
            if (guard != null) return guard;

            _settingsService.Save(new SiteSettings());
            TempData["Success"] = "Settings reset to defaults.";
            return RedirectToAction(nameof(Index));
        }
    }
}
