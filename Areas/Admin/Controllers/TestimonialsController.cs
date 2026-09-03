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
    public class TestimonialsController : Controller
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public TestimonialsController(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        // GET /Admin/Testimonials
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            using var db = _dbFactory.CreateDbContext();
            var testimonials = await db.Testimonials.Where(t => t.IsActive).OrderBy(t => t.DisplayOrder).ThenBy(t => t.Id).ToListAsync();
            return View(testimonials);
        }

        // POST /Admin/Testimonials/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(string text, string authorName, string authorRole, int rating)
        {
            if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(authorName))
            {
                var initials = authorName.Split(' ').Length > 1 
                    ? $"{authorName.Split(' ')[0][0]}{authorName.Split(' ')[1][0]}".ToUpper()
                    : authorName[..Math.Min(2, authorName.Length)].ToUpper();

                using var db = _dbFactory.CreateDbContext();
                var maxOrder = await db.Testimonials.AnyAsync() ? await db.Testimonials.MaxAsync(t => t.DisplayOrder) : 0;
                var item = new TestimonialEntity
                {
                    Text = text.Trim(),
                    AuthorName = authorName.Trim(),
                    AuthorRole = authorRole?.Trim() ?? string.Empty,
                    Initials = initials,
                    Rating = rating > 0 ? rating : 5,
                    DisplayOrder = maxOrder + 1,
                    IsActive = true
                };
                db.Testimonials.Add(item);
                await db.SaveChangesAsync();
                TempData["Success"] = $"Testimonial by '{authorName}' added successfully!";
            }
            return RedirectToAction("Index");
        }

        // POST /Admin/Testimonials/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            using var db = _dbFactory.CreateDbContext();
            var item = await db.Testimonials.FindAsync(id);
            if (item != null)
            {
                db.Testimonials.Remove(item);
                await db.SaveChangesAsync();
                TempData["Success"] = $"Testimonial removed successfully.";
            }
            return RedirectToAction("Index");
        }
    }
}
