using caportal.Filters;
using caportal.Models;
using caportal.Models.Entities;
using caportal.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace caportal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class DashboardController : Controller
    {
        // Shared seed data used across all dashboard views
        private static List<CaProfessional> GetAllCAs() =>
        [
            new CaProfessional { Id=1,  Name="CA Priya Mehta",    Initials="PM", Designation="FCA", YearsExp=12, City="Mumbai",    Specialisations=["GST","Income Tax","Audit"],          Rating=4.9m, CasesHandled=340, ResponseTime="1h",  MembershipNo="ICAI/2012/PM001", IsFeatured=true,  JoinedOn=new DateTime(2026,6,1),  Status="Active"    },
            new CaProfessional { Id=2,  Name="CA Rajesh Sharma",  Initials="RS", Designation="ACA", YearsExp=8,  City="Delhi",     Specialisations=["Transfer Pricing","FEMA"],           Rating=4.8m, CasesHandled=210, ResponseTime="2h",  MembershipNo="ICAI/2016/RS002", IsFeatured=true,  JoinedOn=new DateTime(2026,6,5),  Status="Active"    },
            new CaProfessional { Id=3,  Name="CA Anita Krishnan", Initials="AK", Designation="FCA", YearsExp=15, City="Bangalore", Specialisations=["Forensic Audit","ROC"],              Rating=5.0m, CasesHandled=500, ResponseTime="30m", MembershipNo="ICAI/2009/AK003", IsFeatured=true,  JoinedOn=new DateTime(2026,6,10), Status="Active"    },
            new CaProfessional { Id=4,  Name="CA Vikram Joshi",   Initials="VJ", Designation="ACA", YearsExp=6,  City="Pune",      Specialisations=["Startup Finance","MCA"],             Rating=4.7m, CasesHandled=180, ResponseTime="3h",  MembershipNo="ICAI/2018/VJ004", IsFeatured=true,  JoinedOn=new DateTime(2026,6,12), Status="Pending"   },
            new CaProfessional { Id=5,  Name="CA Sunita Patel",   Initials="SP", Designation="FCA", YearsExp=10, City="Ahmedabad", Specialisations=["ROC","MCA","GST"],                   Rating=4.6m, CasesHandled=290, ResponseTime="2h",  MembershipNo="ICAI/2014/SP005", IsFeatured=false, JoinedOn=new DateTime(2026,6,15), Status="Active"    },
            new CaProfessional { Id=6,  Name="CA Mohit Agarwal",  Initials="MA", Designation="ACA", YearsExp=9,  City="Kolkata",   Specialisations=["Corporate Tax","Transfer Pricing"],  Rating=4.8m, CasesHandled=260, ResponseTime="1h",  MembershipNo="ICAI/2011/MA006", IsFeatured=false, JoinedOn=new DateTime(2026,6,18), Status="Active"    },
            new CaProfessional { Id=7,  Name="CA Deepa Nair",     Initials="DN", Designation="ACA", YearsExp=4,  City="Chennai",   Specialisations=["FEMA","RBI Compliance"],             Rating=4.5m, CasesHandled=95,  ResponseTime="4h",  MembershipNo="ICAI/2020/DN007", IsFeatured=false, JoinedOn=new DateTime(2026,6,20), Status="Suspended" },
            new CaProfessional { Id=8,  Name="CA Arjun Singh",    Initials="AS", Designation="FCA", YearsExp=11, City="Hyderabad", Specialisations=["GST","Audit","Income Tax"],          Rating=4.7m, CasesHandled=310, ResponseTime="2h",  MembershipNo="ICAI/2017/AS008", IsFeatured=false, JoinedOn=new DateTime(2026,6,25), Status="Active"    },
            new CaProfessional { Id=9,  Name="CA Kavita Rao",     Initials="KR", Designation="FCA", YearsExp=13, City="Jaipur",    Specialisations=["Tax Litigation","Income Tax"],       Rating=4.9m, CasesHandled=420, ResponseTime="1h",  MembershipNo="ICAI/2015/KR009", IsFeatured=false, JoinedOn=new DateTime(2026,6,28), Status="Pending"   },
            new CaProfessional { Id=10, Name="CA Nitin Gupta",    Initials="NG", Designation="ACA", YearsExp=7,  City="Lucknow",   Specialisations=["Internal Audit","MCA"],              Rating=4.6m, CasesHandled=145, ResponseTime="3h",  MembershipNo="ICAI/2013/NG010", IsFeatured=false, JoinedOn=new DateTime(2026,6,30), Status="Active"    },
        ];

        // GET /ajs  or  /Admin/Dashboard
        public IActionResult Index()
        {
            var cas = GetAllCAs();
            ViewBag.Username      = HttpContext.Session.GetString("AdminUsername") ?? "ajs";
            ViewBag.TotalCAs      = cas.Count;
            ViewBag.ActiveCAs     = cas.Count(c => c.Status == "Active");
            ViewBag.PendingCAs    = cas.Count(c => c.Status == "Pending");
            ViewBag.SuspendedCAs  = cas.Count(c => c.Status == "Suspended");
            ViewBag.AvgRating     = cas.Average(c => c.Rating).ToString("F1");
            ViewBag.RecentCAs     = cas.OrderByDescending(c => c.JoinedOn).Take(10).ToList();

            // Rich Dashboard KPIs matching reference mockup
            ViewBag.TodayRevenue = "₹ 24,58,760";
            ViewBag.TodaySales = "₹ 1,25,430";
            ViewBag.TotalLeads = "1,258";
            ViewBag.TotalOrders = "856";
            ViewBag.TotalCustomers = "3,452";
            ViewBag.PendingDocuments = "320";
            ViewBag.TodayAppointments = "28";

            // Rich Bottom metrics
            ViewBag.ConversionRate = "24.6%";
            ViewBag.AvgOrderValue = "₹ 6,782";
            ViewBag.CustomerSatisfaction = "4.8/5";
            ViewBag.RepeatCustomers = "68.4%";

            // Mock Recent Orders
            ViewBag.RecentOrders = new List<DashboardOrder>
            {
                new() { OrderId = "ORD-2505101", Customer = "Ravi Sharma", Service = "GST Registration", Amount = "₹ 2,499", Status = "Processing", ColorClass = "bg-warning" },
                new() { OrderId = "ORD-2501783", Customer = "ABC Pvt. Ltd.", Service = "Private Limited Co.", Amount = "₹ 12,999", Status = "Processing", ColorClass = "bg-warning" },
                new() { OrderId = "ORD-2501556", Customer = "Sneha Verma", Service = "Trademark Registration", Amount = "₹ 1,999", Status = "Document Pending", ColorClass = "bg-info" },
                new() { OrderId = "ORD-2501518", Customer = "Sunrise Enterprises", Service = "GST Return Filing", Amount = "₹ 1,999", Status = "Completed", ColorClass = "bg-success" },
                new() { OrderId = "ORD-2501157", Customer = "Karan Mehta", Service = "LLP Registration", Amount = "₹ 5,999", Status = "Processing", ColorClass = "bg-warning" }
            };
            return View();
        }

        // GET /Admin/Dashboard/CaList
        public IActionResult CaList()
        {
            var cas = GetAllCAs();
            ViewBag.CAs           = cas;
            ViewBag.TotalCAs      = cas.Count;
            ViewBag.ActiveCAs     = cas.Count(c => c.Status == "Active");
            ViewBag.PendingCAs    = cas.Count(c => c.Status == "Pending");
            ViewBag.SuspendedCAs  = cas.Count(c => c.Status == "Suspended");
            return View();
        }

        // GET /Admin/Dashboard/Clients
        public IActionResult Clients()
        {

            var clients = new List<Client>
            {
                new Client { Id=1,  CompanyName="TechVentures Pvt. Ltd.", Type="Corporate",  ContactEmail="suresh@techventures.in",   City="Bangalore",  AssignedCA="CA Priya Mehta",    Service="GST & Tax",      Status="Active",   RegisteredOn=new DateTime(2026,1,15) },
                new Client { Id=2,  CompanyName="GreenLeaf Foods",        Type="Startup",    ContactEmail="nisha@greenleaf.in",       City="Mumbai",     AssignedCA="CA Anita Krishnan", Service="Audit",          Status="Active",   RegisteredOn=new DateTime(2026,2,3)  },
                new Client { Id=3,  CompanyName="Apex Exports Pvt. Ltd.", Type="Corporate",  ContactEmail="mohan@apexexports.com",    City="Surat",      AssignedCA="CA Rajesh Sharma",  Service="FEMA & Tax",     Status="Active",   RegisteredOn=new DateTime(2026,2,20) },
                new Client { Id=4,  CompanyName="Sunrise Real Estate",    Type="SME",        ContactEmail="raj@sunrise.in",           City="Delhi",      AssignedCA="CA Vikram Joshi",   Service="ROC",            Status="Pending",  RegisteredOn=new DateTime(2026,3,10) },
                new Client { Id=5,  CompanyName="Kavya Textiles",         Type="SME",        ContactEmail="kavya@kavyatex.in",        City="Coimbatore", AssignedCA="CA Mohit Agarwal",  Service="GST",            Status="Active",   RegisteredOn=new DateTime(2026,3,22) },
                new Client { Id=6,  CompanyName="NextGen Software",       Type="Startup",    ContactEmail="ceo@nextgensw.io",         City="Hyderabad",  AssignedCA="CA Ishita Verma",   Service="Tax Planning",   Status="Active",   RegisteredOn=new DateTime(2026,4,5)  },
                new Client { Id=7,  CompanyName="Mr. Amit Verma",         Type="Individual", ContactEmail="amit.v@gmail.com",         City="Jaipur",     AssignedCA="CA Kavita Rao",     Service="ITR Filing",     Status="Active",   RegisteredOn=new DateTime(2026,4,18) },
                new Client { Id=8,  CompanyName="Bharat Motors",          Type="Corporate",  ContactEmail="accounts@bharatmotors.in", City="Pune",       AssignedCA="CA Nitin Gupta",    Service="Internal Audit", Status="Inactive", RegisteredOn=new DateTime(2026,5,2)  },
                new Client { Id=9,  CompanyName="SkyHigh Logistics",      Type="SME",        ContactEmail="cfo@skyhigh.in",           City="Ahmedabad",  AssignedCA="CA Sunita Patel",   Service="MCA",            Status="Active",   RegisteredOn=new DateTime(2026,5,14) },
                new Client { Id=10, CompanyName="Dr. Preethi Suresh",     Type="Individual", ContactEmail="preethi.s@outlook.com",    City="Chennai",    AssignedCA="CA Rekha Desai",    Service="ITR",            Status="Active",   RegisteredOn=new DateTime(2026,5,28) },
                new Client { Id=11, CompanyName="Rudra Constructions",    Type="Corporate",  ContactEmail="md@rudracorp.in",          City="Nagpur",     AssignedCA="CA Arjun Singh",    Service="GST Audit",      Status="Active",   RegisteredOn=new DateTime(2026,6,3)  },
                new Client { Id=12, CompanyName="PixelPro Studios",       Type="Startup",    ContactEmail="hello@pixelpro.in",        City="Kochi",      AssignedCA="CA Ravi Menon",     Service="Incorporation",  Status="Pending",  RegisteredOn=new DateTime(2026,6,19) },
            };

            ViewBag.Clients       = clients;
            ViewBag.TotalClients  = clients.Count;
            ViewBag.ActiveClients = clients.Count(c => c.Status == "Active");
            ViewBag.PendingClients= clients.Count(c => c.Status == "Pending");
            ViewBag.TypeBreakdown = clients.GroupBy(c => c.Type)
                                           .Select(g => new { Type = g.Key, Count = g.Count() })
                                           .OrderByDescending(x => x.Count)
                                           .ToList<dynamic>();
            return View();
        }
    }
}
