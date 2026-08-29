using caportal.Filters;
using caportal.Services;
using Microsoft.AspNetCore.Mvc;

namespace caportal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class HomepageController : Controller
    {
        private readonly SiteSettingsService _settingsService;

        public HomepageController(SiteSettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        // GET /Admin/Homepage
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Username = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            return View(_settingsService.Get());
        }

        // POST /Admin/Homepage/SaveHero
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveHero(string heroTitle, string heroSubtitle, string heroAccentColor, string heroBgFrom, string heroBgTo)
        {
            var settings = _settingsService.Get();
            settings.HeroTitle = heroTitle ?? settings.HeroTitle;
            settings.HeroSubtitle = heroSubtitle ?? settings.HeroSubtitle;
            settings.HeroAccentColor = heroAccentColor ?? settings.HeroAccentColor;
            settings.HeroBgFrom = heroBgFrom ?? settings.HeroBgFrom;
            settings.HeroBgTo = heroBgTo ?? settings.HeroBgTo;
            _settingsService.Save(settings);

            TempData["Success"] = "Homepage Hero Section updated!";
            return RedirectToAction("Index");
        }

        // POST /Admin/Homepage/ToggleSections
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleSections(
            bool showHeroSection, bool showServicesSection, bool showAboutUsSection,
            bool showFeaturedCAsSection, bool showTestimonialsSection, bool showBlogSection,
            bool showCtaSection, bool showFooter)
        {
            var settings = _settingsService.Get();
            settings.ShowHeroSection = showHeroSection;
            settings.ShowServicesSection = showServicesSection;
            settings.ShowAboutUsSection = showAboutUsSection;
            settings.ShowFeaturedCAsSection = showFeaturedCAsSection;
            settings.ShowTestimonialsSection = showTestimonialsSection;
            settings.ShowBlogSection = showBlogSection;
            settings.ShowCtaSection = showCtaSection;
            settings.ShowFooter = showFooter;
            _settingsService.Save(settings);

            TempData["Success"] = "Homepage Section visibility updated!";
            return RedirectToAction("Index");
        }
    }
}
