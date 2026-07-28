using caportal.Data;
using caportal.Models;
using caportal.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace caportal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class WhyChooseUsController : Controller
    {
        private const string SessionKey = "AdminLoggedIn";
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
        private readonly SiteSettingsService _settingsService;
        private readonly IWebHostEnvironment _env;

        public WhyChooseUsController(
            IDbContextFactory<ApplicationDbContext> dbFactory,
            SiteSettingsService settingsService,
            IWebHostEnvironment env)
        {
            _dbFactory       = dbFactory;
            _settingsService = settingsService;
            _env             = env;
        }

        private IActionResult? Auth()
        {
            if (HttpContext.Session.GetString(SessionKey) != "true")
                return RedirectToAction("Login", "Auth", new { area = "Admin" });
            return null;
        }

        // GET /Admin/WhyChooseUs
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var g = Auth(); if (g != null) return g;
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            ViewBag.Settings = _settingsService.Get();
            ViewBag.Items = new List<WhyChooseUsItem>();
            try
            {
                using var db = _dbFactory.CreateDbContext();
                db.Database.SetCommandTimeout(15);
                ViewBag.Items = await db.WhyChooseUsItems.OrderBy(s => s.DisplayOrder).ToListAsync();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Database error: " + ex.Message;
            }
            return View();
        }

        // POST /Admin/WhyChooseUs/SaveSection
        [HttpPost]
        public IActionResult SaveSection(
            string whyChooseUsBadge, string whyChooseUsTitle, string whyChooseUsSub,
            string whyChooseUsStatsTitle,
            string whyChooseUsStat1Val, string whyChooseUsStat1Lbl,
            string whyChooseUsStat2Val, string whyChooseUsStat2Lbl,
            string whyChooseUsStat3Val, string whyChooseUsStat3Lbl,
            string whyChooseUsStat4Val, string whyChooseUsStat4Lbl)
        {
            var g = Auth(); if (g != null) return g;
            var settings = _settingsService.Get();
            settings.WhyChooseUsBadge      = whyChooseUsBadge ?? settings.WhyChooseUsBadge;
            settings.WhyChooseUsTitle      = whyChooseUsTitle ?? settings.WhyChooseUsTitle;
            settings.WhyChooseUsSub        = whyChooseUsSub ?? settings.WhyChooseUsSub;
            settings.WhyChooseUsStatsTitle = whyChooseUsStatsTitle ?? settings.WhyChooseUsStatsTitle;
            settings.WhyChooseUsStat1Val   = whyChooseUsStat1Val ?? settings.WhyChooseUsStat1Val;
            settings.WhyChooseUsStat1Lbl   = whyChooseUsStat1Lbl ?? settings.WhyChooseUsStat1Lbl;
            settings.WhyChooseUsStat2Val   = whyChooseUsStat2Val ?? settings.WhyChooseUsStat2Val;
            settings.WhyChooseUsStat2Lbl   = whyChooseUsStat2Lbl ?? settings.WhyChooseUsStat2Lbl;
            settings.WhyChooseUsStat3Val   = whyChooseUsStat3Val ?? settings.WhyChooseUsStat3Val;
            settings.WhyChooseUsStat3Lbl   = whyChooseUsStat3Lbl ?? settings.WhyChooseUsStat3Lbl;
            settings.WhyChooseUsStat4Val   = whyChooseUsStat4Val ?? settings.WhyChooseUsStat4Val;
            settings.WhyChooseUsStat4Lbl   = whyChooseUsStat4Lbl ?? settings.WhyChooseUsStat4Lbl;
            
            _settingsService.Save(settings);
            TempData["Success"] = "Section settings saved!";
            return RedirectToAction("Index");
        }

        // POST /Admin/WhyChooseUs/Save
        [HttpPost]
        public async Task<IActionResult> Save(
            int id, string title, string description, int displayOrder, string icon,
            IFormFile? imageFile, string? existingImagePath)
        {
            var g = Auth(); if (g != null) return g;

            // Handle icon upload if any
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
                var folder = Path.Combine(_env.WebRootPath, "images", "whychooseus");
                Directory.CreateDirectory(folder);
                var fileName = $"wcu-{Guid.NewGuid():N}{ext}";
                var filePath = Path.Combine(folder, fileName);
                await using var fs = System.IO.File.Create(filePath);
                await imageFile.CopyToAsync(fs);
                imagePath = $"/images/whychooseus/{fileName}";
            }

            using var db = _dbFactory.CreateDbContext();
            if (id == 0)
            {
                db.WhyChooseUsItems.Add(new WhyChooseUsItem
                {
                    Title        = title,
                    Description  = description,
                    DisplayOrder = displayOrder,
                    ImagePath    = imagePath,
                    Icon         = icon ?? "fas fa-star"
                });
                TempData["Success"] = $"\"{title}\" added!";
            }
            else
            {
                var existing = await db.WhyChooseUsItems.FindAsync(id);
                if (existing != null)
                {
                    existing.Title        = title;
                    existing.Description  = description;
                    existing.DisplayOrder = displayOrder;
                    existing.Icon         = icon ?? "fas fa-star";
                    if (!string.IsNullOrEmpty(imagePath))
                        existing.ImagePath = imagePath;
                    TempData["Success"] = $"\"{title}\" updated!";
                }
            }
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST /Admin/WhyChooseUs/Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var g = Auth(); if (g != null) return g;
            using var db = _dbFactory.CreateDbContext();
            var item = await db.WhyChooseUsItems.FindAsync(id);
            if (item != null)
            {
                db.WhyChooseUsItems.Remove(item);
                await db.SaveChangesAsync();
                TempData["Success"] = $"\"{item.Title}\" deleted.";
            }
            return RedirectToAction("Index");
        }

        // GET /Admin/WhyChooseUs/GetJson/5
        [HttpGet("Admin/WhyChooseUs/GetJson/{id:int}")]
        public async Task<IActionResult> GetJson(int id)
        {
            using var db = _dbFactory.CreateDbContext();
            var item = await db.WhyChooseUsItems.FindAsync(id);
            if (item == null) return NotFound();
            return Json(new {
                item.Id, item.Title, item.Description,
                item.DisplayOrder, item.ImagePath, item.Icon
            });
        }
    }
}
