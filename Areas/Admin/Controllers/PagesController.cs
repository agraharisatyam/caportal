using caportal.Filters;
using Microsoft.AspNetCore.Mvc;

namespace caportal.Areas.Admin.Controllers
{
    public class ContentPageItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Slug { get; set; } = "";
        public string MetaDescription { get; set; } = "";
        public string HtmlContent { get; set; } = "";
        public bool IsPublished { get; set; } = true;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    [Area("Admin")]
    [AdminAuthorize]
    public class PagesController : Controller
    {
        private static readonly List<ContentPageItem> _pages = new()
        {
            new ContentPageItem { Id=1, Title="About Us", Slug="about-us", MetaDescription="Learn about CACampus — India's premier platform connecting businesses with ICAI-verified Chartered Accountants.", IsPublished=true, LastUpdated=DateTime.Now.AddDays(-5), HtmlContent="<h2>About CACampus</h2><p>CACampus combines technology and expertise to deliver reliable CA, legal, and compliance solutions across India.</p>" },
            new ContentPageItem { Id=2, Title="Terms & Conditions", Slug="terms-and-conditions", MetaDescription="Terms and conditions governing the use of CACampus platform and professional engagement.", IsPublished=true, LastUpdated=DateTime.Now.AddDays(-12), HtmlContent="<h2>Terms & Conditions</h2><p>Welcome to CACampus. By using our website and services, you agree to comply with our terms.</p>" },
            new ContentPageItem { Id=3, Title="Privacy Policy", Slug="privacy-policy", MetaDescription="CACampus privacy policy detailing how client and professional data is collected, stored, and protected.", IsPublished=true, LastUpdated=DateTime.Now.AddDays(-20), HtmlContent="<h2>Privacy Policy</h2><p>We respect your privacy and process all client and financial information with strict confidentiality.</p>" },
            new ContentPageItem { Id=4, Title="Contact Us", Slug="contact-us", MetaDescription="Get in touch with CACampus support and client advisory team.", IsPublished=true, LastUpdated=DateTime.Now.AddDays(-2), HtmlContent="<h2>Contact Us</h2><p>Have questions? Reach out to our dedicated support team via phone or email.</p>" }
        };

        // GET /Admin/Pages
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            return View(_pages);
        }

        // GET /Admin/Pages/Edit/1
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            var page = _pages.FirstOrDefault(p => p.Id == id);
            if (page == null) return RedirectToAction("Index");
            return View(page);
        }

        // POST /Admin/Pages/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, string title, string slug, string metaDescription, string htmlContent, bool isPublished)
        {
            var page = _pages.FirstOrDefault(p => p.Id == id);
            if (page != null)
            {
                page.Title = title;
                page.Slug = slug;
                page.MetaDescription = metaDescription;
                page.HtmlContent = htmlContent;
                page.IsPublished = isPublished;
                page.LastUpdated = DateTime.Now;
                TempData["Success"] = $"Page '{title}' updated successfully!";
            }
            return RedirectToAction("Index");
        }
    }
}
