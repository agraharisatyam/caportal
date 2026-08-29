using caportal.Filters;
using Microsoft.AspNetCore.Mvc;

namespace caportal.Areas.Admin.Controllers
{
    public class MediaFileItem
    {
        public string FileName { get; set; } = "";
        public string RelativeUrl { get; set; } = "";
        public string Folder { get; set; } = "";
        public long SizeBytes { get; set; }
        public DateTime LastModified { get; set; }
    }

    [Area("Admin")]
    [AdminAuthorize]
    public class MediaController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public MediaController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // GET /Admin/Media
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";

            var imgFolder = Path.Combine(_env.WebRootPath, "images");
            var items = new List<MediaFileItem>();

            if (Directory.Exists(imgFolder))
            {
                var files = Directory.GetFiles(imgFolder, "*.*", SearchOption.AllDirectories);
                foreach (var f in files)
                {
                    var ext = Path.GetExtension(f).ToLowerInvariant();
                    if (new[] { ".png", ".jpg", ".jpeg", ".svg", ".webp", ".gif" }.Contains(ext))
                    {
                        var relative = f.Replace(_env.WebRootPath, "").Replace("\\", "/");
                        var fi = new FileInfo(f);
                        items.Add(new MediaFileItem
                        {
                            FileName = fi.Name,
                            RelativeUrl = relative,
                            Folder = Path.GetDirectoryName(relative)?.Replace("\\", "/") ?? "/images",
                            SizeBytes = fi.Length,
                            LastModified = fi.LastWriteTime
                        });
                    }
                }
            }

            return View(items.OrderByDescending(x => x.LastModified).ToList());
        }

        // POST /Admin/Media/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file is { Length: > 0 })
            {
                var allowed = new[] { ".png", ".jpg", ".jpeg", ".svg", ".webp", ".gif" };
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowed.Contains(ext))
                {
                    TempData["Error"] = $"File extension '{ext}' is not allowed.";
                    return RedirectToAction("Index");
                }

                var imgFolder = Path.Combine(_env.WebRootPath, "images");
                Directory.CreateDirectory(imgFolder);

                var fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}-{Guid.NewGuid():N}{ext}";
                var filePath = Path.Combine(imgFolder, fileName);

                await using var fs = System.IO.File.Create(filePath);
                await file.CopyToAsync(fs);

                TempData["Success"] = $"File '{file.FileName}' uploaded successfully! URL: /images/{fileName}";
            }

            return RedirectToAction("Index");
        }

        // POST /Admin/Media/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string relativeUrl)
        {
            if (!string.IsNullOrEmpty(relativeUrl))
            {
                var safePath = relativeUrl.TrimStart('/', '\\').Replace("/", "\\");
                var fullPath = Path.Combine(_env.WebRootPath, safePath);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                    TempData["Success"] = "Media asset deleted.";
                }
            }
            return RedirectToAction("Index");
        }
    }
}
