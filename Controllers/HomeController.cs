using caportal.Data;
using caportal.Models;
using caportal.Models.Entities;
using caportal.Models.ViewModels;
using caportal.Services;
using caportal.Services.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace caportal.Controllers
{
    public class HomeController : Controller
    {
        private readonly SiteSettingsService _settingsService;
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
        private readonly caportal.Services.Email.IEmailService _emailService;

        public HomeController(
            SiteSettingsService settingsService, 
            IDbContextFactory<ApplicationDbContext> dbFactory,
            caportal.Services.Email.IEmailService emailService)
        {
            _settingsService = settingsService;
            _dbFactory = dbFactory;
            _emailService = emailService;
        }

        // GET /site-dynamic.css — returns CSS generated from current settings
        [HttpGet("/site-dynamic.css")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public ContentResult DynamicCss()
        {
            return Content(_settingsService.GenerateCss(), "text/css");
        }

        public async Task<IActionResult> Index()
        {
            var settings = _settingsService.Get();

            // Self-healing: if columns are empty in database for the existing row, populate default values
            if (string.IsNullOrEmpty(settings.WhyChooseUsTitle))
            {
                settings.WhyChooseUsBadge      = "WHY CHOOSE";
                settings.WhyChooseUsTitle      = "CA CHARTERED CAMPUS?";
                settings.WhyChooseUsSub        = "We combine expertise, technology and commitment to deliver reliable CA, legal and compliance solutions for individuals and businesses.";
                settings.WhyChooseUsStatsTitle = "TRUSTED BY 50,000+ BUSINESSES";
                settings.WhyChooseUsStat1Val   = "50,000+";
                settings.WhyChooseUsStat1Lbl   = "Businesses Served";
                settings.WhyChooseUsStat2Val   = "200+";
                settings.WhyChooseUsStat2Lbl   = "Expert CAs";
                settings.WhyChooseUsStat3Val   = "15+";
                settings.WhyChooseUsStat3Lbl   = "Service Categories";
                settings.WhyChooseUsStat4Val   = "24x7";
                settings.WhyChooseUsStat4Lbl   = "Support Available";
                _settingsService.Save(settings);
            }

            if (string.IsNullOrEmpty(settings.FeaturedCAsTitle))
            {
                settings.FeaturedCAsBadge      = "Top Talent";
                settings.FeaturedCAsTitle      = "Trusted Chartered Accountants <span>Across India</span>";
                settings.FeaturedCAsSubtitle   = "Handpicked CAs with proven track records, top ratings, and deep domain expertise.";
                _settingsService.Save(settings);
            }

            ViewBag.Settings = settings;
            ViewBag.ServicesBadge = settings.ServicesBadge;
            ViewBag.ServicesTitle = settings.ServicesTitle;

            // Load services from DB, seed defaults if empty
            using var db = _dbFactory.CreateDbContext();
            var services = await db.CoveredServices.OrderBy(s => s.DisplayOrder).ToListAsync();

            if (!services.Any())
            {
                var defaults = new List<CoveredService>
                {
                    new() { Icon="fas fa-file-invoice-dollar", Title="GST & Indirect Tax",    Description="Registration, returns, audits & litigation",         DisplayOrder=1, ImagePath="/images/services/gst-tax.svg" },
                    new() { Icon="fas fa-landmark",            Title="Income Tax",             Description="ITR filing, assessments & appeals",                  DisplayOrder=2, ImagePath="/images/services/income-tax.svg" },
                    new() { Icon="fas fa-building",            Title="Corporate Compliance",   Description="ROC filings, MCA & company law",                     DisplayOrder=3, ImagePath="/images/services/corporate-compliance.svg" },
                    new() { Icon="fas fa-balance-scale",       Title="Tax Litigation",         Description="ITAT, HC & SC representations",                     DisplayOrder=4, ImagePath="/images/services/tax-litigation.svg" },
                    new() { Icon="fas fa-chart-line",          Title="Audit & Assurance",      Description="Statutory, internal & forensic audit",               DisplayOrder=5, ImagePath="/images/services/audit-assurance.svg" },
                    new() { Icon="fas fa-rocket",              Title="Startup Finance",         Description="Fundraising, ESOP & equity structuring",             DisplayOrder=6, ImagePath="/images/services/startup-finance.svg" },
                    new() { Icon="fas fa-globe",               Title="FEMA & RBI",             Description="Cross-border transactions & NRI taxation",           DisplayOrder=7, ImagePath="/images/services/fema-rbi.svg" },
                    new() { Icon="fas fa-handshake",           Title="Transfer Pricing",        Description="International transactions & documentation",         DisplayOrder=8, ImagePath="/images/services/transfer-pricing.svg" },
                };
                db.CoveredServices.AddRange(defaults);
                await db.SaveChangesAsync();
                services = defaults;
            }

            // Ensure all services have high quality images assigned if currently empty
            var defaultImgMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "GST & Indirect Tax",    "/images/services/gst-tax.svg" },
                { "Income Tax",             "/images/services/income-tax.svg" },
                { "Corporate Compliance",   "/images/services/corporate-compliance.svg" },
                { "Tax Litigation",         "/images/services/tax-litigation.svg" },
                { "Audit & Assurance",      "/images/services/audit-assurance.svg" },
                { "Startup Finance",         "/images/services/startup-finance.svg" },
                { "FEMA & RBI",             "/images/services/fema-rbi.svg" },
                { "Transfer Pricing",        "/images/services/transfer-pricing.svg" }
            };

            bool servicesUpdated = false;
            foreach (var s in services)
            {
                if (string.IsNullOrEmpty(s.ImagePath) && defaultImgMap.TryGetValue(s.Title, out var path))
                {
                    s.ImagePath = path;
                    servicesUpdated = true;
                }
            }

            if (servicesUpdated)
            {
                await db.SaveChangesAsync();
            }

            // Load Why Choose Us items from DB, seed defaults if empty
            var whyChooseUsItems = await db.WhyChooseUsItems.OrderBy(w => w.DisplayOrder).ToListAsync();
            if (!whyChooseUsItems.Any())
            {
                var defaults = new List<WhyChooseUsItem>
                {
                    new() { Title = "EXPERT CA TEAM",            Description = "Qualified & experienced CAs and professionals with in-depth knowledge.", Icon = "fas fa-user-graduate", ImagePath="/images/wcu/wcu-team.svg",     DisplayOrder = 1 },
                    new() { Title = "100% RELIABLE",             Description = "Accurate, transparent and dependable services you can trust.",           Icon = "fas fa-shield-alt",    ImagePath="/images/wcu/wcu-reliable.svg", DisplayOrder = 2 },
                    new() { Title = "ON-TIME DELIVERY",          Description = "We value your time and ensure timely delivery of all services.",          Icon = "fas fa-clock",         ImagePath="/images/wcu/wcu-ontime.svg",   DisplayOrder = 3 },
                    new() { Title = "AFFORDABLE PRICING",         Description = "Transparent pricing with no hidden charges and best value for money.",   Icon = "fas fa-rupee-sign",    ImagePath="/images/wcu/wcu-pricing.svg",  DisplayOrder = 4 },
                    new() { Title = "END-TO-END SUPPORT",        Description = "From consultation to completion, we provide complete support.",          Icon = "fas fa-thumbs-up",     ImagePath="/images/wcu/wcu-support.svg",  DisplayOrder = 5 },
                    new() { Title = "100% ONLINE PROCESS",       Description = "Paperless, hassle-free and fully online service experience.",             Icon = "fas fa-desktop",       ImagePath="/images/wcu/wcu-online.svg",   DisplayOrder = 6 },
                    new() { Title = "SECURE & CONFIDENTIAL",     Description = "Your data and documents are safe with us at every step.",                 Icon = "fas fa-lock",          ImagePath="/images/wcu/wcu-secure.svg",   DisplayOrder = 7 },
                    new() { Title = "GOVERNMENT APPROVED",       Description = "All services are as per government norms and regulations.",               Icon = "fas fa-landmark",      ImagePath="/images/wcu/wcu-approved.svg", DisplayOrder = 8 },
                    new() { Title = "WIDE RANGE OF SERVICES",     Description = "One-stop solution for CA, tax, legal, compliance and business needs.",   Icon = "fas fa-award",         ImagePath="/images/wcu/wcu-range.svg",    DisplayOrder = 9 },
                    new() { Title = "DEDICATED SUPPORT",         Description = "Our support team is always available to assist you.",                     Icon = "fas fa-headset",       ImagePath="/images/wcu/wcu-headset.svg",  DisplayOrder = 10 }
                };
                db.WhyChooseUsItems.AddRange(defaults);
                await db.SaveChangesAsync();
                whyChooseUsItems = defaults;
            }

            // Ensure all Why Choose Us items have high quality images assigned if currently empty
            var wcuImgMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "EXPERT CA TEAM",        "/images/wcu/wcu-team.svg" },
                { "100% RELIABLE",         "/images/wcu/wcu-reliable.svg" },
                { "ON-TIME DELIVERY",      "/images/wcu/wcu-ontime.svg" },
                { "AFFORDABLE PRICING",     "/images/wcu/wcu-pricing.svg" },
                { "END-TO-END SUPPORT",    "/images/wcu/wcu-support.svg" },
                { "100% ONLINE PROCESS",   "/images/wcu/wcu-online.svg" },
                { "SECURE & CONFIDENTIAL", "/images/wcu/wcu-secure.svg" },
                { "GOVERNMENT APPROVED",   "/images/wcu/wcu-approved.svg" },
                { "WIDE RANGE OF SERVICES", "/images/wcu/wcu-range.svg" },
                { "DEDICATED SUPPORT",     "/images/wcu/wcu-headset.svg" }
            };

            bool wcuUpdated = false;
            foreach (var item in whyChooseUsItems)
            {
                if (string.IsNullOrEmpty(item.ImagePath) && wcuImgMap.TryGetValue(item.Title, out var path))
                {
                    item.ImagePath = path;
                    wcuUpdated = true;
                }
            }

            if (wcuUpdated)
            {
                await db.SaveChangesAsync();
            }

            // Load Hero Banner slides from DB, seed default if empty
            List<HeroBannerSlide> heroSlides = new();
            try
            {
                var createTableSql = @"
                    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'HeroBannerSlides')
                    BEGIN
                        CREATE TABLE [HeroBannerSlides] (
                            [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                            [Title] NVARCHAR(200) NOT NULL DEFAULT '',
                            [Subtitle] NVARCHAR(500) NULL,
                            [Badge] NVARCHAR(100) NULL,
                            [ImagePath] NVARCHAR(500) NOT NULL DEFAULT '/images/hero-banner.png',
                            [MobileImagePath] NVARCHAR(500) NULL,
                            [LinkUrl] NVARCHAR(500) NULL,
                            [ButtonText] NVARCHAR(100) NULL,
                            [ButtonUrl] NVARCHAR(500) NULL,
                            [SecondaryButtonText] NVARCHAR(100) NULL,
                            [SecondaryButtonUrl] NVARCHAR(500) NULL,
                            [DisplayOrder] INT NOT NULL DEFAULT 0,
                            [IsActive] BIT NOT NULL DEFAULT 1,
                            [SlideType] NVARCHAR(50) NOT NULL DEFAULT 'image',
                            [BgGradientFrom] NVARCHAR(50) NULL DEFAULT '#0a1628',
                            [BgGradientTo] NVARCHAR(50) NULL DEFAULT '#1a2a4a',
                            [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
                        );
                    END";
                db.Database.ExecuteSqlRaw(createTableSql);

                heroSlides = await db.HeroBannerSlides.Where(s => s.IsActive).OrderBy(s => s.DisplayOrder).ToListAsync();
                if (!heroSlides.Any())
                {
                    var defaultSlide = new HeroBannerSlide
                    {
                        Title = "CA & Legal Compliance Platform",
                        Subtitle = "Connecting businesses with ICAI-verified Chartered Accountants across India.",
                        Badge = "⭐ India's #1 Verified CA Network",
                        ImagePath = !string.IsNullOrEmpty(settings.HeroBannerImage) ? settings.HeroBannerImage : "/images/hero-banner.png",
                        MobileImagePath = settings.HeroBannerMobileImage,
                        LinkUrl = settings.HeroBannerLink,
                        ButtonText = settings.HeroPrimaryCtaText ?? "Find a CA",
                        ButtonUrl = settings.HeroPrimaryCtaUrl ?? "/find-expert",
                        DisplayOrder = 1,
                        IsActive = true,
                        SlideType = settings.HeroMode ?? "image"
                    };
                    db.HeroBannerSlides.Add(defaultSlide);
                    await db.SaveChangesAsync();
                    heroSlides = new List<HeroBannerSlide> { defaultSlide };
                }
            }
            catch
            {
                // Fallback
            }

            var dbCas = await db.CaProfessionals
                .Where(c => c.Status == "Active")
                .OrderByDescending(c => c.IsFeatured)
                .ThenBy(c => c.DisplayOrder)
                .ThenByDescending(c => c.Rating)
                .Take(10)
                .ToListAsync();

            var dbTestimonials = await db.Testimonials
                .Where(t => t.IsActive)
                .OrderBy(t => t.DisplayOrder)
                .Select(t => new Testimonial
                {
                    Text = t.Text,
                    AuthorName = t.AuthorName,
                    AuthorRole = t.AuthorRole,
                    Initials = t.Initials,
                    Rating = t.Rating
                })
                .ToListAsync();

            var dbFaqs = await db.Faqs
                .Where(f => f.IsActive)
                .OrderBy(f => f.DisplayOrder)
                .Select(f => new FaqItem
                {
                    Question = f.Question,
                    Answer = f.Answer
                })
                .ToListAsync();

            var dbPricingPlans = await db.PricingPlans
                .Where(p => p.IsActive)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();

            var vm = new HomeViewModel
            {
                Services = services,
                WhyChooseUsItems = whyChooseUsItems,
                HeroBannerSlides = heroSlides,
                Stats = new SiteStats
                {
                    TotalCAs          = $"{Math.Max(500, dbCas.Count)}+",
                    ClientSatisfaction = "98%",
                    CasesHandled      = "50K+",
                    Cities            = "Pan-India"
                },
                FeaturedProfessionals = dbCas,
                Testimonials = dbTestimonials,
                Faqs = dbFaqs,
                PricingPlans = dbPricingPlans
            };
            return View(vm);
        }

        [HttpGet("/privacy")]
        [HttpGet("/privacy-policy")]
        public Task<IActionResult> PrivacyPolicy() => Page("privacy-policy");

        [HttpGet("/terms")]
        [HttpGet("/terms-and-conditions")]
        public Task<IActionResult> TermsAndConditions() => Page("terms-and-conditions");

        [HttpGet("/refund-policy")]
        [HttpGet("/cancellation-and-refund-policy")]
        public Task<IActionResult> RefundPolicy() => Page("cancellation-and-refund-policy");

        [HttpGet("/about")]
        [HttpGet("/about-us")]
        public Task<IActionResult> AboutUs() => Page("about-us");

        [HttpGet("/contact-us")]
        public IActionResult ContactUsRedirect() => RedirectToAction("Contact");

        [HttpGet("/page/{slug}")]
        [HttpGet("/pages/{slug}")]
        public async Task<IActionResult> Page(string slug)
        {
            ViewBag.Settings = _settingsService.Get();
            using var db = _dbFactory.CreateDbContext();
            var page = await db.ContentPages.FirstOrDefaultAsync(p => p.Slug.ToLower() == slug.ToLower() && p.IsPublished);
            if (page == null)
            {
                return RedirectToAction("Index");
            }
            return View(page);
        }

        [HttpGet("/services/{slug}")]
        public IActionResult ServiceDetail(string slug)
        {
            var service = ServiceDetailsRepository.GetBySlug(slug);
            if (service == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Settings = _settingsService.Get();
            return View(service);
        }

        // ── Find an Expert listing ─────────────────────────────────────────
        [HttpGet("/find-expert")]
        public async Task<IActionResult> FindExpert(string? city, string? service, string? exp, string? rating, string? sort, int page = 1)
        {
            ViewBag.Settings = _settingsService.Get();

            using var db = _dbFactory.CreateDbContext();
            var allProfessionals = await db.CaProfessionals
                .Where(p => p.Status == "Active")
                .ToListAsync();

            // Apply filters
            if (!string.IsNullOrEmpty(city))
                allProfessionals = allProfessionals.Where(p => p.City.Equals(city, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrEmpty(service))
                allProfessionals = allProfessionals.Where(p => p.Specialisations.Any(s => s.Contains(service, StringComparison.OrdinalIgnoreCase))).ToList();

            if (!string.IsNullOrEmpty(exp))
            {
                allProfessionals = exp switch
                {
                    "0-5"  => allProfessionals.Where(p => p.YearsExp <= 5).ToList(),
                    "5-10" => allProfessionals.Where(p => p.YearsExp is >= 5 and <= 10).ToList(),
                    "10+"  => allProfessionals.Where(p => p.YearsExp > 10).ToList(),
                    _      => allProfessionals
                };
            }

            if (!string.IsNullOrEmpty(rating) && decimal.TryParse(rating, out var minRating))
                allProfessionals = allProfessionals.Where(p => p.Rating >= minRating).ToList();

            // Sort
            allProfessionals = sort switch
            {
                "rating"   => allProfessionals.OrderByDescending(p => p.Rating).ToList(),
                "exp"      => allProfessionals.OrderByDescending(p => p.YearsExp).ToList(),
                "fee_asc"  => allProfessionals.OrderBy(p => p.YearsExp).ToList(),
                "fee_desc" => allProfessionals.OrderByDescending(p => p.YearsExp).ToList(),
                _          => allProfessionals.OrderByDescending(p => p.IsFeatured).ThenByDescending(p => p.Rating).ToList()
            };

            ViewBag.Professionals = allProfessionals;
            ViewBag.Page     = page;
            ViewBag.QCity    = city    ?? "";
            ViewBag.QService = service ?? "";
            ViewBag.QExp     = exp     ?? "";
            ViewBag.QRating  = rating  ?? "";
            ViewBag.QSort    = sort    ?? "relevant";

            return View();
        }

        // ── Expert detail profile ──────────────────────────────────────────
        [HttpGet("/expert/{slug}")]
        public async Task<IActionResult> ExpertDetail(string slug)
        {
            ViewBag.Settings = _settingsService.Get();

            var parts = slug.Split('-');
            int expertId = 1;
            if (parts.Length > 0 && int.TryParse(parts[^1], out var parsedId))
                expertId = parsedId;

            using var db = _dbFactory.CreateDbContext();
            var professional = await db.CaProfessionals.FirstOrDefaultAsync(p => p.Id == expertId)
                ?? await db.CaProfessionals.FirstOrDefaultAsync(p => p.Status == "Active")
                ?? new CaProfessional { Name = "Chartered Accountant", City = "Pan-India" };

            return View(professional);
        }

        // ── Consultation booking submission ────────────────────────────────
        [HttpPost("/expert/book")]
        public async Task<IActionResult> BookExpert([FromForm] string name, [FromForm] string phone, [FromForm] string? email, [FromForm] string? service, [FromForm] string? description, [FromForm] int expertId, [FromForm] string? website = null)
        {
            // Bot honeypot detection
            if (!string.IsNullOrEmpty(website))
            {
                return Json(new { success = true, message = "Consultation requested successfully!" });
            }

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phone))
            {
                return Json(new { success = false, message = "Please provide your full name and mobile number." });
            }

            try
            {
                using var db = _dbFactory.CreateDbContext();
                var expert = await db.CaProfessionals.FindAsync(expertId);
                var req = new ClientRequest
                {
                    ClientName = name.Trim(),
                    ClientPhone = phone.Trim(),
                    ClientEmail = email?.Trim() ?? string.Empty,
                    ServiceRequired = service?.Trim() ?? "CA Consultation",
                    AssignedCA = expert != null ? expert.Name : "Unassigned",
                    Description = description?.Trim() ?? string.Empty,
                    Source = "Expert Profile Booking",
                    Status = "Pending",
                    RequestedOn = DateTime.UtcNow
                };
                db.ClientRequests.Add(req);
                await db.SaveChangesAsync();

                // Send email notifications in background
                if (!string.IsNullOrWhiteSpace(email))
                {
                    _ = _emailService.SendEmailAsync(
                        email.Trim(),
                        "Consultation Request Confirmed - CA Chartered Campus",
                        $"<h3>Dear {name.Trim()},</h3><p>We have received your consultation request for <strong>{req.ServiceRequired}</strong> with <strong>{req.AssignedCA}</strong>.</p><p>Our dedicated desk will coordinate your appointment within 2 business hours.</p><p>Best regards,<br>CA Chartered Campus Team</p>",
                        name.Trim());
                }

                var settings = _settingsService.Get();
                if (!string.IsNullOrWhiteSpace(settings.SiteEmail))
                {
                    _ = _emailService.SendEmailAsync(
                        settings.SiteEmail,
                        $"[New CA Booking] {name.Trim()} - {req.AssignedCA}",
                        $"<h3>New Consultation Booking</h3><p><strong>Client:</strong> {name.Trim()}<br><strong>Phone:</strong> {phone.Trim()}<br><strong>Email:</strong> {email ?? "N/A"}<br><strong>Expert:</strong> {req.AssignedCA}<br><strong>Service:</strong> {req.ServiceRequired}<br><strong>Notes:</strong> {description ?? "N/A"}</p>");
                }

                return Json(new { success = true, message = "Consultation requested successfully! Our senior consultation desk will connect with you promptly." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error saving request: " + ex.Message });
            }
        }

        // ── Newsletter subscription ────────────────────────────────────────
        [HttpPost("/newsletter/subscribe")]
        public async Task<IActionResult> SubscribeNewsletter([FromForm] string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            {
                return Json(new { success = false, message = "Please enter a valid email address." });
            }

            try
            {
                using var db = _dbFactory.CreateDbContext();
                var normalized = email.Trim().ToLowerInvariant();
                var existing = await db.NewsletterSubscribers.FirstOrDefaultAsync(s => s.Email.ToLower() == normalized);
                if (existing == null)
                {
                    db.NewsletterSubscribers.Add(new NewsletterSubscriber
                    {
                        Email = normalized,
                        SubscribedAt = DateTime.UtcNow,
                        IsActive = true
                    });
                    await db.SaveChangesAsync();

                    // Send welcome email
                    _ = _emailService.SendEmailAsync(
                        normalized,
                        "Welcome to CA Chartered Campus Insights",
                        "<h3>Welcome to CA Chartered Campus!</h3><p>Thank you for subscribing. You'll now receive timely updates on GST advisories, direct tax amendments, MCA notifications, and business regulatory updates.</p>");
                }
                return Json(new { success = true, message = "Thank you for subscribing to our tax and compliance newsletter!" });
            }
            catch
            {
                return Json(new { success = false, message = "Unable to subscribe at this moment." });
            }
        }

        // GET /contact
        [HttpGet("/contact")]
        public async Task<IActionResult> Contact()
        {
            ViewBag.Settings = _settingsService.Get();
            using var db = _dbFactory.CreateDbContext();
            var services = await db.CoveredServices.OrderBy(s => s.DisplayOrder).ToListAsync();
            ViewBag.Services = services;
            return View();
        }

        // POST /contact/submit
        [HttpPost("/contact/submit")]
        public async Task<IActionResult> SubmitContact([FromForm] string name, [FromForm] string email, [FromForm] string phone, [FromForm] string city, [FromForm] string clientType, [FromForm] string service, [FromForm] string preferredTime, [FromForm] string message, [FromForm] string? website = null)
        {
            // Bot honeypot detection
            if (!string.IsNullOrEmpty(website))
            {
                return Json(new { success = true, message = "Thank you! Your enquiry has been registered." });
            }

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phone))
            {
                return Json(new { success = false, message = "Please provide your name and phone number." });
            }

            try
            {
                using var db = _dbFactory.CreateDbContext();
                var req = new ClientRequest
                {
                    ClientName = name.Trim(),
                    ClientEmail = email?.Trim() ?? string.Empty,
                    ClientPhone = phone.Trim(),
                    City = city?.Trim() ?? string.Empty,
                    ClientType = clientType?.Trim() ?? "Individual",
                    ServiceRequired = service?.Trim() ?? "General Consultation",
                    Description = message?.Trim() ?? string.Empty,
                    PreferredTime = preferredTime?.Trim() ?? string.Empty,
                    Source = "Contact Page",
                    Status = "Pending",
                    RequestedOn = DateTime.UtcNow
                };
                db.ClientRequests.Add(req);
                await db.SaveChangesAsync();

                // Send email confirmations in background
                if (!string.IsNullOrWhiteSpace(email))
                {
                    _ = _emailService.SendEmailAsync(
                        email.Trim(),
                        "Thank you for contacting CA Chartered Campus",
                        $"<h3>Dear {name.Trim()},</h3><p>We have received your enquiry regarding <strong>{req.ServiceRequired}</strong>.</p><p>One of our senior Chartered Accountants will review your requirements and get in touch within 2 hours.</p><p>Warm regards,<br>CA Chartered Campus Advisory Desk</p>",
                        name.Trim());
                }

                var settings = _settingsService.Get();
                if (!string.IsNullOrWhiteSpace(settings.SiteEmail))
                {
                    _ = _emailService.SendEmailAsync(
                        settings.SiteEmail,
                        $"[New Web Enquiry] {name.Trim()} - {req.ServiceRequired}",
                        $"<h3>New Client Enquiry</h3><p><strong>Name:</strong> {name.Trim()}<br><strong>Phone:</strong> {phone.Trim()}<br><strong>Email:</strong> {email ?? "N/A"}<br><strong>City:</strong> {city ?? "N/A"}<br><strong>Type:</strong> {clientType ?? "N/A"}<br><strong>Service:</strong> {req.ServiceRequired}<br><strong>Time:</strong> {preferredTime ?? "N/A"}<br><strong>Message:</strong><br>{message ?? "N/A"}</p>");
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }

            return Json(new { success = true, message = "Thank you! Your enquiry has been registered and our senior CA consultation desk will connect with you within 2 hours." });
        }

        // POST /Home/SubmitClientRequest (Used by Service/Detail modal)
        [HttpPost("/Home/SubmitClientRequest")]
        public async Task<IActionResult> SubmitClientRequest(
            [FromForm] string? ClientName,
            [FromForm] string? ClientPhone,
            [FromForm] string? ClientEmail,
            [FromForm] string? City,
            [FromForm] string? ServiceRequired,
            [FromForm] string? Description,
            [FromForm] string? PreferredTime,
            [FromForm] string? Source,
            [FromForm] string? website = null)
        {
            if (!string.IsNullOrEmpty(website))
            {
                return Json(new { success = true, message = "Consultation requested successfully!" });
            }

            if (string.IsNullOrWhiteSpace(ClientName) || string.IsNullOrWhiteSpace(ClientPhone))
            {
                return Json(new { success = false, message = "Name and phone number are required." });
            }

            try
            {
                using var db = _dbFactory.CreateDbContext();
                var req = new ClientRequest
                {
                    ClientName = ClientName.Trim(),
                    ClientPhone = ClientPhone.Trim(),
                    ClientEmail = ClientEmail?.Trim() ?? string.Empty,
                    City = City?.Trim() ?? string.Empty,
                    ClientType = "Individual",
                    ServiceRequired = ServiceRequired?.Trim() ?? "CA Consultation",
                    Description = Description?.Trim() ?? string.Empty,
                    PreferredTime = PreferredTime?.Trim() ?? string.Empty,
                    Source = Source?.Trim() ?? "Service Detail Page",
                    Status = "Pending",
                    RequestedOn = DateTime.UtcNow
                };

                db.ClientRequests.Add(req);
                await db.SaveChangesAsync();

                if (!string.IsNullOrWhiteSpace(ClientEmail))
                {
                    _ = _emailService.SendEmailAsync(
                        ClientEmail.Trim(),
                        "Consultation Request Received - CA Chartered Campus",
                        $"<h3>Dear {ClientName.Trim()},</h3><p>We received your request for <strong>{req.ServiceRequired}</strong>.</p><p>Our advisory desk will coordinate with you shortly.</p>",
                        ClientName.Trim());
                }

                var settings = _settingsService.Get();
                if (!string.IsNullOrWhiteSpace(settings.SiteEmail))
                {
                    _ = _emailService.SendEmailAsync(
                        settings.SiteEmail,
                        $"[Service Lead] {ClientName.Trim()} - {req.ServiceRequired}",
                        $"<p><strong>Name:</strong> {ClientName.Trim()}<br><strong>Phone:</strong> {ClientPhone.Trim()}<br><strong>Email:</strong> {ClientEmail ?? "N/A"}<br><strong>Service:</strong> {req.ServiceRequired}<br><strong>Details:</strong> {Description}</p>");
                }

                return Json(new { success = true, message = "Thank you! Your request has been registered." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // POST /Home/PostConsult (Used by sidebar form in ServiceDetail)
        [HttpPost("/Home/PostConsult")]
        public async Task<IActionResult> PostConsult(
            [FromForm] string? Name,
            [FromForm] string? Phone,
            [FromForm] string? ServiceSlug,
            [FromForm] string? Message,
            [FromForm] string? website = null)
        {
            if (!string.IsNullOrEmpty(website))
            {
                return Json(new { success = true, message = "Callback requested!" });
            }

            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Phone))
            {
                return Json(new { success = false, message = "Name and phone are required." });
            }

            try
            {
                using var db = _dbFactory.CreateDbContext();
                var req = new ClientRequest
                {
                    ClientName = Name.Trim(),
                    ClientPhone = Phone.Trim(),
                    ServiceRequired = !string.IsNullOrWhiteSpace(ServiceSlug) ? ServiceSlug.Trim() : "Callback Request",
                    Description = Message?.Trim() ?? string.Empty,
                    Source = "Service Sidebar Callback",
                    Status = "Pending",
                    RequestedOn = DateTime.UtcNow
                };

                db.ClientRequests.Add(req);
                await db.SaveChangesAsync();

                var settings = _settingsService.Get();
                if (!string.IsNullOrWhiteSpace(settings.SiteEmail))
                {
                    _ = _emailService.SendEmailAsync(
                        settings.SiteEmail,
                        $"[Callback Request] {Name.Trim()} - {req.ServiceRequired}",
                        $"<p><strong>Name:</strong> {Name.Trim()}<br><strong>Phone:</strong> {Phone.Trim()}<br><strong>Message:</strong> {Message}</p>");
                }

                return Json(new { success = true, message = "Callback requested successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // GET /sitemap.xml (Comprehensive Dynamic XML Sitemap)
        [HttpGet("/sitemap.xml")]
        [ResponseCache(Duration = 43200, Location = ResponseCacheLocation.Any)]
        public async Task<ContentResult> Sitemap()
        {
            var host = $"{Request.Scheme}://{Request.Host}";
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            // Static key routes
            sb.AppendLine($"  <url><loc>{host}/</loc><changefreq>daily</changefreq><priority>1.0</priority></url>");
            sb.AppendLine($"  <url><loc>{host}/find-expert</loc><changefreq>daily</changefreq><priority>0.9</priority></url>");
            sb.AppendLine($"  <url><loc>{host}/blog</loc><changefreq>daily</changefreq><priority>0.8</priority></url>");
            sb.AppendLine($"  <url><loc>{host}/contact</loc><changefreq>monthly</changefreq><priority>0.7</priority></url>");

            try
            {
                using var db = _dbFactory.CreateDbContext();

                // Dynamic Blog Posts
                var blogs = await db.BlogPosts.Where(b => b.IsPublished).Select(b => new { b.Slug, b.PublishedDate }).ToListAsync();
                foreach (var blog in blogs)
                {
                    sb.AppendLine($"  <url><loc>{host}/blog/{blog.Slug}</loc><lastmod>{blog.PublishedDate:yyyy-MM-dd}</lastmod><changefreq>weekly</changefreq><priority>0.8</priority></url>");
                }

                // Dynamic Covered Services
                var services = await db.CoveredServices.Select(s => s.Id).ToListAsync();
                foreach (var sid in services)
                {
                    sb.AppendLine($"  <url><loc>{host}/service/{sid}</loc><changefreq>weekly</changefreq><priority>0.8</priority></url>");
                }

                // Dynamic Content Pages (Legal & Info)
                var pages = await db.ContentPages.Where(p => p.IsPublished).Select(p => new { p.Slug, p.LastUpdated }).ToListAsync();
                foreach (var page in pages)
                {
                    sb.AppendLine($"  <url><loc>{host}/page/{page.Slug}</loc><lastmod>{page.LastUpdated:yyyy-MM-dd}</lastmod><changefreq>monthly</changefreq><priority>0.6</priority></url>");
                }
            }
            catch
            {
                // Fallback static links if DB fails
            }

            sb.AppendLine("</urlset>");
            return Content(sb.ToString(), "application/xml");
        }

        // GET /robots.txt
        [HttpGet("/robots.txt")]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
        public ContentResult Robots()
        {
            var host = $"{Request.Scheme}://{Request.Host}";
            var txt = $@"User-agent: *
Allow: /
Disallow: /Admin/
Disallow: /ajs

Sitemap: {host}/sitemap.xml
";
            return Content(txt, "text/plain");
        }

        [Route("/not-found")]
        [Route("/Home/NotFoundPage/{statusCode?}")]
        public IActionResult NotFoundPage(int? statusCode = null)
        {
            Response.StatusCode = statusCode ?? 404;
            return View("NotFound");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
