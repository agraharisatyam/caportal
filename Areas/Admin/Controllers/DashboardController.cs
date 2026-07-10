using Microsoft.AspNetCore.Mvc;

namespace caportal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private const string SessionKey = "AdminLoggedIn";

        // Guard: redirect to login if not authenticated
        private IActionResult? RequireAuth()
        {
            if (HttpContext.Session.GetString(SessionKey) != "true")
                return RedirectToAction("Login", "Auth", new { area = "Admin" });
            return null;
        }

        // GET /Admin/Dashboard  or  /ajs
        public IActionResult Index()
        {
            var guard = RequireAuth();
            if (guard != null) return guard;

            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            return View();
        }

        // GET /Admin/Dashboard/CaList
        public IActionResult CaList()
        {
            var guard = RequireAuth();
            if (guard != null) return guard;
            return View();
        }

        // GET /Admin/Dashboard/Clients
        public IActionResult Clients()
        {
            var guard = RequireAuth();
            if (guard != null) return guard;
            return View();
        }
    }
}
