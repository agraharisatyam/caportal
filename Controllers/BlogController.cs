using caportal.Services;
using caportal.Services.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace caportal.Controllers
{
    public class BlogController : Controller
    {
        private readonly SiteSettingsService _settingsService;

        public BlogController(SiteSettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        // GET /blog
        [HttpGet("/blog")]
        public IActionResult Index(string? category, string? search)
        {
            ViewBag.Settings = _settingsService.Get();

            var allPosts = BlogRepository.GetAll(includeUnpublished: false);

            if (!string.IsNullOrEmpty(category) && !category.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                allPosts = allPosts.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(search))
            {
                allPosts = allPosts.Where(p => 
                    p.Title.Contains(search, StringComparison.OrdinalIgnoreCase) || 
                    p.Excerpt.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.Tags.Any(t => t.Contains(search, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            ViewBag.SelectedCategory = category ?? "All";
            ViewBag.SearchQuery = search ?? "";
            ViewBag.Categories = new[] { "All", "GST & Tax", "Income Tax", "Corporate Law", "Startup Advisory" };

            // Top featured post (first post)
            ViewBag.FeaturedPost = allPosts.FirstOrDefault();
            ViewBag.GridPosts = allPosts.Skip(1).ToList();

            return View(allPosts);
        }

        // GET /blog/{slug}
        [HttpGet("/blog/{slug}")]
        public IActionResult Detail(string slug)
        {
            ViewBag.Settings = _settingsService.Get();

            var post = BlogRepository.GetBySlug(slug);
            if (post == null)
            {
                return RedirectToAction("Index");
            }

            // Related posts in same category
            ViewBag.RelatedPosts = BlogRepository.GetAll()
                .Where(p => p.Id != post.Id && p.Category.Equals(post.Category, StringComparison.OrdinalIgnoreCase))
                .Take(3)
                .ToList();

            return View(post);
        }
    }
}
