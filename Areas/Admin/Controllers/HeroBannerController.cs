using caportal.Data;
using caportal.Filters;
using caportal.Models.Entities;
using caportal.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace caportal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class HeroBannerController : Controller
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
        private readonly SiteSettingsService _settingsService;
        private readonly IWebHostEnvironment _env;

        public HeroBannerController(
            IDbContextFactory<ApplicationDbContext> dbFactory,
            SiteSettingsService settingsService,
            IWebHostEnvironment env)
        {
            _dbFactory = dbFactory;
            _settingsService = settingsService;
            _env = env;
        }

        private async Task EnsureSlideTableExists(ApplicationDbContext db)
        {
            try
            {
                var sql = @"
                    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'HeroBannerSlides')
                    BEGIN
                        CREATE TABLE [HeroBannerSlides] (
                            [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                            [Title] NVARCHAR(200) NOT NULL DEFAULT '',
                            [Subtitle] NVARCHAR(500) NULL,
                            [Badge] NVARCHAR(100) NULL,
                            [ImagePath] NVARCHAR(500) NOT NULL DEFAULT '/images/hero-banner.png',
                            [MobileImagePath] NVARCHAR(500) NULL,
                            [LinkUrl] NVARCHAR(500) NULL,
                            [ButtonText] NVARCHAR(100) NULL,
                            [ButtonUrl] NVARCHAR(500) NULL,
                            [SecondaryButtonText] NVARCHAR(100) NULL,
                            [SecondaryButtonUrl] NVARCHAR(500) NULL,
                            [DisplayOrder] INT NOT NULL DEFAULT 0,
                            [IsActive] BIT NOT NULL DEFAULT 1,
                            [SlideType] NVARCHAR(50) NOT NULL DEFAULT 'image',
                            [BgGradientFrom] NVARCHAR(50) NULL DEFAULT '#0a1628',
                            [BgGradientTo] NVARCHAR(50) NULL DEFAULT '#1a2a4a',
                            [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
                        );
                    END";
                await db.Database.ExecuteSqlRawAsync(sql);
            }
            catch
            {
                // ignored
            }
        }

        // GET: /Admin/HeroBanner
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            var settings = _settingsService.Get();
            ViewBag.Settings = settings;

            using var db = _dbFactory.CreateDbContext();
            await EnsureSlideTableExists(db);

            var slides = await db.HeroBannerSlides.OrderBy(s => s.DisplayOrder).ToListAsync();
            if (!slides.Any())
            {
                var defaultSlide = new HeroBannerSlide
                {
                    Title = settings.HeroTitle ?? "CA & Legal Compliance Platform",
                    Subtitle = settings.HeroSubtitle ?? "Connecting businesses with ICAI-verified Chartered Accountants across India.",
                    Badge = settings.HeroBadge ?? "⭐ India's #1 Verified CA Network",
                    ImagePath = !string.IsNullOrEmpty(settings.HeroBannerImage) ? settings.HeroBannerImage : "/images/hero-banner.png",
                    MobileImagePath = settings.HeroBannerMobileImage,
                    LinkUrl = settings.HeroBannerLink,
                    ButtonText = settings.HeroPrimaryCtaText ?? "Find a CA",
                    ButtonUrl = settings.HeroPrimaryCtaUrl ?? "/find-expert",
                    SecondaryButtonText = settings.HeroSecondaryCtaText ?? "Explore Services",
                    SecondaryButtonUrl = settings.HeroSecondaryCtaUrl ?? "/#features",
                    DisplayOrder = 1,
                    IsActive = true,
                    SlideType = settings.HeroMode ?? "image"
                };
                db.HeroBannerSlides.Add(defaultSlide);
                await db.SaveChangesAsync();
                slides = new List<HeroBannerSlide> { defaultSlide };
            }

            return View(slides);
        }

        // POST: /Admin/HeroBanner/SaveSlide
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSlide(
            int id,
            string title,
            string? subtitle,
            string? badge,
            string slideType,
            string? linkUrl,
            string? buttonText,
            string? buttonUrl,
            string? secondaryButtonText,
            string? secondaryButtonUrl,
            int displayOrder,
            bool isActive,
            string? bgGradientFrom,
            string? bgGradientTo,
            string? existingImagePath,
            string? existingMobileImagePath,
            IFormFile? imageFile,
            IFormFile? mobileImageFile)
        {
            using var db = _dbFactory.CreateDbContext();
            await EnsureSlideTableExists(db);

            string imagePath = existingImagePath ?? "/images/hero-banner.png";
            if (imageFile is { Length: > 0 })
            {
                var uploaded = await SaveUploadedFile(imageFile, "hero-slide");
                if (!string.IsNullOrEmpty(uploaded)) imagePath = uploaded;
            }

            string? mobileImagePath = existingMobileImagePath;
            if (mobileImageFile is { Length: > 0 })
            {
                var uploaded = await SaveUploadedFile(mobileImageFile, "hero-slide-mobile");
                if (!string.IsNullOrEmpty(uploaded)) mobileImagePath = uploaded;
            }

            if (id == 0)
            {
                var slide = new HeroBannerSlide
                {
                    Title = title ?? "Banner Slide",
                    Subtitle = subtitle,
                    Badge = badge,
                    SlideType = !string.IsNullOrEmpty(slideType) ? slideType : "image",
                    ImagePath = imagePath,
                    MobileImagePath = mobileImagePath,
                    LinkUrl = linkUrl,
                    ButtonText = buttonText,
                    ButtonUrl = buttonUrl,
                    SecondaryButtonText = secondaryButtonText,
                    SecondaryButtonUrl = secondaryButtonUrl,
                    DisplayOrder = displayOrder,
                    IsActive = isActive,
                    BgGradientFrom = bgGradientFrom ?? "#0a1628",
                    BgGradientTo = bgGradientTo ?? "#1a2a4a",
                    CreatedAt = DateTime.UtcNow
                };
                db.HeroBannerSlides.Add(slide);
                TempData["Success"] = $"Banner \"{slide.Title}\" added successfully!";
            }
            else
            {
                var slide = await db.HeroBannerSlides.FindAsync(id);
                if (slide != null)
                {
                    slide.Title = title ?? slide.Title;
                    slide.Subtitle = subtitle;
                    slide.Badge = badge;
                    slide.SlideType = !string.IsNullOrEmpty(slideType) ? slideType : slide.SlideType;
                    slide.ImagePath = imagePath;
                    if (!string.IsNullOrEmpty(mobileImagePath)) slide.MobileImagePath = mobileImagePath;
                    slide.LinkUrl = linkUrl;
                    slide.ButtonText = buttonText;
                    slide.ButtonUrl = buttonUrl;
                    slide.SecondaryButtonText = secondaryButtonText;
                    slide.SecondaryButtonUrl = secondaryButtonUrl;
                    slide.DisplayOrder = displayOrder;
                    slide.IsActive = isActive;
                    slide.BgGradientFrom = bgGradientFrom ?? slide.BgGradientFrom;
                    slide.BgGradientTo = bgGradientTo ?? slide.BgGradientTo;

                    TempData["Success"] = $"Banner \"{slide.Title}\" updated successfully!";
                }
            }

            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: /Admin/HeroBanner/DeleteSlide
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSlide(int id)
        {
            using var db = _dbFactory.CreateDbContext();
            var slide = await db.HeroBannerSlides.FindAsync(id);
            if (slide != null)
            {
                db.HeroBannerSlides.Remove(slide);
                await db.SaveChangesAsync();
                TempData["Success"] = "Banner deleted successfully.";
            }
            return RedirectToAction("Index");
        }

        // POST: /Admin/HeroBanner/ToggleSlide
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleSlide(int id)
        {
            using var db = _dbFactory.CreateDbContext();
            var slide = await db.HeroBannerSlides.FindAsync(id);
            if (slide != null)
            {
                slide.IsActive = !slide.IsActive;
                await db.SaveChangesAsync();
                return Json(new { success = true, isActive = slide.IsActive });
            }
            return Json(new { success = false, message = "Slide not found" });
        }

        // POST: /Admin/HeroBanner/Reorder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reorder([FromBody] List<int> slideIds)
        {
            if (slideIds == null || !slideIds.Any()) return BadRequest();

            using var db = _dbFactory.CreateDbContext();
            for (int i = 0; i < slideIds.Count; i++)
            {
                var id = slideIds[i];
                var slide = await db.HeroBannerSlides.FindAsync(id);
                if (slide != null)
                {
                    slide.DisplayOrder = i + 1;
                }
            }
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

        // GET: /Admin/HeroBanner/GetSlideJson/5
        [HttpGet("Admin/HeroBanner/GetSlideJson/{id:int}")]
        public async Task<IActionResult> GetSlideJson(int id)
        {
            using var db = _dbFactory.CreateDbContext();
            var slide = await db.HeroBannerSlides.FindAsync(id);
            if (slide == null) return NotFound();

            return Json(new
            {
                slide.Id,
                slide.Title,
                slide.Subtitle,
                slide.Badge,
                slide.SlideType,
                slide.ImagePath,
                slide.MobileImagePath,
                slide.LinkUrl,
                slide.ButtonText,
                slide.ButtonUrl,
                slide.SecondaryButtonText,
                slide.SecondaryButtonUrl,
                slide.DisplayOrder,
                slide.IsActive,
                slide.BgGradientFrom,
                slide.BgGradientTo
            });
        }

        private async Task<string> SaveUploadedFile(IFormFile file, string filePrefix)
        {
            var allowedExts = new[] { ".png", ".jpg", ".jpeg", ".svg", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExts.Contains(ext)) return string.Empty;

            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "banners");
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }

            var fileName = $"{filePrefix}_{DateTime.UtcNow.Ticks}{ext}";
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/banners/{fileName}";
        }
    }
}
