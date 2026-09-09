using caportal.Data;
using caportal.Filters;
using caportal.Models;
using caportal.Models.Entities;
using caportal.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
                EnsureSubServiceImagePath(db);
                ViewBag.Services = await db.CoveredServices.OrderBy(s => s.DisplayOrder).ToListAsync();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Database error: " + ex.Message;
            }
            return View();
        }

        private static bool _subImgColChecked = false;
        private static readonly object _subImgLock = new();
        private static void EnsureSubServiceImagePath(ApplicationDbContext db)
        {
            if (_subImgColChecked) return;
            lock (_subImgLock)
            {
                if (_subImgColChecked) return;
                try
                {
                    db.Database.ExecuteSqlRaw(@"
                        IF NOT EXISTS (
                            SELECT 1 FROM sys.columns 
                            WHERE object_id = OBJECT_ID(N'SubServices') AND name = N'ImagePath'
                        )
                        BEGIN
                            ALTER TABLE [SubServices] ADD [ImagePath] nvarchar(max) NOT NULL DEFAULT '';
                        END");
                    _subImgColChecked = true;
                }
                catch { _subImgColChecked = true; }
            }
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

        // POST /Admin/Services/Save — add or update with optional image upload & sub-services
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(
            int id, string title, string description, int displayOrder,
            string? pageUrl, IFormFile? imageFile, string? existingImagePath,
            string? subServicesJson)
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
                var newSvc = new CoveredService
                {
                    Title        = title,
                    Description  = description,
                    DisplayOrder = displayOrder,
                    ImagePath    = imagePath,
                    PageUrl      = string.IsNullOrWhiteSpace(pageUrl) ? "#professionals" : pageUrl,
                    Icon         = "fas fa-briefcase"
                };
                db.CoveredServices.Add(newSvc);
                await db.SaveChangesAsync();

                var subDtos = ParseSubServices(subServicesJson);
                if (subDtos.Any())
                {
                    var subEntities = subDtos
                        .Where(d => !string.IsNullOrWhiteSpace(d.Title))
                        .Select(d => new SubService
                        {
                            CoveredServiceId = newSvc.Id,
                            Title = d.Title.Trim(),
                            Description = d.Description?.Trim() ?? "",
                            Price = d.Price?.Trim() ?? "",
                            Category = d.Category?.Trim() ?? "",
                            Icon = d.Icon?.Trim() ?? "",
                            IsPopular = d.IsPopular,
                            IsNew = d.IsNew,
                            DisplayOrder = d.DisplayOrder,
                            IsActive = d.IsActive
                        });
                    db.SubServices.AddRange(subEntities);
                    await db.SaveChangesAsync();
                }

                TempData["Success"] = $"\"{title}\" added successfully!";
            }
            else
            {
                var existing = await db.CoveredServices
                    .Include(s => s.SubServices)
                    .FirstOrDefaultAsync(s => s.Id == id);
                if (existing != null)
                {
                    existing.Title        = title;
                    existing.Description  = description;
                    existing.DisplayOrder = displayOrder;
                    existing.PageUrl      = string.IsNullOrWhiteSpace(pageUrl) ? existing.PageUrl : pageUrl;
                    if (!string.IsNullOrEmpty(imagePath))
                        existing.ImagePath = imagePath;

                    if (subServicesJson != null)
                    {
                        SyncSubServices(db, existing, ParseSubServices(subServicesJson));
                    }

                    await db.SaveChangesAsync();
                    TempData["Success"] = $"\"{title}\" updated!";
                }
            }
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
            using var db = _dbFactory.CreateDbContext();
            EnsureSubServiceImagePath(db);
            return View("Create", new CoveredService { DisplayOrder = 1 });
        }

        // GET /Admin/Services/Edit/5
        [HttpGet("Admin/Services/Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            using var db = _dbFactory.CreateDbContext();
            EnsureSubServiceImagePath(db);
            var service = await db.CoveredServices
                .Include(s => s.SubServices)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (service == null) return RedirectToAction("Index");
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            return View("Edit", service);
        }

        // POST /Admin/Services/Edit/5
        [HttpPost("Admin/Services/Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, string title, string description, int displayOrder,
            string? pageUrl, IFormFile? imageFile, string? existingImagePath,
            string? subServicesJson)
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
            var existing = await db.CoveredServices
                .Include(s => s.SubServices)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (existing != null)
            {
                existing.Title        = title;
                existing.Description  = description;
                existing.DisplayOrder = displayOrder;
                existing.PageUrl      = string.IsNullOrWhiteSpace(pageUrl) ? existing.PageUrl : pageUrl;
                if (!string.IsNullOrEmpty(imagePath))
                    existing.ImagePath = imagePath;

                if (subServicesJson != null)
                {
                    SyncSubServices(db, existing, ParseSubServices(subServicesJson));
                }

                await db.SaveChangesAsync();
                TempData["Success"] = $"\"{title}\" updated!";
            }
            return RedirectToAction("Index");
        }

        private static List<SubServiceDto> ParseSubServices(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new();
            try
            {
                return JsonSerializer.Deserialize<List<SubServiceDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new();
            }
            catch
            {
                return new();
            }
        }

        private static void SyncSubServices(ApplicationDbContext db, CoveredService existing, List<SubServiceDto> subDtos)
        {
            var submittedValidDtos = subDtos.Where(d => !string.IsNullOrWhiteSpace(d.Title)).ToList();
            var submittedIds = submittedValidDtos.Where(d => d.Id > 0).Select(d => d.Id).ToHashSet();

            // Remove subservices deleted from the table
            var toRemove = existing.SubServices.Where(s => !submittedIds.Contains(s.Id)).ToList();
            if (toRemove.Any())
            {
                db.SubServices.RemoveRange(toRemove);
            }

            // Update existing or add new
            foreach (var dto in submittedValidDtos)
            {
                if (dto.Id > 0)
                {
                    var current = existing.SubServices.FirstOrDefault(s => s.Id == dto.Id);
                    if (current != null)
                    {
                        current.Title = dto.Title.Trim();
                        current.Description = dto.Description?.Trim() ?? "";
                        current.Price = dto.Price?.Trim() ?? "";
                        current.Category = dto.Category?.Trim() ?? "";
                        current.Icon = dto.Icon?.Trim() ?? "";
                        if (!string.IsNullOrEmpty(dto.ImagePath)) current.ImagePath = dto.ImagePath;
                        current.IsPopular = dto.IsPopular;
                        current.IsNew = dto.IsNew;
                        current.DisplayOrder = dto.DisplayOrder;
                        current.IsActive = dto.IsActive;
                    }
                }
                else
                {
                    var newSub = new SubService
                    {
                        CoveredServiceId = existing.Id,
                        Title = dto.Title.Trim(),
                        Description = dto.Description?.Trim() ?? "",
                        Price = dto.Price?.Trim() ?? "",
                        Category = dto.Category?.Trim() ?? "",
                        Icon = dto.Icon?.Trim() ?? "",
                        IsPopular = dto.IsPopular,
                        IsNew = dto.IsNew,
                        DisplayOrder = dto.DisplayOrder,
                        IsActive = dto.IsActive
                    };
                    existing.SubServices.Add(newSub);
                    db.SubServices.Add(newSub);
                }
            }
        }

        // POST /Admin/Services/QuickSaveSubService (Immediate AJAX save from popup modal)
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> QuickSaveSubService([FromBody] QuickSubServiceRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Title) || req.CoveredServiceId <= 0)
            {
                return Json(new { success = false, message = "Valid Title and Service ID are required." });
            }

            try
            {
                using var db = _dbFactory.CreateDbContext();
                if (req.Id > 0)
                {
                    var existing = await db.SubServices.FirstOrDefaultAsync(s => s.Id == req.Id && s.CoveredServiceId == req.CoveredServiceId);
                    if (existing != null)
                    {
                        existing.Title = req.Title.Trim();
                        existing.Description = req.Description?.Trim() ?? "";
                        existing.Price = req.Price?.Trim() ?? "";
                        existing.Category = req.Category?.Trim() ?? "";
                        existing.Icon = req.Icon?.Trim() ?? "";
                        existing.IsPopular = req.IsPopular;
                        existing.IsNew = req.IsNew;
                        existing.DisplayOrder = req.DisplayOrder;
                        existing.IsActive = req.IsActive;
                        await db.SaveChangesAsync();
                        return Json(new { success = true, id = existing.Id, message = "Sub-service updated successfully." });
                    }
                }

                var newSub = new SubService
                {
                    CoveredServiceId = req.CoveredServiceId,
                    Title = req.Title.Trim(),
                    Description = req.Description?.Trim() ?? "",
                    Price = req.Price?.Trim() ?? "",
                    Category = req.Category?.Trim() ?? "",
                    Icon = req.Icon?.Trim() ?? "",
                    IsPopular = req.IsPopular,
                    IsNew = req.IsNew,
                    DisplayOrder = req.DisplayOrder,
                    IsActive = req.IsActive
                };
                db.SubServices.Add(newSub);
                await db.SaveChangesAsync();
                return Json(new { success = true, id = newSub.Id, message = "Sub-service saved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error saving sub-service: " + ex.Message });
            }
        }

        // POST /Admin/Services/UploadSubServiceImage
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UploadSubServiceImage(IFormFile file, int subServiceId)
        {
            if (file == null || file.Length == 0)
                return Json(new { success = false, message = "No file provided." });

            var allowed = new[] { ".png", ".jpg", ".jpeg", ".svg", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext))
                return Json(new { success = false, message = $"Type '{ext}' not allowed." });

            var folder = Path.Combine(_env.WebRootPath, "images", "subservices");
            Directory.CreateDirectory(folder);
            var fileName = $"sub-{Guid.NewGuid():N}{ext}";
            var filePath = Path.Combine(folder, fileName);
            await using var fs = System.IO.File.Create(filePath);
            await file.CopyToAsync(fs);
            var imagePath = $"/images/subservices/{fileName}";

            if (subServiceId > 0)
            {
                using var db = _dbFactory.CreateDbContext();
                var sub = await db.SubServices.FindAsync(subServiceId);
                if (sub != null)
                {
                    sub.ImagePath = imagePath;
                    await db.SaveChangesAsync();
                }
            }

            return Json(new { success = true, imagePath });
        }

        // POST /Admin/Services/QuickDeleteSubService (Immediate AJAX delete)
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> QuickDeleteSubService([FromBody] QuickDeleteSubServiceRequest req)
        {
            if (req == null || req.Id <= 0)
            {
                return Json(new { success = false, message = "Invalid Sub-Service ID." });
            }

            try
            {
                using var db = _dbFactory.CreateDbContext();
                var sub = await db.SubServices.FindAsync(req.Id);
                if (sub != null)
                {
                    db.SubServices.Remove(sub);
                    await db.SaveChangesAsync();
                }
                return Json(new { success = true, message = "Sub-service deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    public class QuickSubServiceRequest
    {
        public int Id { get; set; }
        public int CoveredServiceId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public bool IsPopular { get; set; }
        public bool IsNew { get; set; }
        public int DisplayOrder { get; set; } = 1;
        public bool IsActive { get; set; } = true;
    }

    public class QuickDeleteSubServiceRequest
    {
        public int Id { get; set; }
    }

    public class SubServiceDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public bool IsPopular { get; set; } = false;
        public bool IsNew { get; set; } = false;
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
