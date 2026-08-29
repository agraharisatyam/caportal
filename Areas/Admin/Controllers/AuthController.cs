using caportal.Services.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace caportal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        private readonly AdminAuthService _authService;
        private readonly LoginRateLimiter _rateLimiter;

        public AuthController(AdminAuthService authService, LoginRateLimiter rateLimiter)
        {
            _authService = authService;
            _rateLimiter = rateLimiter;
        }

        // GET /Admin/Auth/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (_authService.IsAuthenticated(HttpContext))
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST /Admin/Auth/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password, string? returnUrl = null)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            username = username?.Trim() ?? "";

            // 1. Check rate limiter lockout
            if (_rateLimiter.IsLockedOut(ip, username, out int remainingSecs))
            {
                ViewBag.Error = $"Account locked due to multiple failed login attempts. Please try again in {remainingSecs} seconds.";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            // 2. Validate PBKDF2 hashed credentials
            if (_authService.ValidateCredentials(username, password, out var user) && user != null)
            {
                _rateLimiter.ResetAttempts(ip, username);
                _authService.AuthenticateSession(HttpContext, user);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            // 3. Record failed attempt
            int remainingAttempts = _rateLimiter.RecordFailedAttempt(ip, username, out bool justLockedOut);
            if (justLockedOut)
            {
                ViewBag.Error = "Maximum failed login attempts exceeded. Your account/IP has been locked for 5 minutes.";
            }
            else
            {
                ViewBag.Error = $"Invalid username or password. {remainingAttempts} attempt(s) remaining before lockout.";
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST /Admin/Auth/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            _authService.SignOutSession(HttpContext);
            return RedirectToAction("Login", "Auth", new { area = "Admin" });
        }
    }
}
