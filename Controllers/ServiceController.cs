using caportal.Data;
using caportal.Models;
using caportal.Models.Entities;
using caportal.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace caportal.Controllers
{
    public class ServiceController : Controller
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
        private readonly SiteSettingsService _settingsService;

        public ServiceController(
            IDbContextFactory<ApplicationDbContext> dbFactory,
            SiteSettingsService settingsService)
        {
            _dbFactory       = dbFactory;
            _settingsService = settingsService;
        }

        // GET /service/{id}
        public async Task<IActionResult> Detail(int id)
        {
            using var db = _dbFactory.CreateDbContext();
            var service = await db.CoveredServices.FindAsync(id);
            if (service == null) return RedirectToAction("Index", "Home");

            ViewBag.Settings    = _settingsService.Get();
            ViewBag.AllServices = await db.CoveredServices
                                          .OrderBy(s => s.DisplayOrder)
                                          .ToListAsync();
            return View("Detail", service);
        }
    }
}
