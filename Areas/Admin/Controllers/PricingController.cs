using caportal.Filters;
using Microsoft.AspNetCore.Mvc;

namespace caportal.Areas.Admin.Controllers
{
    public class PricingPlanItem
    {
        public int Id { get; set; }
        public string PlanName { get; set; } = "";
        public string PriceDisplay { get; set; } = "";
        public string BillingCycle { get; set; } = "/month";
        public string Description { get; set; } = "";
        public List<string> Features { get; set; } = new();
        public bool IsPopular { get; set; } = false;
    }

    [Area("Admin")]
    [AdminAuthorize]
    public class PricingController : Controller
    {
        private static readonly List<PricingPlanItem> _plans = new()
        {
            new PricingPlanItem { Id=1, PlanName="Starter Plan", PriceDisplay="₹ 0", BillingCycle="Free", Description="For individuals and small business enquiries.", Features=new(){ "Browse Verified CAs", "Contact up to 3 CAs", "Standard Support" }, IsPopular=false },
            new PricingPlanItem { Id=2, PlanName="Professional", PriceDisplay="₹ 1,499", BillingCycle="/month", Description="For growing businesses needing active compliance.", Features=new(){ "Unlimited CA Contacts", "Direct Call & WhatsApp", "Priority Milestone Tracking", "Dedicated Account Manager" }, IsPopular=true },
            new PricingPlanItem { Id=3, PlanName="Enterprise", PriceDisplay="₹ 4,999", BillingCycle="/month", Description="For corporates requiring complete outsourced CA & legal team.", Features=new(){ "Custom CA Retainership", "Dedicated Senior FCA", "End-to-End Audit & Tax", "24/7 SLA Guarantee" }, IsPopular=false }
        };

        // GET /Admin/Pricing
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            return View(_plans);
        }

        // POST /Admin/Pricing/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(string planName, string priceDisplay, string billingCycle, string description, string featuresText, bool isPopular)
        {
            if (!string.IsNullOrWhiteSpace(planName))
            {
                var featureList = featuresText.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

                _plans.Add(new PricingPlanItem
                {
                    Id = _plans.Count + 1,
                    PlanName = planName,
                    PriceDisplay = priceDisplay,
                    BillingCycle = billingCycle,
                    Description = description,
                    Features = featureList,
                    IsPopular = isPopular
                });

                TempData["Success"] = $"Pricing Plan '{planName}' added!";
            }
            return RedirectToAction("Index");
        }

        // POST /Admin/Pricing/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var p = _plans.FirstOrDefault(x => x.Id == id);
            if (p != null)
            {
                _plans.Remove(p);
                TempData["Success"] = $"Plan '{p.PlanName}' removed.";
            }
            return RedirectToAction("Index");
        }
    }
}
