using caportal.Filters;
using caportal.Models;
using caportal.Models.Entities;
using caportal.Services;
using Microsoft.AspNetCore.Mvc;

namespace caportal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class SettingsController : Controller
    {
        private readonly SiteSettingsService _settingsService;
        private readonly IWebHostEnvironment _env;

        public SettingsController(SiteSettingsService settingsService, IWebHostEnvironment env)
        {
            _settingsService = settingsService;
            _env = env;
        }

        // GET /Admin/Settings/ManageSections
        [HttpGet]
        public IActionResult ManageSections()
        {
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            return View(_settingsService.Get());
        }

        // POST /Admin/Settings/SaveSections
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveSections(
            bool ShowHeader, bool ShowHeroSection, bool ShowAboutUsSection,
            bool ShowServicesSection, bool ShowFeaturedCAsSection, bool ShowTestimonialsSection,
            bool ShowBlogSection, bool ShowCtaSection, bool ShowFooter,
            string? SectionOrder)
        {
            var settings = _settingsService.Get();
            settings.ShowHeader             = ShowHeader;
            settings.ShowHeroSection        = ShowHeroSection;
            settings.ShowAboutUsSection     = ShowAboutUsSection;
            settings.ShowServicesSection    = ShowServicesSection;
            settings.ShowFeaturedCAsSection = ShowFeaturedCAsSection;
            settings.ShowTestimonialsSection = ShowTestimonialsSection;
            settings.ShowBlogSection        = ShowBlogSection;
            settings.ShowCtaSection         = ShowCtaSection;
            settings.ShowFooter             = ShowFooter;
            if (!string.IsNullOrEmpty(SectionOrder))
                settings.SectionOrder = SectionOrder;
            _settingsService.Save(settings);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true, message = "Section settings saved!" });

            TempData["Success"] = "Section settings saved successfully!";
            return RedirectToAction("ManageSections");
        }

        // GET /Admin/Settings (Index)
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            return View(_settingsService.Get());
        }

        // POST /Admin/Settings/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(SiteSettings settings)
        {
            var cur = _settingsService.Get();
            if (string.IsNullOrEmpty(settings.LogoPath))      settings.LogoPath      = cur.LogoPath;
            if (string.IsNullOrEmpty(settings.LogoSmallPath)) settings.LogoSmallPath = cur.LogoSmallPath;
            _settingsService.Save(settings);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, message = "Settings saved! Changes are live on the website." });
            }

            TempData["Success"] = "Settings saved! Changes are live on the website.";
            return RedirectToAction("Index");
        }

        // POST /Admin/Settings/UploadLogo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadLogo(IFormFile? logoFile, IFormFile? logoSmallFile)
        {
            var settings  = _settingsService.Get();
            var webRoot   = _env.WebRootPath;
            var imgFolder = Path.Combine(webRoot, "images");
            Directory.CreateDirectory(imgFolder);

            var ok = new[] { ".png", ".jpg", ".jpeg", ".svg", ".webp" };

            if (logoFile is { Length: > 0 })
            {
                var ext  = Path.GetExtension(logoFile.FileName).ToLowerInvariant();
                if (!ok.Contains(ext)) {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return Json(new { success = false, message = $"Type '{ext}' not allowed." });
                    TempData["Error"] = $"Type '{ext}' not allowed.";
                    return RedirectToAction("Index");
                }

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

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, message = "Logo uploaded successfully!", logoPath = settings.LogoPath, logoSmallPath = settings.LogoSmallPath });
            }

            TempData["Success"] = $"Logo uploaded! → {settings.LogoPath}";
            return RedirectToAction("Index");
        }

        // POST /Admin/Settings/Reset
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reset()
        {
            var cur = _settingsService.Get();
            // preserve logo on reset
            var fresh = new SiteSettings { LogoPath = cur.LogoPath, LogoSmallPath = cur.LogoSmallPath, LogoAlt = cur.LogoAlt };
            _settingsService.Save(fresh);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, message = "Settings reset to defaults." });
            }

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
