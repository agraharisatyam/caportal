using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using caportal.Models.Entities;

namespace caportal.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            var logger = scope.ServiceProvider.GetService<ILogger<ApplicationDbContext>>();

            try
            {
                using var db = dbFactory.CreateDbContext();
                db.Database.SetCommandTimeout(60);

                // ── 1. Self-healing schema generation ──────────────────────────────────
                await EnsureTablesExistAsync(db);

                // ── 2. Seed Default Data ───────────────────────────────────────────────
                await SeedCaProfessionalsAsync(db);
                await SeedClientsAsync(db);
                await SeedDashboardOrdersAsync(db);
                await SeedClientRequestsAsync(db);
                await SeedBlogPostsAsync(db);
                await SeedFaqsAsync(db);
                await SeedTestimonialsAsync(db);
                await SeedPricingPlansAsync(db);
                await SeedContentPagesAsync(db);

                logger?.LogInformation("DbInitializer: Database schema verified and seed data populated successfully.");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "DbInitializer: Error while initializing database.");
            }
        }

        private static async Task EnsureTablesExistAsync(ApplicationDbContext db)
        {
            var ddl = @"
            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'CaProfessionals')
            BEGIN
                CREATE TABLE [CaProfessionals] (
                    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    [Name] NVARCHAR(150) NOT NULL,
                    [Initials] NVARCHAR(10) NOT NULL DEFAULT '',
                    [Designation] NVARCHAR(50) NOT NULL DEFAULT 'FCA',
                    [YearsExp] INT NOT NULL DEFAULT 5,
                    [City] NVARCHAR(100) NOT NULL DEFAULT '',
                    [Specialisations] NVARCHAR(MAX) NOT NULL DEFAULT '',
                    [Rating] DECIMAL(3,2) NOT NULL DEFAULT 4.8,
                    [CasesHandled] INT NOT NULL DEFAULT 100,
                    [ResponseTime] NVARCHAR(50) NOT NULL DEFAULT '1h',
                    [MembershipNo] NVARCHAR(50) NOT NULL DEFAULT '',
                    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Active',
                    [IsVerified] BIT NOT NULL DEFAULT 1,
                    [IsFeatured] BIT NOT NULL DEFAULT 0,
                    [ImagePath] NVARCHAR(500) NOT NULL DEFAULT '',
                    [Bio] NVARCHAR(MAX) NOT NULL DEFAULT '',
                    [ConsultationFee] INT NOT NULL DEFAULT 499,
                    [Phone] NVARCHAR(50) NOT NULL DEFAULT '',
                    [Email] NVARCHAR(100) NOT NULL DEFAULT '',
                    [DisplayOrder] INT NOT NULL DEFAULT 0,
                    [JoinedOn] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
                );
            END;

            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'Clients')
            BEGIN
                CREATE TABLE [Clients] (
                    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    [CompanyName] NVARCHAR(200) NOT NULL,
                    [Type] NVARCHAR(50) NOT NULL DEFAULT 'Corporate',
                    [ContactPerson] NVARCHAR(100) NOT NULL DEFAULT '',
                    [ContactEmail] NVARCHAR(150) NOT NULL DEFAULT '',
                    [ContactPhone] NVARCHAR(50) NOT NULL DEFAULT '',
                    [GstNumber] NVARCHAR(50) NOT NULL DEFAULT '',
                    [PanNumber] NVARCHAR(50) NOT NULL DEFAULT '',
                    [Address] NVARCHAR(500) NOT NULL DEFAULT '',
                    [City] NVARCHAR(100) NOT NULL DEFAULT '',
                    [AssignedCA] NVARCHAR(150) NOT NULL DEFAULT '',
                    [Service] NVARCHAR(150) NOT NULL DEFAULT '',
                    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Active',
                    [RegisteredOn] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
                );
            END;

            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'DashboardOrders')
            BEGIN
                CREATE TABLE [DashboardOrders] (
                    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    [OrderId] NVARCHAR(50) NOT NULL,
                    [Customer] NVARCHAR(150) NOT NULL,
                    [ClientEmail] NVARCHAR(150) NOT NULL DEFAULT '',
                    [ClientPhone] NVARCHAR(50) NOT NULL DEFAULT '',
                    [Service] NVARCHAR(150) NOT NULL,
                    [Amount] NVARCHAR(50) NOT NULL,
                    [AmountValue] DECIMAL(18,2) NOT NULL DEFAULT 0,
                    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Processing',
                    [PaymentStatus] NVARCHAR(50) NOT NULL DEFAULT 'Paid',
                    [ColorClass] NVARCHAR(50) NOT NULL DEFAULT 'bg-warning',
                    [AssignedCA] NVARCHAR(150) NOT NULL DEFAULT '',
                    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
                );
            END;

            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'ClientRequests')
            BEGIN
                CREATE TABLE [ClientRequests] (
                    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    [ClientName] NVARCHAR(150) NOT NULL,
                    [ClientEmail] NVARCHAR(150) NOT NULL DEFAULT '',
                    [ClientPhone] NVARCHAR(50) NOT NULL DEFAULT '',
                    [City] NVARCHAR(100) NOT NULL DEFAULT '',
                    [ClientType] NVARCHAR(50) NOT NULL DEFAULT 'Individual',
                    [ServiceRequired] NVARCHAR(150) NOT NULL DEFAULT '',
                    [AssignedCA] NVARCHAR(150) NOT NULL DEFAULT 'Unassigned',
                    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
                    [Description] NVARCHAR(MAX) NOT NULL DEFAULT '',
                    [PreferredTime] NVARCHAR(100) NOT NULL DEFAULT '',
                    [Source] NVARCHAR(50) NOT NULL DEFAULT 'Contact Form',
                    [RequestedOn] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
                );
            END;

            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'BlogPosts')
            BEGIN
                CREATE TABLE [BlogPosts] (
                    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    [Title] NVARCHAR(300) NOT NULL,
                    [Slug] NVARCHAR(300) NOT NULL,
                    [Category] NVARCHAR(100) NOT NULL DEFAULT 'Taxation',
                    [Excerpt] NVARCHAR(1000) NOT NULL DEFAULT '',
                    [Content] NVARCHAR(MAX) NOT NULL DEFAULT '',
                    [FeaturedImagePath] NVARCHAR(500) NOT NULL DEFAULT '',
                    [AuthorName] NVARCHAR(100) NOT NULL DEFAULT '',
                    [AuthorRole] NVARCHAR(100) NOT NULL DEFAULT '',
                    [AuthorAvatar] NVARCHAR(500) NOT NULL DEFAULT '',
                    [PublishedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                    [IsPublished] BIT NOT NULL DEFAULT 1,
                    [ViewsCount] INT NOT NULL DEFAULT 0,
                    [ReadTimeMinutes] INT NOT NULL DEFAULT 5,
                    [Tags] NVARCHAR(MAX) NOT NULL DEFAULT '',
                    [MetaTitle] NVARCHAR(200) NOT NULL DEFAULT '',
                    [MetaDescription] NVARCHAR(500) NOT NULL DEFAULT '',
                    [MetaKeywords] NVARCHAR(500) NOT NULL DEFAULT ''
                );
            END;

            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'Faqs')
            BEGIN
                CREATE TABLE [Faqs] (
                    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    [Question] NVARCHAR(500) NOT NULL,
                    [Answer] NVARCHAR(MAX) NOT NULL,
                    [Category] NVARCHAR(100) NOT NULL DEFAULT 'General',
                    [DisplayOrder] INT NOT NULL DEFAULT 0,
                    [IsActive] BIT NOT NULL DEFAULT 1
                );
            END;

            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'Testimonials')
            BEGIN
                CREATE TABLE [Testimonials] (
                    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    [Text] NVARCHAR(MAX) NOT NULL,
                    [AuthorName] NVARCHAR(150) NOT NULL,
                    [AuthorRole] NVARCHAR(150) NOT NULL DEFAULT '',
                    [Initials] NVARCHAR(10) NOT NULL DEFAULT '',
                    [Rating] INT NOT NULL DEFAULT 5,
                    [AvatarPath] NVARCHAR(500) NOT NULL DEFAULT '',
                    [DisplayOrder] INT NOT NULL DEFAULT 0,
                    [IsActive] BIT NOT NULL DEFAULT 1
                );
            END;

            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'PricingPlans')
            BEGIN
                CREATE TABLE [PricingPlans] (
                    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    [PlanName] NVARCHAR(100) NOT NULL,
                    [PriceDisplay] NVARCHAR(50) NOT NULL,
                    [BillingCycle] NVARCHAR(50) NOT NULL DEFAULT '/month',
                    [Description] NVARCHAR(500) NOT NULL DEFAULT '',
                    [Features] NVARCHAR(MAX) NOT NULL DEFAULT '',
                    [IsPopular] BIT NOT NULL DEFAULT 0,
                    [DisplayOrder] INT NOT NULL DEFAULT 0,
                    [IsActive] BIT NOT NULL DEFAULT 1
                );
            END;

            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'ContentPages')
            BEGIN
                CREATE TABLE [ContentPages] (
                    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    [Title] NVARCHAR(200) NOT NULL,
                    [Slug] NVARCHAR(200) NOT NULL,
                    [MetaDescription] NVARCHAR(500) NOT NULL DEFAULT '',
                    [HtmlContent] NVARCHAR(MAX) NOT NULL DEFAULT '',
                    [IsPublished] BIT NOT NULL DEFAULT 1,
                    [LastUpdated] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
                );
            END;

            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'NewsletterSubscribers')
            BEGIN
                CREATE TABLE [NewsletterSubscribers] (
                    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    [Email] NVARCHAR(200) NOT NULL,
                    [SubscribedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                    [IsActive] BIT NOT NULL DEFAULT 1
                );
            END;
            ";

            await db.Database.ExecuteSqlRawAsync(ddl);
        }

        private static async Task SeedCaProfessionalsAsync(ApplicationDbContext db)
        {
            if (await db.CaProfessionals.AnyAsync()) return;

            var items = new List<CaProfessional>
            {
                new() { Name="CA Priya Mehta",    Initials="PM", Designation="FCA", YearsExp=12, City="Mumbai",    Specialisations=["GST","Income Tax","Audit","Tax Planning","FEMA"],              Rating=4.9m, CasesHandled=340, ResponseTime="1h",  MembershipNo="ICAI/2012/PM001", IsFeatured=true,  IsVerified=true, JoinedOn=new DateTime(2026,6,1),  Status="Active",  ImagePath="/images/ca/ca-priya-mehta.svg", ConsultationFee=799, Phone="+91 98201 11223", Email="ca.priya@cacharteredcampus.com", Bio="Senior FCA with 12+ years of experience in GST audits, corporate taxation, and litigation." },
                new() { Name="CA Rajesh Sharma",  Initials="RS", Designation="ACA", YearsExp=8,  City="Delhi",     Specialisations=["Transfer Pricing","FEMA","GST","International Tax"],           Rating=4.8m, CasesHandled=210, ResponseTime="2h",  MembershipNo="ICAI/2016/RS002", IsFeatured=true,  IsVerified=true, JoinedOn=new DateTime(2026,6,5),  Status="Active",  ImagePath="/images/ca/ca-rajesh-sharma.svg", ConsultationFee=699, Phone="+91 98110 22334", Email="ca.rajesh@cacharteredcampus.com", Bio="Specialist in transfer pricing audits, international cross-border transactions, and FEMA." },
                new() { Name="CA Anita Krishnan", Initials="AK", Designation="FCA", YearsExp=15, City="Bangalore", Specialisations=["Forensic Audit","ROC","MCA","Statutory Audit","Compliance"],   Rating=5.0m, CasesHandled=500, ResponseTime="30m", MembershipNo="ICAI/2009/AK003", IsFeatured=true,  IsVerified=true, JoinedOn=new DateTime(2026,6,10), Status="Active",  ImagePath="/images/ca/ca-anita-krishnan.svg", ConsultationFee=999, Phone="+91 98450 33445", Email="ca.anita@cacharteredcampus.com", Bio="Expert in corporate law advisory, forensic audit, ROC filings, and startup board compliance." },
                new() { Name="CA Vikram Joshi",   Initials="VJ", Designation="ACA", YearsExp=6,  City="Pune",      Specialisations=["Startup Finance","MCA","Equity Structuring","Fundraising"],    Rating=4.7m, CasesHandled=180, ResponseTime="3h",  MembershipNo="ICAI/2018/VJ004", IsFeatured=true,  IsVerified=true, JoinedOn=new DateTime(2026,6,12), Status="Active",   ImagePath="/images/ca/ca-vikram-joshi.svg", ConsultationFee=599, Phone="+91 98220 44556", Email="ca.vikram@cacharteredcampus.com", Bio="Startup finance advisor aiding tech startups through seed rounds, ESOPs, and valuations." },
                new() { Name="CA Sunita Patel",   Initials="SP", Designation="FCA", YearsExp=10, City="Ahmedabad", Specialisations=["ROC","MCA","GST","Payroll","Labour Compliance"],               Rating=4.6m, CasesHandled=290, ResponseTime="2h",  MembershipNo="ICAI/2014/SP005", IsFeatured=false, IsVerified=true, JoinedOn=new DateTime(2026,6,15), Status="Active",  ImagePath="/images/ca/ca-priya-mehta.svg", ConsultationFee=649, Phone="+91 98790 55667", Email="ca.sunita@cacharteredcampus.com", Bio="Specializing in business structuring, annual ROC filings, and comprehensive corporate payroll." },
                new() { Name="CA Mohit Agarwal",  Initials="MA", Designation="ACA", YearsExp=9,  City="Kolkata",   Specialisations=["Corporate Tax","Transfer Pricing","Income Tax","Audit"],       Rating=4.8m, CasesHandled=260, ResponseTime="1h",  MembershipNo="ICAI/2011/MA006", IsFeatured=false, IsVerified=true, JoinedOn=new DateTime(2026,6,18), Status="Active",  ImagePath="/images/ca/ca-rajesh-sharma.svg", ConsultationFee=699, Phone="+91 98300 66778", Email="ca.mohit@cacharteredcampus.com", Bio="Handling complex corporate income tax assessments, appeals, and comprehensive statutory audits." },
                new() { Name="CA Deepa Nair",     Initials="DN", Designation="ACA", YearsExp=4,  City="Chennai",   Specialisations=["FEMA","RBI Compliance","NRI Taxation"],                       Rating=4.5m, CasesHandled=95,  ResponseTime="4h",  MembershipNo="ICAI/2020/DN007", IsFeatured=false, IsVerified=true, JoinedOn=new DateTime(2026,6,20), Status="Active", ImagePath="/images/ca/ca-anita-krishnan.svg", ConsultationFee=499, Phone="+91 98400 77889", Email="ca.deepa@cacharteredcampus.com", Bio="Focusing on NRI taxation, foreign remittance certification (Form 15CA/CB), and FEMA compliances." },
                new() { Name="CA Arjun Singh",    Initials="AS", Designation="FCA", YearsExp=11, City="Hyderabad", Specialisations=["GST","Audit","Income Tax","MIS Reporting"],                   Rating=4.7m, CasesHandled=310, ResponseTime="2h",  MembershipNo="ICAI/2017/AS008", IsFeatured=false, IsVerified=true, JoinedOn=new DateTime(2026,6,25), Status="Active",  ImagePath="/images/ca/ca-vikram-joshi.svg", ConsultationFee=749, Phone="+91 98490 88990", Email="ca.arjun@cacharteredcampus.com", Bio="Experienced in manufacturing and enterprise GST implementation, departmental audits, and direct taxes." },
                new() { Name="CA Kavita Rao",     Initials="KR", Designation="FCA", YearsExp=13, City="Jaipur",    Specialisations=["Tax Litigation","Income Tax","ITAT Representation","Appeals"],  Rating=4.9m, CasesHandled=420, ResponseTime="1h",  MembershipNo="ICAI/2015/KR009", IsFeatured=false, IsVerified=true, JoinedOn=new DateTime(2026,6,28), Status="Active",   ImagePath="/images/ca/ca-priya-mehta.svg", ConsultationFee=849, Phone="+91 98290 99001", Email="ca.kavita@cacharteredcampus.com", Bio="High-stakes income tax appeal and litigation specialist with strong representations before the ITAT." },
                new() { Name="CA Nitin Gupta",    Initials="NG", Designation="ACA", YearsExp=7,  City="Lucknow",   Specialisations=["Internal Audit","MCA","Bookkeeping","Accounting"],             Rating=4.6m, CasesHandled=145, ResponseTime="3h",  MembershipNo="ICAI/2013/NG010", IsFeatured=false, IsVerified=true, JoinedOn=new DateTime(2026,6,30), Status="Active",  ImagePath="/images/ca/ca-rajesh-sharma.svg", ConsultationFee=549, Phone="+91 98390 10112", Email="ca.nitin@cacharteredcampus.com", Bio="Assisting growing businesses with bookkeeping, internal financial controls, and MCA regularizations." },
                new() { Name="CA Rahul Sharma",   Initials="RH", Designation="FCA", YearsExp=12, City="Mumbai",    Specialisations=["GST","Income Tax","Company Registration","Audit","TDS"],       Rating=4.9m, CasesHandled=850, ResponseTime="1h",  MembershipNo="ICAI/2012/RH011", IsFeatured=false, IsVerified=true, JoinedOn=new DateTime(2026,5,15), Status="Active",  ImagePath="/images/ca/ca-rajesh-sharma.svg", ConsultationFee=799, Phone="+91 98200 12345", Email="ca.rahul@cacharteredcampus.com", Bio="Specialized in business registrations, GST filings, and comprehensive corporate compliance suites." },
                new() { Name="CA Amit Verma",     Initials="AV", Designation="FCA", YearsExp=15, City="Delhi",     Specialisations=["GST","Income Tax","Company Registration","Audit","FEMA"],      Rating=5.0m, CasesHandled=1200, ResponseTime="45m", MembershipNo="ICAI/2009/AV012", IsFeatured=false, IsVerified=true, JoinedOn=new DateTime(2026,5,20), Status="Active",  ImagePath="/images/ca/ca-rajesh-sharma.svg", ConsultationFee=999, Phone="+91 98100 23456", Email="ca.amit@cacharteredcampus.com", Bio="Top-rated tax and corporate audit advisor with 15 years experience serving Fortune 500 & MNCs." },
                new() { Name="CA Sanjay Kumar",   Initials="SK", Designation="ACA", YearsExp=6,  City="Bangalore", Specialisations=["Startup Finance","Bookkeeping","MCA","Virtual CFO"],           Rating=4.6m, CasesHandled=120, ResponseTime="2h",  MembershipNo="ICAI/2018/SK013", IsFeatured=false, IsVerified=true, JoinedOn=new DateTime(2026,5,25), Status="Active",  ImagePath="/images/ca/ca-vikram-joshi.svg", ConsultationFee=599, Phone="+91 98450 34567", Email="ca.sanjay@cacharteredcampus.com", Bio="Virtual CFO services, financial modeling, and early-stage startup compliance setup." },
                new() { Name="CA Lakshmi Iyer",   Initials="LI", Designation="FCA", YearsExp=18, City="Chennai",   Specialisations=["Tax Litigation","FEMA","Income Tax","Cross-border Tax"],       Rating=5.0m, CasesHandled=780, ResponseTime="1h",  MembershipNo="ICAI/2006/LI014", IsFeatured=false, IsVerified=true, JoinedOn=new DateTime(2026,5,28), Status="Active",  ImagePath="/images/ca/ca-anita-krishnan.svg", ConsultationFee=1099, Phone="+91 98400 45678", Email="ca.lakshmi@cacharteredcampus.com", Bio="Veteran tax lawyer and FCA specializing in cross-border tax disputes and High Court tax appeals." },
                new() { Name="CA Ravi Khurana",   Initials="RK", Designation="ACA", YearsExp=5,  City="Pune",      Specialisations=["GST","ROC Filing","Company Registration"],                    Rating=4.5m, CasesHandled=98,  ResponseTime="3h",  MembershipNo="ICAI/2019/RK015", IsFeatured=false, IsVerified=true, JoinedOn=new DateTime(2026,6,2),  Status="Active",  ImagePath="/images/ca/ca-vikram-joshi.svg", ConsultationFee=499, Phone="+91 98220 56789", Email="ca.ravi@cacharteredcampus.com", Bio="Helping proprietorships, partnerships, and private companies with day-to-day statutory filings." }
            };

            await db.CaProfessionals.AddRangeAsync(items);
            await db.SaveChangesAsync();
        }

        private static async Task SeedClientsAsync(ApplicationDbContext db)
        {
            if (await db.Clients.AnyAsync()) return;

            var items = new List<Client>
            {
                new() { CompanyName="TechVentures Pvt. Ltd.", Type="Corporate",  ContactPerson="Suresh Kumar", ContactEmail="suresh@techventures.in",   ContactPhone="+91 98200 11221", City="Bangalore",  AssignedCA="CA Priya Mehta",    Service="GST & Tax",      Status="Active",   RegisteredOn=new DateTime(2026,1,15) },
                new() { CompanyName="GreenLeaf Foods",        Type="Startup",    ContactPerson="Nisha Rao",     ContactEmail="nisha@greenleaf.in",       ContactPhone="+91 98200 22332", City="Mumbai",     AssignedCA="CA Anita Krishnan", Service="Audit",          Status="Active",   RegisteredOn=new DateTime(2026,2,3)  },
                new() { CompanyName="Apex Exports Pvt. Ltd.", Type="Corporate",  ContactPerson="Mohan Verma",   ContactEmail="mohan@apexexports.com",    ContactPhone="+91 98200 33443", City="Surat",      AssignedCA="CA Rajesh Sharma",  Service="FEMA & Tax",     Status="Active",   RegisteredOn=new DateTime(2026,2,20) },
                new() { CompanyName="Sunrise Real Estate",    Type="SME",        ContactPerson="Raj Patel",     ContactEmail="raj@sunrise.in",           ContactPhone="+91 98200 44554", City="Delhi",      AssignedCA="CA Vikram Joshi",   Service="ROC",            Status="Active",   RegisteredOn=new DateTime(2026,3,10) },
                new() { CompanyName="Kavya Textiles",         Type="SME",        ContactPerson="Kavya Sundaram",ContactEmail="kavya@kavyatex.in",        ContactPhone="+91 98200 55665", City="Coimbatore", AssignedCA="CA Mohit Agarwal",  Service="GST",            Status="Active",   RegisteredOn=new DateTime(2026,3,22) },
                new() { CompanyName="NextGen Software",       Type="Startup",    ContactPerson="Deepak Shah",   ContactEmail="ceo@nextgensw.io",         ContactPhone="+91 98200 66776", City="Hyderabad",  AssignedCA="CA Priya Mehta",    Service="Tax Planning",   Status="Active",   RegisteredOn=new DateTime(2026,4,5)  },
                new() { CompanyName="Mr. Amit Verma",         Type="Individual", ContactPerson="Amit Verma",    ContactEmail="amit.v@gmail.com",         ContactPhone="+91 98200 77887", City="Jaipur",     AssignedCA="CA Kavita Rao",     Service="ITR Filing",     Status="Active",   RegisteredOn=new DateTime(2026,4,18) },
                new() { CompanyName="Bharat Motors",          Type="Corporate",  ContactPerson="Alok Jain",     ContactEmail="accounts@bharatmotors.in", ContactPhone="+91 98200 88998", City="Pune",       AssignedCA="CA Nitin Gupta",    Service="Internal Audit", Status="Active",   RegisteredOn=new DateTime(2026,5,2)  },
                new() { CompanyName="SkyHigh Logistics",      Type="SME",        ContactPerson="Sameer Khan",   ContactEmail="cfo@skyhigh.in",           ContactPhone="+91 98200 99009", City="Ahmedabad",  AssignedCA="CA Sunita Patel",   Service="MCA",            Status="Active",   RegisteredOn=new DateTime(2026,5,14) },
                new() { CompanyName="Dr. Preethi Suresh",     Type="Individual", ContactPerson="Dr. Preethi",   ContactEmail="preethi.s@outlook.com",    ContactPhone="+91 98200 10101", City="Chennai",    AssignedCA="CA Lakshmi Iyer",   Service="ITR",            Status="Active",   RegisteredOn=new DateTime(2026,5,28) },
                new() { CompanyName="Rudra Constructions",    Type="Corporate",  ContactPerson="Rudra Dev",     ContactEmail="md@rudracorp.in",          ContactPhone="+91 98200 20202", City="Nagpur",     AssignedCA="CA Arjun Singh",    Service="GST Audit",      Status="Active",   RegisteredOn=new DateTime(2026,6,3)  },
                new() { CompanyName="PixelPro Studios",       Type="Startup",    ContactPerson="Rohan Das",     ContactEmail="hello@pixelpro.in",        ContactPhone="+91 98200 30303", City="Kochi",      AssignedCA="CA Vikram Joshi",   Service="Incorporation",  Status="Pending",  RegisteredOn=new DateTime(2026,6,19) }
            };

            await db.Clients.AddRangeAsync(items);
            await db.SaveChangesAsync();
        }

        private static async Task SeedDashboardOrdersAsync(ApplicationDbContext db)
        {
            if (await db.DashboardOrders.AnyAsync()) return;

            var items = new List<DashboardOrder>
            {
                new() { OrderId="ORD-2505101", Customer="Ravi Sharma",         ClientEmail="ravi.s@gmail.com",     ClientPhone="+91 98201 22331", Service="GST Registration",          Amount="₹ 2,499", AmountValue=2499, Status="Processing",        PaymentStatus="Paid", ColorClass="bg-warning", AssignedCA="CA Priya Mehta",    CreatedAt=DateTime.UtcNow.AddHours(-2) },
                new() { OrderId="ORD-2501783", Customer="ABC Pvt. Ltd.",       ClientEmail="finance@abc.com",      ClientPhone="+91 98202 33442", Service="Private Limited Co.",       Amount="₹ 12,999", AmountValue=12999, Status="Processing",       PaymentStatus="Paid", ColorClass="bg-warning", AssignedCA="CA Anita Krishnan", CreatedAt=DateTime.UtcNow.AddHours(-5) },
                new() { OrderId="ORD-2501556", Customer="Sneha Verma",         ClientEmail="sneha.v@greenleaf.in", ClientPhone="+91 98203 44553", Service="Trademark Registration",   Amount="₹ 1,999", AmountValue=1999, Status="Document Pending",  PaymentStatus="Paid", ColorClass="bg-info",    AssignedCA="CA Rajesh Sharma",  CreatedAt=DateTime.UtcNow.AddDays(-1) },
                new() { OrderId="ORD-2501518", Customer="Sunrise Enterprises", ClientEmail="info@sunrise.in",      ClientPhone="+91 98204 55664", Service="GST Return Filing",        Amount="₹ 1,999", AmountValue=1999, Status="Completed",         PaymentStatus="Paid", ColorClass="bg-success", AssignedCA="CA Priya Mehta",    CreatedAt=DateTime.UtcNow.AddDays(-2) },
                new() { OrderId="ORD-2501157", Customer="Karan Mehta",         ClientEmail="karan@techvent.in",    ClientPhone="+91 98205 66775", Service="LLP Registration",          Amount="₹ 5,999", AmountValue=5999, Status="Processing",        PaymentStatus="Paid", ColorClass="bg-warning", AssignedCA="CA Vikram Joshi",   CreatedAt=DateTime.UtcNow.AddDays(-3) }
            };

            await db.DashboardOrders.AddRangeAsync(items);
            await db.SaveChangesAsync();
        }

        private static async Task SeedClientRequestsAsync(ApplicationDbContext db)
        {
            if (await db.ClientRequests.AnyAsync()) return;

            var items = new List<ClientRequest>
            {
                new() { ClientName="Rahul Sharma", ClientEmail="rahul@techventures.in", ClientPhone="+91 98765 11111", City="Mumbai",    ClientType="Corporate", ServiceRequired="GST Return Filing", AssignedCA="CA Priya Mehta",    Status="Assigned",  Description="Need urgent GSTR-1 and GSTR-3B filing for Q2.", Source="Contact Form",   RequestedOn=DateTime.UtcNow.AddHours(-3) },
                new() { ClientName="Amit Verma",   ClientEmail="amit.v@gmail.com",     ClientPhone="+91 98765 22222", City="Delhi",     ClientType="Individual",ServiceRequired="Income Tax Return (ITR)", AssignedCA="CA Rajesh Sharma", Status="Pending",   Description="ITR-3 filing for proprietorship business.",    Source="Expert Profile", RequestedOn=DateTime.UtcNow.AddHours(-5) },
                new() { ClientName="Sneha Gupta",  ClientEmail="sneha@greenleaf.in",   ClientPhone="+91 98765 33333", City="Bangalore", ClientType="Startup",   ServiceRequired="Private Limited Incorporation", AssignedCA="CA Anita Krishnan", Status="Completed", Description="Need DIN, DSC, and name approval for new startup.", Source="Homepage",      RequestedOn=DateTime.UtcNow.AddDays(-1) },
                new() { ClientName="Karan Mehta",  ClientEmail="karan@sunrise.in",     ClientPhone="+91 98765 44444", City="Pune",      ClientType="SME",       ServiceRequired="Statutory Audit", AssignedCA="CA Vikram Joshi",   Status="Assigned",  Description="Annual financial audit for FY 2025-26.",       Source="Contact Form",   RequestedOn=DateTime.UtcNow.AddDays(-2) },
                new() { ClientName="Priya Singh",  ClientEmail="priya@skyhigh.in",     ClientPhone="+91 98765 55555", City="Ahmedabad", ClientType="Startup",   ServiceRequired="Trademark Registration", AssignedCA="Unassigned",      Status="Pending",   Description="Brand name logo trademark search and filing.",  Source="Expert Profile", RequestedOn=DateTime.UtcNow.AddDays(-3) }
            };

            await db.ClientRequests.AddRangeAsync(items);
            await db.SaveChangesAsync();
        }

        private static async Task SeedBlogPostsAsync(ApplicationDbContext db)
        {
            if (await db.BlogPosts.AnyAsync()) return;

            var items = new List<BlogPost>
            {
                new()
                {
                    Title = "Complete Guide to GST Input Tax Credit (ITC) Reconciliations for FY 2025-26",
                    Slug = "gst-input-tax-credit-reconciliation-guide",
                    Category = "GST & Tax",
                    Excerpt = "Learn how businesses can maximize ITC claims, resolve GSTR-2B discrepancies, and avoid costly tax notices under current GST rules.",
                    Content = @"<p class='lead'>Input Tax Credit (ITC) reconciliation is one of the most critical monthly compliance exercises for registered Indian businesses under GST. Claiming ineligible ITC or missing out on valid credits can lead to penalty notices or cash flow strain.</p>
<h3>Key Principles of ITC Reconciliation</h3>
<p>To claim ITC smoothly under GSTR-3B, your purchase register must match records uploaded by your suppliers in their GSTR-1, which reflects in your auto-generated <strong>GSTR-2B</strong> statement.</p>
<ul>
    <li><strong>Matching GSTR-2B:</strong> Ensure all invoices are present in GSTR-2B before claiming credit in GSTR-3B.</li>
    <li><strong>Vendor Communication:</strong> Notify non-compliant suppliers promptly to upload pending invoices before the cutoff.</li>
    <li><strong>Reversing Ineligible ITC:</strong> Track Rule 42 & 43 reversals for exempt supplies or non-business usage.</li>
</ul>
<div class='p-3 bg-light border-start border-primary border-4 rounded mb-4'>
    <strong>Pro Tip:</strong> Reconcile your purchase registers bi-weekly rather than at month-end to give vendors adequate time for invoice corrections.
</div>",
                    FeaturedImagePath = "/images/services/gst-tax.svg",
                    AuthorName = "CA Priya Mehta",
                    AuthorRole = "FCA, Senior GST Consultant",
                    AuthorAvatar = "/images/ca/ca-priya-mehta.svg",
                    PublishedDate = DateTime.UtcNow.AddDays(-3),
                    IsPublished = true,
                    ViewsCount = 1420,
                    ReadTimeMinutes = 6,
                    Tags = new() { "GST", "ITC", "Tax Filing", "Compliance" },
                    MetaTitle = "GST Input Tax Credit Reconciliation Guide 2025-26",
                    MetaDescription = "Step by step guide to ITC claims under GSTR-2B vs 3B by Senior FCA."
                },
                new()
                {
                    Title = "Income Tax Planning Strategy for High Net-Worth Individuals & Business Owners",
                    Slug = "income-tax-planning-strategy-hnwi",
                    Category = "Income Tax",
                    Excerpt = "Discover effective strategies under Section 80C, 80D, capital gains exemption, and family trust structuring to optimize your annual tax liability.",
                    Content = @"<p class='lead'>Tax planning for High Net-Worth Individuals (HNWIs) requires a holistic approach that balances current tax savings with long-term wealth preservation and regulatory compliance.</p>
<h3>New vs. Old Tax Regime Analysis</h3>
<p>Choosing between the Old Tax Regime (with deductions) and the New Tax Regime (with lower slab rates) depends on your specific investment profile and capital gains distribution.</p>
<h3>Key Wealth Structuring Options</h3>
<ul>
    <li><strong>Section 54/54F Investment:</strong> Reinvest residential capital gains to legally offset tax.</li>
    <li><strong>HUF Creation:</strong> Establish a Hindu Undivided Family structure for separate tax slab benefits.</li>
    <li><strong>Health Insurance Deductions:</strong> Maximize Section 80D limits for senior citizen parents up to ₹50,000.</li>
</ul>",
                    FeaturedImagePath = "/images/services/income-tax.svg",
                    AuthorName = "CA Rajesh Sharma",
                    AuthorRole = "FCA, Direct Tax Specialist",
                    AuthorAvatar = "/images/ca/ca-rajesh-sharma.svg",
                    PublishedDate = DateTime.UtcNow.AddDays(-7),
                    IsPublished = true,
                    ViewsCount = 980,
                    ReadTimeMinutes = 8,
                    Tags = new() { "Income Tax", "ITR", "Tax Planning", "HNWI" },
                    MetaTitle = "Income Tax Planning for HNWIs and Business Owners",
                    MetaDescription = "Legally optimize your annual tax liability with actionable tips from CA Rajesh Sharma."
                },
                new()
                {
                    Title = "Startup Equity & Valuation: ESOP Structuring & Angel Tax Compliance",
                    Slug = "startup-equity-valuation-esop-guide",
                    Category = "Startup Advisory",
                    Excerpt = "A comprehensive playbook for founders on structuring Employee Stock Ownership Plans (ESOPs), cap tables, and valuation reports.",
                    Content = @"<p class='lead'>For fast-growing startups, equity is the most valuable currency. Structuring ESOPs properly and maintaining accredited valuation reports protects founders during fundraising rounds.</p>
<h3>ESOP Implementation Steps</h3>
<p>Creating an ESOP pool requires shareholder resolution, drafting the ESOP scheme, and getting a Registered Valuer report for fair market value determination.</p>",
                    FeaturedImagePath = "/images/services/startup-finance.svg",
                    AuthorName = "CA Vikram Joshi",
                    AuthorRole = "ACA, Startup & VC Specialist",
                    AuthorAvatar = "/images/ca/ca-vikram-joshi.svg",
                    PublishedDate = DateTime.UtcNow.AddDays(-12),
                    IsPublished = true,
                    ViewsCount = 2150,
                    ReadTimeMinutes = 7,
                    Tags = new() { "Startups", "ESOP", "Valuation", "Fundraising" },
                    MetaTitle = "Startup Equity & ESOP Structuring Guide",
                    MetaDescription = "A complete founder playbook on ESOPs, valuations, and cap tables."
                },
                new()
                {
                    Title = "ROC Annual Compliance Checklist for Private Limited Companies",
                    Slug = "roc-annual-compliance-checklist-pvt-ltd",
                    Category = "Corporate Law",
                    Excerpt = "Ensure your company stays compliant with mandatory MCA filings including AOC-4, MGT-7, DIR-3 KYC, and AGM deadlines.",
                    Content = @"<p class='lead'>Failing to file mandatory ROC returns with the Ministry of Corporate Affairs (MCA) leads to heavy per-day penalties and director disqualifications.</p>
<h3>Essential MCA Returns Checklist</h3>
<ul>
    <li><strong>AOC-4:</strong> Financial Statement filing within 30 days of AGM.</li>
    <li><strong>MGT-7 / MGT-7A:</strong> Annual Return filing within 60 days of AGM.</li>
    <li><strong>DIR-3 KYC:</strong> Annual director KYC verification before September 30.</li>
</ul>",
                    FeaturedImagePath = "/images/services/corporate-compliance.svg",
                    AuthorName = "CA Anita Krishnan",
                    AuthorRole = "FCA, Corporate Law Advisory",
                    AuthorAvatar = "/images/ca/ca-anita-krishnan.svg",
                    PublishedDate = DateTime.UtcNow.AddDays(-18),
                    IsPublished = true,
                    ViewsCount = 1670,
                    ReadTimeMinutes = 5,
                    Tags = new() { "ROC", "MCA", "Corporate Compliance", "Pvt Ltd" },
                    MetaTitle = "ROC Annual Compliance Checklist for Pvt Ltd",
                    MetaDescription = "Never miss an MCA compliance deadline with this comprehensive annual checklist."
                }
            };

            await db.BlogPosts.AddRangeAsync(items);
            await db.SaveChangesAsync();
        }

        private static async Task SeedFaqsAsync(ApplicationDbContext db)
        {
            if (await db.Faqs.AnyAsync()) return;

            var items = new List<FaqItemEntity>
            {
                new() { Question="How are CA professionals verified on CACampus?", Answer="Every CA listed undergoes identity, ICAI membership number verification, and background checks before their profile is approved.", Category="General", DisplayOrder=1, IsActive=true },
                new() { Question="Is CACampus free to use for clients?", Answer="Yes! Our Starter plan is 100% free — you can browse and contact verified CA professionals at no upfront cost. Upgrade for premium retainership.", Category="General", DisplayOrder=2, IsActive=true },
                new() { Question="How long does it take to connect with a CA for my requirement?", Answer="Clients receive prompt responses from empaneled CA professionals typically within 1 to 2 business hours.", Category="Services", DisplayOrder=3, IsActive=true },
                new() { Question="Are payments and documents secure on the platform?", Answer="Absolutely. We enforce end-to-end encrypted document storage and milestone-based transparent billing.", Category="Security", DisplayOrder=4, IsActive=true },
                new() { Question="Can CAs from any city join the CACampus network?", Answer="Yes. CACampus is a pan-India network connecting businesses with certified Chartered Accountants in all tier-1, tier-2, and tier-3 business hubs.", Category="General", DisplayOrder=5, IsActive=true }
            };

            await db.Faqs.AddRangeAsync(items);
            await db.SaveChangesAsync();
        }

        private static async Task SeedTestimonialsAsync(ApplicationDbContext db)
        {
            if (await db.Testimonials.AnyAsync()) return;

            var items = new List<TestimonialEntity>
            {
                new() { Text="Found a GST specialist quickly after posting. The CA resolved our entire compliance backlog seamlessly. Outstanding experience.", AuthorName="Suresh G.",  AuthorRole="Business Owner", Initials="SG", Rating=5, DisplayOrder=1, IsActive=true },
                new() { Text="As a startup, we needed specialized guidance on equity structuring. CACampus connected us with an expert who guided us through our seed round.", AuthorName="Nisha R.",    AuthorRole="Startup Founder",  Initials="NR", Rating=5, DisplayOrder=2, IsActive=true },
                new() { Text="The compliance management process saves us from costly delays every quarter. The CA professionals here are thorough, prompt, and professional.", AuthorName="Mohan K.", AuthorRole="Finance Manager",    Initials="MK", Rating=5, DisplayOrder=3, IsActive=true }
            };

            await db.Testimonials.AddRangeAsync(items);
            await db.SaveChangesAsync();
        }

        private static async Task SeedPricingPlansAsync(ApplicationDbContext db)
        {
            if (await db.PricingPlans.AnyAsync()) return;

            var items = new List<PricingPlanEntity>
            {
                new() { PlanName="Starter Plan", PriceDisplay="₹ 0", BillingCycle="Free", Description="For individuals and small business enquiries.", Features=new(){ "Browse Verified CAs", "Contact up to 3 CAs", "Standard Support" }, IsPopular=false, DisplayOrder=1, IsActive=true },
                new() { PlanName="Professional", PriceDisplay="₹ 1,499", BillingCycle="/month", Description="For growing businesses needing active compliance.", Features=new(){ "Unlimited CA Contacts", "Direct Call & WhatsApp", "Priority Milestone Tracking", "Dedicated Account Manager" }, IsPopular=true, DisplayOrder=2, IsActive=true },
                new() { PlanName="Enterprise", PriceDisplay="₹ 4,999", BillingCycle="/month", Description="For corporates requiring complete outsourced CA & legal team.", Features=new(){ "Custom CA Retainership", "Dedicated Senior FCA", "End-to-End Audit & Tax", "24/7 SLA Guarantee" }, IsPopular=false, DisplayOrder=3, IsActive=true }
            };

            await db.PricingPlans.AddRangeAsync(items);
            await db.SaveChangesAsync();
        }

        private static async Task SeedContentPagesAsync(ApplicationDbContext db)
        {
            if (await db.ContentPages.AnyAsync()) return;

            var items = new List<ContentPage>
            {
                new() { Title="About Us", Slug="about-us", MetaDescription="Learn about CACampus — India's premier verified CA network.", IsPublished=true, LastUpdated=DateTime.UtcNow, HtmlContent="<h2>About CACampus</h2><p>CACampus combines modern technology and certified expertise to deliver reliable CA, legal, and compliance solutions across India.</p><p>Our empaneled chartered accountants help over 50,000+ businesses with tax planning, ROC filings, forensic audits, and GST compliance.</p>" },
                new() { Title="Terms & Conditions", Slug="terms-and-conditions", MetaDescription="Terms and conditions governing the use of CACampus platform.", IsPublished=true, LastUpdated=DateTime.UtcNow, HtmlContent="<h2>Terms & Conditions</h2><p>Welcome to CACampus. By using our platform, you agree to engage verified professionals under transparent milestone billing and confidentiality protocols.</p>" },
                new() { Title="Privacy Policy", Slug="privacy-policy", MetaDescription="CACampus privacy policy and data security commitments.", IsPublished=true, LastUpdated=DateTime.UtcNow, HtmlContent="<h2>Privacy Policy</h2><p>We respect client privacy and adhere to the highest industry standards for encrypting financial documents and sensitive business records.</p>" },
                new() { Title="Contact Us", Slug="contact-us", MetaDescription="Contact CACampus support and client advisory desk.", IsPublished=true, LastUpdated=DateTime.UtcNow, HtmlContent="<h2>Contact Us</h2><p>Have questions? Reach our dedicated consultation desk anytime via phone or email for rapid assistance.</p>" }
            };

            await db.ContentPages.AddRangeAsync(items);
            await db.SaveChangesAsync();
        }
    }
}
