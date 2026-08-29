using caportal.Filters;
using caportal.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace caportal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class TestimonialsController : Controller
    {
        private static readonly List<Testimonial> _testimonials = new()
        {
            new Testimonial { Text="Found a GST specialist quickly after posting. The CA resolved our entire compliance backlog seamlessly. Outstanding experience.", AuthorName="Suresh G.", AuthorRole="Business Owner", Initials="SG", Rating=5 },
            new Testimonial { Text="As a startup, we needed specialized guidance on equity structuring. CACampus connected us with an expert who guided us through our seed round.", AuthorName="Nisha R.", AuthorRole="Startup Founder", Initials="NR", Rating=5 },
            new Testimonial { Text="The compliance management process saves us from costly delays every quarter. The CA professionals here are thorough, prompt, and professional.", AuthorName="Mohan K.", AuthorRole="Finance Manager", Initials="MK", Rating=5 }
        };

        // GET /Admin/Testimonials
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            return View(_testimonials);
        }

        // POST /Admin/Testimonials/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(string text, string authorName, string authorRole, int rating)
        {
            if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(authorName))
            {
                var initials = authorName.Split(' ').Length > 1 
                    ? $"{authorName.Split(' ')[0][0]}{authorName.Split(' ')[1][0]}".ToUpper()
                    : authorName[..Math.Min(2, authorName.Length)].ToUpper();

                _testimonials.Add(new Testimonial
                {
                    Text = text,
                    AuthorName = authorName,
                    AuthorRole = authorRole,
                    Initials = initials,
                    Rating = rating > 0 ? rating : 5
                });
                TempData["Success"] = $"Testimonial by '{authorName}' added!";
            }
            return RedirectToAction("Index");
        }

        // POST /Admin/Testimonials/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int index)
        {
            if (index >= 0 && index < _testimonials.Count)
            {
                var name = _testimonials[index].AuthorName;
                _testimonials.RemoveAt(index);
                TempData["Success"] = $"Testimonial by '{name}' removed.";
            }
            return RedirectToAction("Index");
        }
    }
}
