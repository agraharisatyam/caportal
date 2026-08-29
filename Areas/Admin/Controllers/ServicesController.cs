using caportal.Data;
using caportal.Filters;
using caportal.Models;
using caportal.Models.Entities;
using caportal.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace caportal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class ServicesController : Controller
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
        private readonly SiteSettingsService _settingsService;
        private readonly IWebHostEnvironment _env;

        public ServicesController(
            IDbContextFactory<ApplicationDbContext> dbFactory,
            SiteSettingsService settingsService,
            IWebHostEnvironment env)
        {
            _dbFactory       = dbFactory;
            _settingsService = settingsService;
            _env             = env;
        }

        // GET /Admin/Services
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            ViewBag.Settings = _settingsService.Get();
            ViewBag.Services = new List<CoveredService>();
            try
            {
                using var db = _dbFactory.CreateDbContext();
                db.Database.SetCommandTimeout(15);
                ViewBag.Services = await db.CoveredServices.OrderBy(s => s.DisplayOrder).ToListAsync();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Database error: " + ex.Message;
            }
            return View();
        }

        // POST /Admin/Services/SaveSection
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveSection(string servicesBadge, string servicesTitle)
        {
            var settings = _settingsService.Get();
            settings.ServicesBadge = servicesBadge ?? settings.ServicesBadge;
            settings.ServicesTitle = servicesTitle ?? settings.ServicesTitle;
            _settingsService.Save(settings);
            TempData["Success"] = "Section settings saved!";
            return RedirectToAction("Index");
        }

        // POST /Admin/Services/Save — add or update with optional image upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(
            int id, string title, string description, int displayOrder,
            string? pageUrl, IFormFile? imageFile, string? existingImagePath)
        {
            // Handle image upload
            string imagePath = existingImagePath ?? "";
            if (imageFile is { Length: > 0 })
            {
                var allowed = new[] { ".png", ".jpg", ".jpeg", ".svg", ".webp" };
                var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                if (!allowed.Contains(ext))
                {
                    TempData["Error"] = $"File type '{ext}' not allowed.";
                    return RedirectToAction("Index");
                }
                var folder = Path.Combine(_env.WebRootPath, "images", "services");
                Directory.CreateDirectory(folder);
                var fileName = $"svc-{Guid.NewGuid():N}{ext}";
                var filePath = Path.Combine(folder, fileName);
                await using var fs = System.IO.File.Create(filePath);
                await imageFile.CopyToAsync(fs);
                imagePath = $"/images/services/{fileName}";
            }

            using var db = _dbFactory.CreateDbContext();
            if (id == 0)
            {
                db.CoveredServices.Add(new CoveredService
                {
                    Title        = title,
                    Description  = description,
                    DisplayOrder = displayOrder,
                    ImagePath    = imagePath,
                    PageUrl      = string.IsNullOrWhiteSpace(pageUrl) ? "#professionals" : pageUrl,
                    Icon         = "fas fa-briefcase"
                });
                TempData["Success"] = $"\"{title}\" added!";
            }
            else
            {
                var existing = await db.CoveredServices.FindAsync(id);
                if (existing != null)
                {
                    existing.Title        = title;
                    existing.Description  = description;
                    existing.DisplayOrder = displayOrder;
                    existing.PageUrl      = string.IsNullOrWhiteSpace(pageUrl) ? existing.PageUrl : pageUrl;
                    if (!string.IsNullOrEmpty(imagePath))
                        existing.ImagePath = imagePath;
                    TempData["Success"] = $"\"{title}\" updated!";
                }
            }
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST /Admin/Services/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            using var db = _dbFactory.CreateDbContext();
            var svc = await db.CoveredServices.FindAsync(id);
            if (svc != null)
            {
                db.CoveredServices.Remove(svc);
                await db.SaveChangesAsync();
                TempData["Success"] = $"\"{svc.Title}\" deleted.";
            }
            return RedirectToAction("Index");
        }

        // GET /Admin/Services/GetJson/5  — JSON for edit modal (renamed to avoid route clash)
        [HttpGet("Admin/Services/GetJson/{id:int}")]
        public async Task<IActionResult> GetJson(int id)
        {
            using var db = _dbFactory.CreateDbContext();
            var svc = await db.CoveredServices.FindAsync(id);
            if (svc == null) return NotFound();
            return Json(new {
                svc.Id, svc.Title, svc.Description,
                svc.DisplayOrder, svc.ImagePath, svc.Icon
            });
        }

        // GET /Admin/Services/Create
        [HttpGet("Admin/Services/Create")]
        public IActionResult Create()
        {
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            return View("Create", new CoveredService { DisplayOrder = 1 });
        }

        // GET /Admin/Services/Edit/5
        [HttpGet("Admin/Services/Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            using var db = _dbFactory.CreateDbContext();
            var service = await db.CoveredServices.FindAsync(id);
            if (service == null) return RedirectToAction("Index");
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            return View("Edit", service);
        }

        // POST /Admin/Services/Edit/5
        [HttpPost("Admin/Services/Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, string title, string description, int displayOrder,
            string? pageUrl, IFormFile? imageFile, string? existingImagePath)
        {
            string imagePath = existingImagePath ?? "";
            if (imageFile is { Length: > 0 })
            {
                var allowed = new[] { ".png", ".jpg", ".jpeg", ".svg", ".webp" };
                var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                if (!allowed.Contains(ext))
                {
                    TempData["Error"] = $"File type '{ext}' not allowed.";
                    return RedirectToAction("Index");
                }
                var folder = Path.Combine(_env.WebRootPath, "images", "services");
                Directory.CreateDirectory(folder);
                var fileName = $"svc-{Guid.NewGuid():N}{ext}";
                var filePath = Path.Combine(folder, fileName);
                await using var fs = System.IO.File.Create(filePath);
                await imageFile.CopyToAsync(fs);
                imagePath = $"/images/services/{fileName}";
            }

            using var db = _dbFactory.CreateDbContext();
            var existing = await db.CoveredServices.FindAsync(id);
            if (existing != null)
            {
                existing.Title        = title;
                existing.Description  = description;
                existing.DisplayOrder = displayOrder;
                existing.PageUrl      = string.IsNullOrWhiteSpace(pageUrl) ? existing.PageUrl : pageUrl;
                if (!string.IsNullOrEmpty(imagePath))
                    existing.ImagePath = imagePath;
                await db.SaveChangesAsync();
                TempData["Success"] = $"\"{title}\" updated!";
            }
            return RedirectToAction("Index");
        }
    }
}
