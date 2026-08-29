using caportal.Filters;
using caportal.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace caportal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class FaqsController : Controller
    {
        private static readonly List<FaqItem> _faqs = new()
        {
            new FaqItem { Question="How are CA professionals verified on CACampus?", Answer="Every CA listed undergoes identity and membership verification before approving their profile." },
            new FaqItem { Question="Is CACampus free to use for clients?", Answer="Yes. Our Starter plan is free — browse and contact CA professionals at no cost. Upgrade for premium features." },
            new FaqItem { Question="How long does it take to find a CA for my requirement?", Answer="Clients receive prompt direct responses from empaneled CA professionals based on project requirements." },
            new FaqItem { Question="Are payments secure on the platform?", Answer="Absolutely. We support transparent, milestone-based billing for clear deliverable tracking." },
            new FaqItem { Question="Can CAs from any city join CACampus?", Answer="Yes. CACampus is a pan-India platform with empaneled professionals across major business hubs." }
        };

        // GET /Admin/Faqs
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            return View(_faqs);
        }

        // POST /Admin/Faqs/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(string question, string answer)
        {
            if (!string.IsNullOrWhiteSpace(question) && !string.IsNullOrWhiteSpace(answer))
            {
                _faqs.Add(new FaqItem { Question = question, Answer = answer });
                TempData["Success"] = "FAQ item added successfully!";
            }
            return RedirectToAction("Index");
        }

        // POST /Admin/Faqs/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int index)
        {
            if (index >= 0 && index < _faqs.Count)
            {
                _faqs.RemoveAt(index);
                TempData["Success"] = "FAQ item deleted.";
            }
            return RedirectToAction("Index");
        }
    }
}
