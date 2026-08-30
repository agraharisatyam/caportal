using caportal.Models.Entities;

namespace caportal.Services.Repositories
{
    public static class BlogRepository
    {
        private static readonly List<BlogPost> _posts = new()
        {
            new BlogPost
            {
                Id = 1,
                Title = "Complete Guide to GST Input Tax Credit (ITC) Reconciliations for FY 2025-26",
                Slug = "gst-input-tax-credit-reconciliation-guide",
                Category = "GST & Tax",
                Excerpt = "Learn how businesses can maximize ITC claims, resolve GSTR-2B discrepancies, and avoid costly tax notices under current GST rules.",
                Content = @"<p class='lead'>Input Tax Credit (ITC) reconciliation is one of the most critical monthly compliance exercises for registered Indian businesses under GST. Claiming ineligible ITC or missing out on valid credits can lead to penalty notices or cash flow strain.</p>

<h3>Key Principles of ITC Reconciliation</h3>
<p>To claim ITC smoothly under GSTR-3B, your purchase register must match records uploaded by your suppliers in their GSTR-1, which reflects in your auto-generated <strong>GSTR-2B</strong> statement.</p>

<ul>
    <li><strong>Matching GSTR-2B:</strong> Ensure all invoices are present in GSTR-2B before claiming credit in GSTR-3B.</li>
    <li><strong>Vendor Communication:</strong> Notify non-compliant suppliers promptly to upload pending invoices before the November cutoff.</li>
    <li><strong>Reversing Ineligible ITC:</strong> Track Rule 42 & 43 reversals for exempt supplies or non-business usage.</li>
</ul>

<div class='p-3 bg-light border-start border-primary border-4 rounded mb-4'>
    <strong>Pro Tip:</strong> Reconcile your purchase registers bi-weekly rather than at month-end to give vendors adequate time for invoice corrections.
</div>

<h3>Common ITC Pitfalls to Avoid</h3>
<p>Ensure that vendor GSTINs, invoice dates, and tax values are matched with 100% precision. Minor typographical errors in GSTIN can block credit under automated verification algorithms.</p>",
                FeaturedImagePath = "/images/services/gst-tax.svg",
                AuthorName = "CA Priya Mehta",
                AuthorRole = "FCA, Senior GST Consultant",
                AuthorAvatar = "/images/ca/ca-priya-mehta.svg",
                PublishedDate = DateTime.Now.AddDays(-3),
                IsPublished = true,
                ViewsCount = 1420,
                ReadTimeMinutes = 6,
                Tags = new() { "GST", "ITC", "Tax Filing", "Compliance" }
            },
            new BlogPost
            {
                Id = 2,
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
                PublishedDate = DateTime.Now.AddDays(-7),
                IsPublished = true,
                ViewsCount = 980,
                ReadTimeMinutes = 8,
                Tags = new() { "Income Tax", "ITR", "Tax Planning", "HNWI" }
            },
            new BlogPost
            {
                Id = 3,
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
                PublishedDate = DateTime.Now.AddDays(-12),
                IsPublished = true,
                ViewsCount = 2150,
                ReadTimeMinutes = 7,
                Tags = new() { "Startups", "ESOP", "Valuation", "Fundraising" }
            },
            new BlogPost
            {
                Id = 4,
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
                PublishedDate = DateTime.Now.AddDays(-18),
                IsPublished = true,
                ViewsCount = 1670,
                ReadTimeMinutes = 5,
                Tags = new() { "ROC", "MCA", "Corporate Compliance", "Pvt Ltd" }
            }
        };

        public static List<BlogPost> GetAll(bool includeUnpublished = false)
        {
            lock (_posts)
            {
                var query = _posts.AsEnumerable();
                if (!includeUnpublished)
                    query = query.Where(p => p.IsPublished);
                return query.OrderByDescending(p => p.PublishedDate).ToList();
            }
        }

        public static BlogPost? GetBySlug(string slug)
        {
            lock (_posts)
            {
                var post = _posts.FirstOrDefault(p => p.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
                if (post != null)
                {
                    post.ViewsCount++;
                }
                return post;
            }
        }

        public static BlogPost? GetById(int id)
        {
            lock (_posts)
            {
                return _posts.FirstOrDefault(p => p.Id == id);
            }
        }

        public static void Add(BlogPost post)
        {
            lock (_posts)
            {
                post.Id = _posts.Count > 0 ? _posts.Max(p => p.Id) + 1 : 1;
                if (string.IsNullOrEmpty(post.Slug))
                {
                    post.Slug = post.Title.ToLowerInvariant()
                        .Replace(" ", "-")
                        .Replace("&", "and");
                }
                _posts.Add(post);
            }
        }

        public static void Update(BlogPost post)
        {
            lock (_posts)
            {
                var existing = _posts.FirstOrDefault(p => p.Id == post.Id);
                if (existing != null)
                {
                    existing.Title = post.Title;
                    existing.Slug = post.Slug;
                    existing.Category = post.Category;
                    existing.Excerpt = post.Excerpt;
                    existing.Content = post.Content;
                    existing.FeaturedImagePath = post.FeaturedImagePath;
                    existing.AuthorName = post.AuthorName;
                    existing.AuthorRole = post.AuthorRole;
                    existing.IsPublished = post.IsPublished;
                    existing.ReadTimeMinutes = post.ReadTimeMinutes;
                    existing.Tags = post.Tags;
                    existing.MetaTitle = post.MetaTitle;
                    existing.MetaDescription = post.MetaDescription;
                    existing.MetaKeywords = post.MetaKeywords;
                }
            }
        }

        public static void Delete(int id)
        {
            lock (_posts)
            {
                var post = _posts.FirstOrDefault(p => p.Id == id);
                if (post != null)
                {
                    _posts.Remove(post);
                }
            }
        }
    }
}
