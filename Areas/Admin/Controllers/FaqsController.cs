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
    public class FaqsController : Controller
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public FaqsController(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        // GET /Admin/Faqs
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            using var db = _dbFactory.CreateDbContext();
            var faqs = await db.Faqs.Where(f => f.IsActive).OrderBy(f => f.DisplayOrder).ThenBy(f => f.Id).ToListAsync();
            return View(faqs);
        }

        // POST /Admin/Faqs/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(string question, string answer, string? category)
        {
            if (!string.IsNullOrWhiteSpace(question) && !string.IsNullOrWhiteSpace(answer))
            {
                using var db = _dbFactory.CreateDbContext();
                var maxOrder = await db.Faqs.AnyAsync() ? await db.Faqs.MaxAsync(f => f.DisplayOrder) : 0;
                var item = new FaqItemEntity
                {
                    Question = question.Trim(),
                    Answer = answer.Trim(),
                    Category = string.IsNullOrWhiteSpace(category) ? "General" : category.Trim(),
                    DisplayOrder = maxOrder + 1,
                    IsActive = true
                };
                db.Faqs.Add(item);
                await db.SaveChangesAsync();
                TempData["Success"] = "FAQ item added successfully!";
            }
            return RedirectToAction("Index");
        }

        // POST /Admin/Faqs/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            using var db = _dbFactory.CreateDbContext();
            var item = await db.Faqs.FindAsync(id);
            if (item != null)
            {
                db.Faqs.Remove(item);
                await db.SaveChangesAsync();
                TempData["Success"] = "FAQ item deleted successfully.";
            }
            return RedirectToAction("Index");
        }
    }
}
