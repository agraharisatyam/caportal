using caportal.Data;
using caportal.Models;
using caportal.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace caportal.Controllers
{
    public class HomeController : Controller
    {
        private readonly SiteSettingsService _settingsService;
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public HomeController(SiteSettingsService settingsService, IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _settingsService = settingsService;
            _dbFactory = dbFactory;
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
                    new() { Icon="fas fa-file-invoice-dollar", Title="GST & Indirect Tax",    Description="Registration, returns, audits & litigation",         DisplayOrder=1 },
                    new() { Icon="fas fa-landmark",            Title="Income Tax",             Description="ITR filing, assessments & appeals",                  DisplayOrder=2 },
                    new() { Icon="fas fa-building",            Title="Corporate Compliance",   Description="ROC filings, MCA & company law",                     DisplayOrder=3 },
                    new() { Icon="fas fa-balance-scale",       Title="Tax Litigation",         Description="ITAT, HC & SC representations",                     DisplayOrder=4 },
                    new() { Icon="fas fa-chart-line",          Title="Audit & Assurance",      Description="Statutory, internal & forensic audit",               DisplayOrder=5 },
                    new() { Icon="fas fa-rocket",              Title="Startup Finance",         Description="Fundraising, ESOP & equity structuring",             DisplayOrder=6 },
                    new() { Icon="fas fa-globe",               Title="FEMA & RBI",             Description="Cross-border transactions & NRI taxation",           DisplayOrder=7 },
                    new() { Icon="fas fa-handshake",           Title="Transfer Pricing",        Description="International transactions & documentation",         DisplayOrder=8 },
                };
                db.CoveredServices.AddRange(defaults);
                await db.SaveChangesAsync();
                services = defaults;
            }

            var vm = new HomeViewModel
            {
                Services = services,
                Stats = new SiteStats
                {
                    TotalCAs          = "12K+",
                    ClientSatisfaction = "98%",
                    CasesHandled      = "50K+",
                    Cities            = "200+"
                },
                FeaturedProfessionals =
                [
                    new CaProfessional { Id=1,  Name="CA Priya Mehta",    Initials="PM", Designation="FCA", YearsExp=12, City="Mumbai",    Specialisations=["GST","Income Tax","Audit"],          Rating=4.9m, CasesHandled=340, ResponseTime="1h",  MembershipNo="ICAI/2012/PM001", IsFeatured=true,  JoinedOn=new DateTime(2026,6,1),  Status="Active"  },
                    new CaProfessional { Id=2,  Name="CA Rajesh Sharma",  Initials="RS", Designation="ACA", YearsExp=8,  City="Delhi",     Specialisations=["Transfer Pricing","FEMA"],           Rating=4.8m, CasesHandled=210, ResponseTime="2h",  MembershipNo="ICAI/2016/RS002", IsFeatured=true,  JoinedOn=new DateTime(2026,6,5),  Status="Active"  },
                    new CaProfessional { Id=3,  Name="CA Anita Krishnan", Initials="AK", Designation="FCA", YearsExp=15, City="Bangalore", Specialisations=["Forensic Audit","ROC"],              Rating=5.0m, CasesHandled=500, ResponseTime="30m", MembershipNo="ICAI/2009/AK003", IsFeatured=true,  JoinedOn=new DateTime(2026,6,10), Status="Active"  },
                    new CaProfessional { Id=4,  Name="CA Vikram Joshi",   Initials="VJ", Designation="ACA", YearsExp=6,  City="Pune",      Specialisations=["Startup Finance","MCA"],             Rating=4.7m, CasesHandled=180, ResponseTime="3h",  MembershipNo="ICAI/2018/VJ004", IsFeatured=true,  JoinedOn=new DateTime(2026,6,12), Status="Pending" },
                    new CaProfessional { Id=5,  Name="CA Sunita Patel",   Initials="SP", Designation="FCA", YearsExp=10, City="Ahmedabad", Specialisations=["ROC","MCA","GST"],                   Rating=4.6m, CasesHandled=290, ResponseTime="2h",  MembershipNo="ICAI/2014/SP005", IsFeatured=false, JoinedOn=new DateTime(2026,6,15), Status="Active"  },
                    new CaProfessional { Id=6,  Name="CA Mohit Agarwal",  Initials="MA", Designation="ACA", YearsExp=9,  City="Kolkata",   Specialisations=["Corporate Tax","Transfer Pricing"],  Rating=4.8m, CasesHandled=260, ResponseTime="1h",  MembershipNo="ICAI/2011/MA006", IsFeatured=false, JoinedOn=new DateTime(2026,6,18), Status="Active"  },
                    new CaProfessional { Id=7,  Name="CA Deepa Nair",     Initials="DN", Designation="ACA", YearsExp=4,  City="Chennai",   Specialisations=["FEMA","RBI Compliance"],             Rating=4.5m, CasesHandled=95,  ResponseTime="4h",  MembershipNo="ICAI/2020/DN007", IsFeatured=false, JoinedOn=new DateTime(2026,6,20), Status="Suspended"},
                    new CaProfessional { Id=8,  Name="CA Arjun Singh",    Initials="AS", Designation="FCA", YearsExp=11, City="Hyderabad", Specialisations=["GST","Audit","Income Tax"],          Rating=4.7m, CasesHandled=310, ResponseTime="2h",  MembershipNo="ICAI/2017/AS008", IsFeatured=false, JoinedOn=new DateTime(2026,6,25), Status="Active"  },
                    new CaProfessional { Id=9,  Name="CA Kavita Rao",     Initials="KR", Designation="FCA", YearsExp=13, City="Jaipur",    Specialisations=["Tax Litigation","Income Tax"],       Rating=4.9m, CasesHandled=420, ResponseTime="1h",  MembershipNo="ICAI/2015/KR009", IsFeatured=false, JoinedOn=new DateTime(2026,6,28), Status="Pending" },
                    new CaProfessional { Id=10, Name="CA Nitin Gupta",    Initials="NG", Designation="ACA", YearsExp=7,  City="Lucknow",   Specialisations=["Internal Audit","MCA"],              Rating=4.6m, CasesHandled=145, ResponseTime="3h",  MembershipNo="ICAI/2013/NG010", IsFeatured=false, JoinedOn=new DateTime(2026,6,30), Status="Active"  },
                ],
                Testimonials =
                [
                    new Testimonial { Text="Found a GST specialist within 20 minutes of posting. The CA resolved our entire compliance backlog in a week. Absolutely outstanding platform.", AuthorName="Suresh Gupta",  AuthorRole="CEO, TechVentures Pvt. Ltd.", Initials="SG", Rating=5 },
                    new Testimonial { Text="As a startup, we needed someone who understood equity structuring. CACampus matched us with an expert who guided us through our seed round flawlessly.", AuthorName="Nisha Rao",    AuthorRole="Co-Founder, GreenLeaf Foods",  Initials="NR", Rating=5 },
                    new Testimonial { Text="The compliance reminder feature alone saves us from costly penalties every quarter. The CA professionals here are thorough, prompt, and highly professional.", AuthorName="Mohan Kumar", AuthorRole="Finance Head, Apex Exports",    Initials="MK", Rating=5 },
                ],
                Faqs =
                [
                    new FaqItem { Question="How are CA professionals verified on CACampus?",         Answer="Every CA listed is cross-verified against the ICAI member register. We check membership number, practising certificate validity, and disciplinary history before approving any profile." },
                    new FaqItem { Question="Is CACampus free to use for clients?",                   Answer="Yes. Our Starter plan is completely free — browse and contact up to 3 CA professionals per month at no cost. Upgrade to Professional for unlimited access." },
                    new FaqItem { Question="How long does it take to find a CA for my requirement?", Answer="Most clients receive responses from matched CA professionals within 1–2 hours of posting their requirement." },
                    new FaqItem { Question="Are payments secure on the platform?",                   Answer="Absolutely. We use milestone-based escrow payments. Your payment is held securely and released to the CA only after you confirm the deliverable has been met." },
                    new FaqItem { Question="Can CAs from any city join CACampus?",                   Answer="Yes. CACampus is a pan-India platform. We currently have verified professionals from over 200 cities." },
                ]
            };
            return View(vm);
        }

        public IActionResult Privacy() => View();

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
        public IActionResult FindExpert(string? city, string? service, string? exp, string? rating, string? sort, int page = 1)
        {
            ViewBag.Settings = _settingsService.Get();

            // Full CA roster (same as Index, but exposed for filtering)
            var allProfessionals = new List<CaProfessional>
            {
                new() { Id=1,  Name="CA Priya Mehta",     Initials="PM", Designation="FCA", YearsExp=12, City="Mumbai",    Specialisations=["GST","Income Tax","Audit"],           Rating=4.9m, CasesHandled=340, ResponseTime="1h",  MembershipNo="ICAI/2012/PM001", IsVerified=true },
                new() { Id=2,  Name="CA Rajesh Sharma",   Initials="RS", Designation="ACA", YearsExp=8,  City="Delhi",     Specialisations=["Transfer Pricing","FEMA"],            Rating=4.8m, CasesHandled=210, ResponseTime="2h",  MembershipNo="ICAI/2016/RS002", IsVerified=true },
                new() { Id=3,  Name="CA Anita Krishnan",  Initials="AK", Designation="FCA", YearsExp=15, City="Bangalore", Specialisations=["Forensic Audit","ROC","MCA"],          Rating=5.0m, CasesHandled=500, ResponseTime="30m", MembershipNo="ICAI/2009/AK003", IsVerified=true },
                new() { Id=4,  Name="CA Vikram Joshi",    Initials="VJ", Designation="ACA", YearsExp=6,  City="Pune",      Specialisations=["Startup Finance","MCA"],              Rating=4.7m, CasesHandled=180, ResponseTime="3h",  MembershipNo="ICAI/2018/VJ004", IsVerified=true },
                new() { Id=5,  Name="CA Sunita Patel",    Initials="SP", Designation="FCA", YearsExp=10, City="Ahmedabad", Specialisations=["ROC","MCA","GST"],                    Rating=4.6m, CasesHandled=290, ResponseTime="2h",  MembershipNo="ICAI/2014/SP005", IsVerified=true },
                new() { Id=6,  Name="CA Mohit Agarwal",   Initials="MA", Designation="ACA", YearsExp=9,  City="Kolkata",   Specialisations=["Corporate Tax","Transfer Pricing"],   Rating=4.8m, CasesHandled=260, ResponseTime="1h",  MembershipNo="ICAI/2011/MA006", IsVerified=true },
                new() { Id=7,  Name="CA Deepa Nair",      Initials="DN", Designation="ACA", YearsExp=4,  City="Chennai",   Specialisations=["FEMA","RBI Compliance"],              Rating=4.5m, CasesHandled=95,  ResponseTime="4h",  MembershipNo="ICAI/2020/DN007", IsVerified=false },
                new() { Id=8,  Name="CA Arjun Singh",     Initials="AS", Designation="FCA", YearsExp=11, City="Hyderabad", Specialisations=["GST","Audit","Income Tax"],            Rating=4.7m, CasesHandled=310, ResponseTime="2h",  MembershipNo="ICAI/2017/AS008", IsVerified=true },
                new() { Id=9,  Name="CA Kavita Rao",      Initials="KR", Designation="FCA", YearsExp=13, City="Jaipur",    Specialisations=["Tax Litigation","Income Tax"],         Rating=4.9m, CasesHandled=420, ResponseTime="1h",  MembershipNo="ICAI/2015/KR009", IsVerified=true },
                new() { Id=10, Name="CA Nitin Gupta",     Initials="NG", Designation="ACA", YearsExp=7,  City="Lucknow",   Specialisations=["Internal Audit","MCA"],               Rating=4.6m, CasesHandled=145, ResponseTime="3h",  MembershipNo="ICAI/2013/NG010", IsVerified=true },
                new() { Id=11, Name="CA Rahul Sharma",    Initials="RH", Designation="FCA", YearsExp=12, City="Mumbai",    Specialisations=["GST","Income Tax","Company Registration","Audit"], Rating=4.9m, CasesHandled=850, ResponseTime="1h", MembershipNo="ICAI/2012/RH011", IsVerified=true },
                new() { Id=12, Name="CA Amit Verma",      Initials="AV", Designation="FCA", YearsExp=15, City="Delhi",     Specialisations=["GST","Income Tax","Company Registration","Audit"], Rating=5.0m, CasesHandled=1200, ResponseTime="45m", MembershipNo="ICAI/2009/AV012", IsVerified=true },
                new() { Id=13, Name="CA Sanjay Kumar",    Initials="SK", Designation="ACA", YearsExp=6,  City="Bangalore", Specialisations=["Startup Finance","Bookkeeping"],       Rating=4.6m, CasesHandled=120, ResponseTime="2h",  MembershipNo="ICAI/2018/SK013", IsVerified=true },
                new() { Id=14, Name="CA Lakshmi Iyer",    Initials="LI", Designation="FCA", YearsExp=18, City="Chennai",   Specialisations=["Tax Litigation","FEMA","Income Tax"],  Rating=5.0m, CasesHandled=780, ResponseTime="1h",  MembershipNo="ICAI/2006/LI014", IsVerified=true },
                new() { Id=15, Name="CA Ravi Khurana",    Initials="RK", Designation="ACA", YearsExp=5,  City="Pune",      Specialisations=["GST","ROC Filing"],                   Rating=4.5m, CasesHandled=98,  ResponseTime="3h",  MembershipNo="ICAI/2019/RK015", IsVerified=true },
            };

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
        public IActionResult ExpertDetail(string slug)
        {
            ViewBag.Settings = _settingsService.Get();

            // Parse the id from the slug (format: name-slug-{id})
            var parts = slug.Split('-');
            int expertId = 1;
            if (parts.Length > 0 && int.TryParse(parts[^1], out var parsedId))
                expertId = parsedId;

            // Build the professional list (same dataset as FindExpert)
            var allProfessionals = new List<CaProfessional>
            {
                new() { Id=1,  Name="CA Priya Mehta",     Initials="PM", Designation="FCA", YearsExp=12, City="Mumbai",    Specialisations=["GST","Income Tax","Audit","Tax Planning","FEMA"],              Rating=4.9m, CasesHandled=340, ResponseTime="1h",  MembershipNo="ICAI/2012/PM001", IsVerified=true },
                new() { Id=2,  Name="CA Rajesh Sharma",   Initials="RS", Designation="ACA", YearsExp=8,  City="Delhi",     Specialisations=["Transfer Pricing","FEMA","GST","International Tax"],           Rating=4.8m, CasesHandled=210, ResponseTime="2h",  MembershipNo="ICAI/2016/RS002", IsVerified=true },
                new() { Id=3,  Name="CA Anita Krishnan",  Initials="AK", Designation="FCA", YearsExp=15, City="Bangalore", Specialisations=["Forensic Audit","ROC","MCA","Statutory Audit","Compliance"],   Rating=5.0m, CasesHandled=500, ResponseTime="30m", MembershipNo="ICAI/2009/AK003", IsVerified=true },
                new() { Id=4,  Name="CA Vikram Joshi",    Initials="VJ", Designation="ACA", YearsExp=6,  City="Pune",      Specialisations=["Startup Finance","MCA","Equity Structuring","Fundraising"],    Rating=4.7m, CasesHandled=180, ResponseTime="3h",  MembershipNo="ICAI/2018/VJ004", IsVerified=true },
                new() { Id=5,  Name="CA Sunita Patel",    Initials="SP", Designation="FCA", YearsExp=10, City="Ahmedabad", Specialisations=["ROC","MCA","GST","Payroll","Labour Compliance"],               Rating=4.6m, CasesHandled=290, ResponseTime="2h",  MembershipNo="ICAI/2014/SP005", IsVerified=true },
                new() { Id=6,  Name="CA Mohit Agarwal",   Initials="MA", Designation="ACA", YearsExp=9,  City="Kolkata",   Specialisations=["Corporate Tax","Transfer Pricing","Income Tax","Audit"],       Rating=4.8m, CasesHandled=260, ResponseTime="1h",  MembershipNo="ICAI/2011/MA006", IsVerified=true },
                new() { Id=7,  Name="CA Deepa Nair",      Initials="DN", Designation="ACA", YearsExp=4,  City="Chennai",   Specialisations=["FEMA","RBI Compliance","NRI Taxation"],                       Rating=4.5m, CasesHandled=95,  ResponseTime="4h",  MembershipNo="ICAI/2020/DN007", IsVerified=false },
                new() { Id=8,  Name="CA Arjun Singh",     Initials="AS", Designation="FCA", YearsExp=11, City="Hyderabad", Specialisations=["GST","Audit","Income Tax","MIS Reporting"],                   Rating=4.7m, CasesHandled=310, ResponseTime="2h",  MembershipNo="ICAI/2017/AS008", IsVerified=true },
                new() { Id=9,  Name="CA Kavita Rao",      Initials="KR", Designation="FCA", YearsExp=13, City="Jaipur",    Specialisations=["Tax Litigation","Income Tax","ITAT Representation","Appeals"],  Rating=4.9m, CasesHandled=420, ResponseTime="1h",  MembershipNo="ICAI/2015/KR009", IsVerified=true },
                new() { Id=10, Name="CA Nitin Gupta",     Initials="NG", Designation="ACA", YearsExp=7,  City="Lucknow",   Specialisations=["Internal Audit","MCA","Bookkeeping","Accounting"],             Rating=4.6m, CasesHandled=145, ResponseTime="3h",  MembershipNo="ICAI/2013/NG010", IsVerified=true },
                new() { Id=11, Name="CA Rahul Sharma",    Initials="RH", Designation="FCA", YearsExp=12, City="Mumbai",    Specialisations=["GST","Income Tax","Company Registration","Audit","TDS"],       Rating=4.9m, CasesHandled=850, ResponseTime="1h",  MembershipNo="ICAI/2012/RH011", IsVerified=true },
                new() { Id=12, Name="CA Amit Verma",      Initials="AV", Designation="FCA", YearsExp=15, City="Delhi",     Specialisations=["GST","Income Tax","Company Registration","Audit","FEMA"],      Rating=5.0m, CasesHandled=1200, ResponseTime="45m", MembershipNo="ICAI/2009/AV012", IsVerified=true },
                new() { Id=13, Name="CA Sanjay Kumar",    Initials="SK", Designation="ACA", YearsExp=6,  City="Bangalore", Specialisations=["Startup Finance","Bookkeeping","MCA","Virtual CFO"],           Rating=4.6m, CasesHandled=120, ResponseTime="2h",  MembershipNo="ICAI/2018/SK013", IsVerified=true },
                new() { Id=14, Name="CA Lakshmi Iyer",    Initials="LI", Designation="FCA", YearsExp=18, City="Chennai",   Specialisations=["Tax Litigation","FEMA","Income Tax","Cross-border Tax"],       Rating=5.0m, CasesHandled=780, ResponseTime="1h",  MembershipNo="ICAI/2006/LI014", IsVerified=true },
                new() { Id=15, Name="CA Ravi Khurana",    Initials="RK", Designation="ACA", YearsExp=5,  City="Pune",      Specialisations=["GST","ROC Filing","Company Registration"],                    Rating=4.5m, CasesHandled=98,  ResponseTime="3h",  MembershipNo="ICAI/2019/RK015", IsVerified=true },
            };

            var professional = allProfessionals.FirstOrDefault(p => p.Id == expertId)
                               ?? allProfessionals.First();

            return View(professional);
        }

        // GET /sitemap.xml
        [HttpGet("/sitemap.xml")]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
        public ContentResult Sitemap()
        {
            var host = $"{Request.Scheme}://{Request.Host}";
            var xml = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"">
  <url><loc>{host}/</loc><changefreq>daily</changefreq><priority>1.0</priority></url>
  <url><loc>{host}/#professionals</loc><changefreq>daily</changefreq><priority>0.9</priority></url>
  <url><loc>{host}/#features</loc><changefreq>weekly</changefreq><priority>0.8</priority></url>
  <url><loc>{host}/#how-it-works</loc><changefreq>monthly</changefreq><priority>0.7</priority></url>
  <url><loc>{host}/#pricing</loc><changefreq>weekly</changefreq><priority>0.7</priority></url>
  <url><loc>{host}/#faq</loc><changefreq>monthly</changefreq><priority>0.6</priority></url>
  <url><loc>{host}/#contact</loc><changefreq>monthly</changefreq><priority>0.5</priority></url>
</urlset>";
            return Content(xml, "application/xml");
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
