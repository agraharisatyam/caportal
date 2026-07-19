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
        private readonly IWebHostEnvironment _env;

        public SettingsController(SiteSettingsService settingsService, IWebHostEnvironment env)
        {
            _settingsService = settingsService;
            _env = env;
        }

        private IActionResult? Auth()
        {
            if (HttpContext.Session.GetString(SessionKey) != "true")
                return RedirectToAction("Login", "Auth", new { area = "Admin" });
            return null;
        }

        // GET /Admin/Settings
        [HttpGet]
        public IActionResult Index()
        {
            var g = Auth(); if (g != null) return g;
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            return View(_settingsService.Get());
        }

        // POST /Admin/Settings/Save
        [HttpPost]
        public IActionResult Save(SiteSettings settings)
        {
            var g = Auth(); if (g != null) return g;
            var cur = _settingsService.Get();
            if (string.IsNullOrEmpty(settings.LogoPath))      settings.LogoPath      = cur.LogoPath;
            if (string.IsNullOrEmpty(settings.LogoSmallPath)) settings.LogoSmallPath = cur.LogoSmallPath;
            _settingsService.Save(settings);
            TempData["Success"] = "Settings saved! Changes are live on the website.";
            return RedirectToAction("Index");
        }

        // POST /Admin/Settings/UploadLogo
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UploadLogo(IFormFile? logoFile, IFormFile? logoSmallFile)
        {
            var g = Auth(); if (g != null) return g;

            var settings  = _settingsService.Get();
            var webRoot   = _env.WebRootPath;
            var imgFolder = Path.Combine(webRoot, "images");
            Directory.CreateDirectory(imgFolder);

            var ok = new[] { ".png", ".jpg", ".jpeg", ".svg", ".webp" };

            if (logoFile is { Length: > 0 })
            {
                var ext  = Path.GetExtension(logoFile.FileName).ToLowerInvariant();
                if (!ok.Contains(ext)) { TempData["Error"] = $"Type '{ext}' not allowed."; return RedirectToAction("Index"); }

                var name = "site-logo" + ext;
                var path = Path.Combine(imgFolder, name);
                await using var fs = System.IO.File.Create(path);
                await logoFile.CopyToAsync(fs);

                settings.LogoPath = $"/images/{name}?t={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
                settings.LogoAlt  = settings.SiteName;
            }

            if (logoSmallFile is { Length: > 0 })
            {
                var ext  = Path.GetExtension(logoSmallFile.FileName).ToLowerInvariant();
                if (ok.Contains(ext))
                {
                    var name = "site-logo-sm" + ext;
                    var path = Path.Combine(imgFolder, name);
                    await using var fs = System.IO.File.Create(path);
                    await logoSmallFile.CopyToAsync(fs);
                    settings.LogoSmallPath = $"/images/{name}?t={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
                }
            }

            _settingsService.Save(settings);
            TempData["Success"] = $"Logo uploaded! → {settings.LogoPath}";
            return RedirectToAction("Index");
        }

        // POST /Admin/Settings/Reset
        [HttpPost]
        public IActionResult Reset()
        {
            var g = Auth(); if (g != null) return g;
            var cur = _settingsService.Get();
            // preserve logo on reset
            var fresh = new SiteSettings { LogoPath = cur.LogoPath, LogoSmallPath = cur.LogoSmallPath, LogoAlt = cur.LogoAlt };
            _settingsService.Save(fresh);
            TempData["Success"] = "Settings reset to defaults (logo kept).";
            return RedirectToAction("Index");
        }

        // GET /Admin/Settings/Preview
        [HttpGet]
        public ContentResult Preview() =>
            Content(_settingsService.GenerateCss(), "text/css");

        // GET /Admin/Settings/TestUpload
        [HttpGet]
        public IActionResult TestUpload() =>
            Content($"OK | WebRoot: {_env.WebRootPath}", "text/plain");
    }
}
