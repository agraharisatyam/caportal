using caportal.Models;
using caportal.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace caportal.Controllers
{
    public class HomeController : Controller
    {
        private readonly SiteSettingsService _settingsService;

        public HomeController(SiteSettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        // GET /site-dynamic.css — returns CSS generated from current settings
        [HttpGet("/site-dynamic.css")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public ContentResult DynamicCss()
        {
            return Content(_settingsService.GenerateCss(), "text/css");
        }

        public IActionResult Index()
        {
            var settings = _settingsService.Get();
            ViewBag.Settings = settings;

            var vm = new HomeViewModel
            {
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
