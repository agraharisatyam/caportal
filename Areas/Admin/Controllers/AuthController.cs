using Microsoft.AspNetCore.Mvc;

namespace caportal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        private const string ValidUsername = "ajs";
        private const string ValidPassword = "ajs@1503";
        private const string SessionKey   = "AdminLoggedIn";
        private const string SessionUser  = "AdminUsername";

        // GET /Admin/Auth/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (HttpContext.Session.GetString(SessionKey) == "true")
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST /Admin/Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password, string? returnUrl = null)
        {
            if (username == ValidUsername && password == ValidPassword)
            {
                HttpContext.Session.SetString(SessionKey, "true");
                HttpContext.Session.SetString(SessionUser, username);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            ViewBag.Error     = "Invalid username or password.";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST /Admin/Auth/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth", new { area = "Admin" });
        }
    }
}
