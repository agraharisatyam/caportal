using caportal.Filters;
using caportal.Models.Entities;
using caportal.Services.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace caportal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class BlogController : Controller
    {
        // GET /Admin/Blog
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            var posts = BlogRepository.GetAll(includeUnpublished: true);
            return View(posts);
        }

        // GET /Admin/Blog/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            return View(new BlogPost());
        }

        // POST /Admin/Blog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BlogPost post, string tagsText)
        {
            if (!string.IsNullOrWhiteSpace(post.Title))
            {
                if (!string.IsNullOrEmpty(tagsText))
                {
                    post.Tags = tagsText.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
                }

                BlogRepository.Add(post);
                TempData["Success"] = $"Article '{post.Title}' published successfully!";
                return RedirectToAction("Index");
            }
            return View(post);
        }

        // GET /Admin/Blog/Edit/1
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            var post = BlogRepository.GetById(id);
            if (post == null) return RedirectToAction("Index");
            return View(post);
        }

        // POST /Admin/Blog/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BlogPost post, string tagsText)
        {
            if (post != null && post.Id > 0)
            {
                if (!string.IsNullOrEmpty(tagsText))
                {
                    post.Tags = tagsText.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
                }

                BlogRepository.Update(post);
                TempData["Success"] = $"Article '{post.Title}' updated!";
                return RedirectToAction("Index");
            }
            return View(post);
        }

        // POST /Admin/Blog/TogglePublish
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TogglePublish(int id)
        {
            var post = BlogRepository.GetById(id);
            if (post != null)
            {
                post.IsPublished = !post.IsPublished;
                BlogRepository.Update(post);
                TempData["Success"] = post.IsPublished ? $"Article '{post.Title}' is now LIVE." : $"Article '{post.Title}' set to Draft.";
            }
            return RedirectToAction("Index");
        }

        // POST /Admin/Blog/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var post = BlogRepository.GetById(id);
            if (post != null)
            {
                BlogRepository.Delete(id);
                TempData["Success"] = $"Article '{post.Title}' deleted.";
            }
            return RedirectToAction("Index");
        }
    }
}
