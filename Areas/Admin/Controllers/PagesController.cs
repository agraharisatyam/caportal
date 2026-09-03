using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using caportal.Data;
using caportal.Filters;
using caportal.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace caportal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class PagesController : Controller
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public PagesController(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        // GET /Admin/Pages
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            using var db = _dbFactory.CreateDbContext();
            var pages = await db.ContentPages.OrderBy(p => p.Title).ToListAsync();
            return View(pages);
        }

        // GET /Admin/Pages/Edit/1
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            using var db = _dbFactory.CreateDbContext();
            var page = await db.ContentPages.FindAsync(id);
            if (page == null) return RedirectToAction("Index");
            return View(page);
        }

        // POST /Admin/Pages/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string title, string slug, string metaDescription, string htmlContent, bool isPublished)
        {
            using var db = _dbFactory.CreateDbContext();
            var page = await db.ContentPages.FindAsync(id);
            if (page != null)
            {
                page.Title = title.Trim();
                page.Slug = slug.Trim().ToLowerInvariant().Replace(" ", "-");
                page.MetaDescription = metaDescription?.Trim() ?? string.Empty;
                page.HtmlContent = htmlContent ?? string.Empty;
                page.IsPublished = isPublished;
                page.LastUpdated = DateTime.UtcNow;

                await db.SaveChangesAsync();
                TempData["Success"] = $"Page '{title}' updated successfully!";
            }
            return RedirectToAction("Index");
        }
    }
}
