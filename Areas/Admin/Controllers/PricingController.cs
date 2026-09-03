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
    public class PricingController : Controller
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public PricingController(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        // GET /Admin/Pricing
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            using var db = _dbFactory.CreateDbContext();
            var plans = await db.PricingPlans.Where(p => p.IsActive).OrderBy(p => p.DisplayOrder).ThenBy(p => p.Id).ToListAsync();
            return View(plans);
        }

        // POST /Admin/Pricing/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(string planName, string priceDisplay, string billingCycle, string description, string featuresText, bool isPopular)
        {
            if (!string.IsNullOrWhiteSpace(planName))
            {
                var featureList = (featuresText ?? "")
                    .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .ToList();

                using var db = _dbFactory.CreateDbContext();
                var maxOrder = await db.PricingPlans.AnyAsync() ? await db.PricingPlans.MaxAsync(p => p.DisplayOrder) : 0;
                var plan = new PricingPlanEntity
                {
                    PlanName = planName.Trim(),
                    PriceDisplay = priceDisplay?.Trim() ?? "₹ 0",
                    BillingCycle = string.IsNullOrWhiteSpace(billingCycle) ? "/month" : billingCycle.Trim(),
                    Description = description?.Trim() ?? string.Empty,
                    Features = featureList,
                    IsPopular = isPopular,
                    DisplayOrder = maxOrder + 1,
                    IsActive = true
                };

                db.PricingPlans.Add(plan);
                await db.SaveChangesAsync();
                TempData["Success"] = $"Pricing Plan '{planName}' added successfully!";
            }
            return RedirectToAction("Index");
        }

        // POST /Admin/Pricing/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            using var db = _dbFactory.CreateDbContext();
            var p = await db.PricingPlans.FindAsync(id);
            if (p != null)
            {
                db.PricingPlans.Remove(p);
                await db.SaveChangesAsync();
                TempData["Success"] = $"Plan '{p.PlanName}' removed.";
            }
            return RedirectToAction("Index");
        }
    }
}
