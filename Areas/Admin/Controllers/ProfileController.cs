using caportal.Filters;
using Microsoft.AspNetCore.Mvc;

namespace caportal.Areas.Admin.Controllers
{
    public class BusinessProfileViewModel
    {
        public string BusinessName { get; set; } = "ABC Consultancy Pvt. Ltd.";
        public string Email { get; set; } = "info@abcconsultancy.com";
        public string BusinessType { get; set; } = "Private Limited Company";
        public string PhoneNumber { get; set; } = "+91 98765 43210";
        public string Industry { get; set; } = "Finance & Accounting";
        public string Website { get; set; } = "https://www.abcconsultancy.com";
        public string YearEstablished { get; set; } = "2018";
        public string RegistrationNumber { get; set; } = "CIN12345678901234";
        public string GstNumber { get; set; } = "27ABCDE1234F1Z5";
        public string PanNumber { get; set; } = "ABCDE1234F";
        public string Tagline { get; set; } = "We provide expert financial, tax and compliance services to businesses and individuals.";
        public string BusinessAddress { get; set; } = "123, Business Park, Sector 21, Mumbai, Maharashtra - 400001";
        public string LogoPath { get; set; } = "/images/services/corporate-compliance.svg";

        // Social Media Links
        public string LinkedIn { get; set; } = "https://linkedin.com/company/abcconsultancy";
        public string Twitter { get; set; } = "https://twitter.com/abcconsultancy";
        public string Facebook { get; set; } = "https://facebook.com/abcconsultancy";
        public string YouTube { get; set; } = "https://youtube.com/@abcconsultancy";
        public string Instagram { get; set; } = "https://instagram.com/abcconsultancy";
        public string BlogUrl { get; set; } = "https://www.abcconsultancy.com/blog";

        // Preview & Status Stats
        public string City { get; set; } = "Mumbai, Maharashtra";
        public decimal Rating { get; set; } = 4.8m;
        public int ReviewCount { get; set; } = 32;
        public string ProjectsCompleted { get; set; } = "200+";
        public string HappyClients { get; set; } = "50+";
        public string TeamMembersCount { get; set; } = "15+";
        public string YearsExperience { get; set; } = "4+ Years";
        public bool IsActive { get; set; } = true;
        public int CompletionPercentage { get; set; } = 85;
    }

    [Area("Admin")]
    [AdminAuthorize]
    public class ProfileController : Controller
    {
        private static BusinessProfileViewModel _profile = new();

        // GET /Admin/Profile or /Admin/BusinessProfile
        [HttpGet]
        [Route("Admin/Profile")]
        [Route("Admin/BusinessProfile")]
        public IActionResult Index(string tab = "details")
        {
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            ViewBag.ActiveTab = tab.ToLowerInvariant();
            return View(_profile);
        }

        // POST /Admin/Profile/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Profile/Save")]
        public IActionResult Save(BusinessProfileViewModel model, IFormFile? logoFile)
        {
            if (model != null)
            {
                _profile.BusinessName = model.BusinessName;
                _profile.Email = model.Email;
                _profile.BusinessType = model.BusinessType;
                _profile.PhoneNumber = model.PhoneNumber;
                _profile.Industry = model.Industry;
                _profile.Website = model.Website;
                _profile.YearEstablished = model.YearEstablished;
                _profile.RegistrationNumber = model.RegistrationNumber;
                _profile.GstNumber = model.GstNumber;
                _profile.PanNumber = model.PanNumber;
                _profile.Tagline = model.Tagline;
                _profile.BusinessAddress = model.BusinessAddress;
                _profile.LinkedIn = model.LinkedIn;
                _profile.Twitter = model.Twitter;
                _profile.Facebook = model.Facebook;
                _profile.YouTube = model.YouTube;
                _profile.Instagram = model.Instagram;
                _profile.BlogUrl = model.BlogUrl;

                TempData["Success"] = "Business Profile updated successfully!";
            }
            return RedirectToAction("Index");
        }

        // POST /Admin/Profile/ToggleStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Profile/ToggleStatus")]
        public IActionResult ToggleStatus()
        {
            _profile.IsActive = !_profile.IsActive;
            TempData["Success"] = _profile.IsActive ? "Business Profile activated!" : "Business Profile deactivated.";
            return RedirectToAction("Index");
        }
    }
}
