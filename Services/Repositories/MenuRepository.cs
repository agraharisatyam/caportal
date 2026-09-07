using System.Text.Json;
using caportal.Models.Entities;

namespace caportal.Services.Repositories
{
    public static class MenuRepository
    {
        private static readonly string PrimaryPath = Path.Combine(AppContext.BaseDirectory, "menu.json");
        private static readonly string ProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "menu.json");
        private static List<NavbarMenuItem> _items = new();
        private static readonly object _lock = new();

        static MenuRepository()
        {
            Load();
            if (_items.Count == 0)
            {
                // Populate default menu items
                _items = new List<NavbarMenuItem>
                {
                    new() { Id = 1, DisplayName = "Home", Url = "/", Order = 1, IsActive = true, MegaMenuType = "None" },
                    new() { Id = 2, DisplayName = "Services", Url = "#features", Order = 2, IsActive = true, MegaMenuType = "Services" },
                    new() { Id = 3, DisplayName = "Tax & Legal", Url = "#tax-legal", Order = 3, IsActive = true, MegaMenuType = "TaxLegal" },
                    new() { Id = 4, DisplayName = "Find an Expert", Url = "/find-expert", Order = 4, IsActive = true, MegaMenuType = "FindExpert" },
                    new() { Id = 5, DisplayName = "How It Works", Url = "#how-it-works", Order = 5, IsActive = true, MegaMenuType = "None" },
                    new() { Id = 6, DisplayName = "Pricing", Url = "#pricing", Order = 6, IsActive = false, MegaMenuType = "None" },
                    new() { Id = 7, DisplayName = "Blog", Url = "/blog", Order = 7, IsActive = true, MegaMenuType = "None" },
                    new() { Id = 8, DisplayName = "Contact", Url = "/contact", Order = 8, IsActive = true, MegaMenuType = "None" }
                };
                Save();
            }
        }

        private static void Load()
        {
            lock (_lock)
            {
                try
                {
                    string targetFile = File.Exists(ProjectPath) ? ProjectPath : (File.Exists(PrimaryPath) ? PrimaryPath : "");
                    if (!string.IsNullOrEmpty(targetFile))
                    {
                        var json = File.ReadAllText(targetFile);
                        _items = JsonSerializer.Deserialize<List<NavbarMenuItem>>(json) ?? new();

                        // Auto-heal / migrate: ensure Tax & Legal exists if missing in legacy json
                        if (_items.Count > 0 && !_items.Any(m => m.MegaMenuType == "TaxLegal" || m.DisplayName == "Tax & Legal"))
                        {
                            var maxId = _items.Max(m => m.Id);
                            var taxLegal = new NavbarMenuItem
                            {
                                Id = maxId + 1,
                                DisplayName = "Tax & Legal",
                                Url = "#tax-legal",
                                Order = 3,
                                IsActive = true,
                                MegaMenuType = "TaxLegal"
                            };
                            // shift items with order >= 3
                            foreach (var it in _items.Where(m => m.Order >= 3))
                            {
                                it.Order++;
                            }
                            _items.Add(taxLegal);
                            Save();
                        }

                        // Ensure DropdownItems list is not null on any item
                        foreach (var it in _items)
                        {
                            it.DropdownItems ??= new List<NavbarDropdownItem>();
                        }

                        // Seed default rich dropdown items for mega menus if currently empty
                        EnsureDefaultMegaMenuDropdownItems();
                    }
                }
                catch
                {
                    _items = new();
                }
            }
        }

        private static void Save()
        {
            lock (_lock)
            {
                try
                {
                    var json = JsonSerializer.Serialize(_items, new JsonSerializerOptions { WriteIndented = true });
                    try { File.WriteAllText(PrimaryPath, json); } catch { }
                    try { File.WriteAllText(ProjectPath, json); } catch { }
                }
                catch
                {
                    // Ignore or log
                }
            }
        }

        public static List<NavbarMenuItem> GetAll(bool includeInactive = true)
        {
            lock (_lock)
            {
                var query = _items.AsEnumerable();
                if (!includeInactive)
                {
                    query = query.Where(m => m.IsActive);
                }
                return query.OrderBy(m => m.Order).ToList();
            }
        }

        public static NavbarMenuItem? GetById(int id)
        {
            lock (_lock)
            {
                var item = _items.FirstOrDefault(m => m.Id == id);
                if (item != null)
                {
                    item.DropdownItems ??= new List<NavbarDropdownItem>();
                }
                return item;
            }
        }

        public static void Add(NavbarMenuItem item)
        {
            lock (_lock)
            {
                item.Id = _items.Count > 0 ? _items.Max(m => m.Id) + 1 : 1;
                item.DropdownItems ??= new List<NavbarDropdownItem>();
                _items.Add(item);
                Save();
            }
        }

        public static void Update(NavbarMenuItem updated)
        {
            lock (_lock)
            {
                var existing = _items.FirstOrDefault(m => m.Id == updated.Id);
                if (existing != null)
                {
                    existing.DisplayName = updated.DisplayName;
                    existing.Url = updated.Url;
                    existing.Order = updated.Order;
                    existing.IsActive = updated.IsActive;
                    existing.MegaMenuType = updated.MegaMenuType;
                    existing.OpenInNewTab = updated.OpenInNewTab;
                    if (updated.DropdownItems != null && updated.DropdownItems.Count > 0)
                    {
                        existing.DropdownItems = updated.DropdownItems;
                    }
                    Save();
                }
            }
        }

        public static void Delete(int id)
        {
            lock (_lock)
            {
                var item = _items.FirstOrDefault(m => m.Id == id);
                if (item != null)
                {
                    _items.Remove(item);
                    Save();
                }
            }
        }

        // ── Child Dropdown Items Management ──

        public static NavbarDropdownItem? AddDropdownItem(int menuId, NavbarDropdownItem item)
        {
            lock (_lock)
            {
                var menu = _items.FirstOrDefault(m => m.Id == menuId);
                if (menu == null) return null;

                menu.DropdownItems ??= new List<NavbarDropdownItem>();
                item.Id = menu.DropdownItems.Count > 0 ? menu.DropdownItems.Max(d => d.Id) + 1 : 1;
                if (item.Order == 0)
                {
                    item.Order = menu.DropdownItems.Count > 0 ? menu.DropdownItems.Max(d => d.Order) + 1 : 1;
                }
                menu.DropdownItems.Add(item);
                Save();
                return item;
            }
        }

        public static bool UpdateDropdownItem(int menuId, NavbarDropdownItem updated)
        {
            lock (_lock)
            {
                var menu = _items.FirstOrDefault(m => m.Id == menuId);
                if (menu == null || menu.DropdownItems == null) return false;

                var existing = menu.DropdownItems.FirstOrDefault(d => d.Id == updated.Id);
                if (existing == null) return false;

                existing.Title = updated.Title;
                existing.Url = updated.Url;
                existing.Category = updated.Category;
                existing.Icon = updated.Icon;
                existing.Badge = updated.Badge;
                existing.Order = updated.Order;
                existing.IsActive = updated.IsActive;
                existing.OpenInNewTab = updated.OpenInNewTab;
                Save();
                return true;
            }
        }

        public static bool DeleteDropdownItem(int menuId, int itemId)
        {
            lock (_lock)
            {
                var menu = _items.FirstOrDefault(m => m.Id == menuId);
                if (menu == null || menu.DropdownItems == null) return false;

                var item = menu.DropdownItems.FirstOrDefault(d => d.Id == itemId);
                if (item == null) return false;

                menu.DropdownItems.Remove(item);
                Save();
                return true;
            }
        }

        // ── Default Mega Menu Data Seeder ──

        private static void EnsureDefaultMegaMenuDropdownItems()
        {
            bool updated = false;

            // 1. Services
            var svc = _items.FirstOrDefault(m => m.MegaMenuType == "Services" || m.DisplayName == "Services");
            if (svc != null && (svc.DropdownItems == null || svc.DropdownItems.Count == 0))
            {
                svc.DropdownItems = GetDefaultServicesDropdownItems();
                updated = true;
            }

            // 2. Tax & Legal
            var tax = _items.FirstOrDefault(m => m.MegaMenuType == "TaxLegal" || m.DisplayName.Contains("Tax"));
            if (tax != null && (tax.DropdownItems == null || tax.DropdownItems.Count == 0))
            {
                tax.DropdownItems = GetDefaultTaxLegalDropdownItems();
                updated = true;
            }

            // 3. Find an Expert
            var exp = _items.FirstOrDefault(m => m.MegaMenuType == "FindExpert" || m.DisplayName.Contains("Expert"));
            if (exp != null && (exp.DropdownItems == null || exp.DropdownItems.Count == 0))
            {
                exp.DropdownItems = GetDefaultFindExpertDropdownItems();
                updated = true;
            }

            if (updated)
            {
                Save();
            }
        }

        private static List<NavbarDropdownItem> GetDefaultServicesDropdownItems()
        {
            var list = new List<NavbarDropdownItem>();
            int id = 1;

            void Add(string category, string title, string url, string badge = "", string icon = "")
            {
                list.Add(new NavbarDropdownItem
                {
                    Id = id,
                    Category = category,
                    Title = title,
                    Url = url,
                    Badge = badge,
                    Icon = icon,
                    Order = id,
                    IsActive = true
                });
                id++;
            }

            // Business Registration
            Add("Business Registration", "Startup Registration", "/services/startup-registration");
            Add("Business Registration", "Proprietorship Registration", "/services/proprietorship-registration");
            Add("Business Registration", "Partnership Registration", "/services/partnership-registration");
            Add("Business Registration", "LLP Registration", "/services/llp-registration");
            Add("Business Registration", "Private Limited Company", "/services/private-limited-company", "Popular");
            Add("Business Registration", "OPC Registration", "/services/opc-registration");
            Add("Business Registration", "Public Limited Company", "/services/public-limited-company");
            Add("Business Registration", "Section 8 Company", "/services/section-8-company");
            Add("Business Registration", "Producer Company", "/services/producer-company");
            Add("Business Registration", "Indian Subsidiary", "/services/indian-subsidiary");
            Add("Business Registration", "Shop & Establishment", "/services/shop-establishment");
            Add("Business Registration", "Trade Licence", "/services/trade-licence");
            Add("Business Registration", "MSME Registration", "/services/msme-registration");
            Add("Business Registration", "Virtual Office", "/services/virtual-office");

            // GST & Tax
            Add("GST & Tax", "GST Registration", "/services/gst-registration", "Popular");
            Add("GST & Tax", "GST Return Filing", "/services/gst-return-filing");
            Add("GST & Tax", "GST Amendment", "/services/gst-amendment");
            Add("GST & Tax", "GST Cancellation", "/services/gst-cancellation");
            Add("GST & Tax", "GST Revocation", "/services/gst-revocation");
            Add("GST & Tax", "GST Notice Reply", "/services/gst-notice-reply");
            Add("GST & Tax", "GST Refund", "/services/gst-refund");
            Add("GST & Tax", "GSTR-9 Filing", "/services/gstr-9-filing");
            Add("GST & Tax", "GSTR-10 Filing", "/services/gstr-10-filing");
            Add("GST & Tax", "LUT Filing", "/services/lut-filing");
            Add("GST & Tax", "E-Way Bill Services", "/services/e-way-bill-services");
            Add("GST & Tax", "ITR Filing", "/services/itr-filing", "Popular");
            Add("GST & Tax", "Tax Planning", "/services/tax-planning");
            Add("GST & Tax", "PAN / TAN / TDS", "/services/pan-tan-tds");

            // MCA & Compliance
            Add("MCA & Compliance", "ROC Filing", "/services/roc-filing");
            Add("MCA & Compliance", "Annual Compliance", "/services/annual-compliance", "Popular");
            Add("MCA & Compliance", "Director KYC", "/services/director-kyc");
            Add("MCA & Compliance", "DIN Application", "/services/din-application");
            Add("MCA & Compliance", "Digital Signature (DSC)", "/services/dsc-digital-signature");
            Add("MCA & Compliance", "Registered Office Change", "/services/registered-office-change");
            Add("MCA & Compliance", "Director Appointment", "/services/director-appointment");
            Add("MCA & Compliance", "Share Transfer", "/services/share-transfer");
            Add("MCA & Compliance", "Company Closure", "/services/company-closure");
            Add("MCA & Compliance", "PF Registration", "/services/pf-registration");
            Add("MCA & Compliance", "ESIC Registration", "/services/esic-registration");
            Add("MCA & Compliance", "Payroll Management", "/services/payroll-management");
            Add("MCA & Compliance", "Labour Compliance", "/services/labour-compliance");

            // Trademark & IPR
            Add("Trademark & IPR", "Trademark Search", "/services/trademark-search");
            Add("Trademark & IPR", "Trademark Registration", "/services/trademark-registration", "Popular");
            Add("Trademark & IPR", "Trademark Objection", "/services/trademark-objection");
            Add("Trademark & IPR", "Trademark Hearing", "/services/trademark-hearing");
            Add("Trademark & IPR", "Trademark Renewal", "/services/trademark-renewal");
            Add("Trademark & IPR", "Trademark Assignment", "/services/trademark-assignment");
            Add("Trademark & IPR", "Logo Registration", "/services/logo-registration");
            Add("Trademark & IPR", "Copyright Registration", "/services/copyright-registration");
            Add("Trademark & IPR", "Patent Registration", "/services/patent-registration");
            Add("Trademark & IPR", "Design Registration", "/services/design-registration");
            Add("Trademark & IPR", "Brand Protection", "/services/brand-protection");

            // Accounting & Audit
            Add("Accounting & Audit", "Bookkeeping", "/services/bookkeeping");
            Add("Accounting & Audit", "Accounting", "/services/accounting");
            Add("Accounting & Audit", "Financial Statements", "/services/financial-statements");
            Add("Accounting & Audit", "MIS Reporting", "/services/mis-reporting");
            Add("Accounting & Audit", "Internal Audit", "/services/internal-audit");
            Add("Accounting & Audit", "Statutory Audit", "/services/statutory-audit", "Popular");
            Add("Accounting & Audit", "GST Audit", "/services/gst-audit");
            Add("Accounting & Audit", "Tax Audit", "/services/tax-audit");
            Add("Accounting & Audit", "Stock Audit", "/services/stock-audit");
            Add("Accounting & Audit", "Virtual CFO", "/services/virtual-cfo", "New");

            // Other Services
            Add("Other Services", "NGO Registration", "/services/ngo-registration");
            Add("Other Services", "Trust Registration", "/services/trust-registration");
            Add("Other Services", "Startup Funding", "/services/startup-funding");
            Add("Other Services", "Project Finance", "/services/project-finance");
            Add("Other Services", "Business Loan", "/services/business-loan");
            Add("Other Services", "ISO Certification", "/services/iso-certification");
            Add("Other Services", "FSSAI Licence", "/services/fssai-licence");
            Add("Other Services", "IEC Registration", "/services/iec-registration");
            Add("Other Services", "NRI Taxation", "/services/nri-taxation");
            Add("Other Services", "Legal Documentation", "/services/legal-documentation");
            Add("Other Services", "Agreements", "/services/agreements");

            return list;
        }

        private static List<NavbarDropdownItem> GetDefaultTaxLegalDropdownItems()
        {
            var list = new List<NavbarDropdownItem>();
            int id = 1;

            void Add(string category, string title, string url, string badge = "", string icon = "")
            {
                list.Add(new NavbarDropdownItem
                {
                    Id = id,
                    Category = category,
                    Title = title,
                    Url = url,
                    Badge = badge,
                    Icon = icon,
                    Order = id,
                    IsActive = true
                });
                id++;
            }

            // Direct Tax Services
            Add("Direct Tax Services", "Income Tax Return (ITR) Filing", "/services/itr-filing", "Popular");
            Add("Direct Tax Services", "Income Tax Notice Reply", "/services/gst-notice-reply");
            Add("Direct Tax Services", "Tax Planning & Advisory", "/services/tax-planning");
            Add("Direct Tax Services", "Tax Audit", "/services/tax-audit");
            Add("Direct Tax Services", "Advance Tax Planning", "/services/tax-planning");
            Add("Direct Tax Services", "Tax Consultation", "/services/tax-planning");
            Add("Direct Tax Services", "Capital Gains Tax", "/services/tax-planning");
            Add("Direct Tax Services", "NRI Taxation", "/services/nri-taxation");
            Add("Direct Tax Services", "Form 15CA / 15CB", "/services/pan-tan-tds");

            // GST Services
            Add("GST Services", "GST Registration", "/services/gst-registration", "Popular");
            Add("GST Services", "GST Return Filing (GSTR)", "/services/gst-return-filing");
            Add("GST Services", "GSTR-9 Annual Return", "/services/gstr-9-filing");
            Add("GST Services", "GSTR-10 Final Return", "/services/gstr-10-filing");
            Add("GST Services", "GST Amendment", "/services/gst-amendment");
            Add("GST Services", "GST Cancellation", "/services/gst-cancellation");
            Add("GST Services", "GST Revocation", "/services/gst-revocation");
            Add("GST Services", "GST Notice Reply", "/services/gst-notice-reply");
            Add("GST Services", "GST Refund", "/services/gst-refund");
            Add("GST Services", "LUT Filing", "/services/lut-filing");
            Add("GST Services", "E-Way Bill Services", "/services/e-way-bill-services");

            // PAN / TAN / TDS / TCS
            Add("PAN / TAN / TDS / TCS", "PAN Registration", "/services/pan-tan-tds");
            Add("PAN / TAN / TDS / TCS", "PAN Correction / Update", "/services/pan-tan-tds");
            Add("PAN / TAN / TDS / TCS", "TAN Registration", "/services/pan-tan-tds");
            Add("PAN / TAN / TDS / TCS", "TDS Registration", "/services/pan-tan-tds");
            Add("PAN / TAN / TDS / TCS", "TDS Return Filing", "/services/pan-tan-tds");
            Add("PAN / TAN / TDS / TCS", "Form 16 / 16A", "/services/pan-tan-tds");
            Add("PAN / TAN / TDS / TCS", "TDS Certificate", "/services/pan-tan-tds");

            // Legal Services
            Add("Legal Services", "Legal Documentation", "/services/legal-documentation");
            Add("Legal Services", "Drafting & Vetting", "/services/agreements");
            Add("Legal Services", "Partnership Deed", "/services/partnership-registration");
            Add("Legal Services", "Shareholders Agreement", "/services/share-transfer");
            Add("Legal Services", "Business Contracts", "/services/agreements");
            Add("Legal Services", "Legal Notice", "/services/legal-documentation");
            Add("Legal Services", "Power of Attorney", "/services/legal-documentation");
            Add("Legal Services", "Affidavit / Declaration", "/services/legal-documentation");

            // IPR & Legal Protection
            Add("IPR & Legal Protection", "Trademark Search", "/services/trademark-search");
            Add("IPR & Legal Protection", "Trademark Registration", "/services/trademark-registration", "Popular");
            Add("IPR & Legal Protection", "Trademark Objection Reply", "/services/trademark-objection");
            Add("IPR & Legal Protection", "Trademark Hearing", "/services/trademark-hearing");
            Add("IPR & Legal Protection", "Trademark Renewal", "/services/trademark-renewal");
            Add("IPR & Legal Protection", "Logo Registration", "/services/logo-registration");
            Add("IPR & Legal Protection", "Copyright Registration", "/services/copyright-registration");
            Add("IPR & Legal Protection", "Patent Registration", "/services/patent-registration");

            return list;
        }

        private static List<NavbarDropdownItem> GetDefaultFindExpertDropdownItems()
        {
            var list = new List<NavbarDropdownItem>();
            int id = 1;

            void Add(string category, string title, string url, string badge = "", string icon = "")
            {
                list.Add(new NavbarDropdownItem
                {
                    Id = id,
                    Category = category,
                    Title = title,
                    Url = url,
                    Badge = badge,
                    Icon = icon,
                    Order = id,
                    IsActive = true
                });
                id++;
            }

            // Browse CAs
            Add("Browse CAs", "Browse All CAs", "/find-expert", "", "fas fa-search");
            Add("Browse CAs", "Top Rated CAs", "/find-expert?sort=rating", "Top", "fas fa-star");
            Add("Browse CAs", "Most Experienced", "/find-expert?sort=exp", "", "fas fa-briefcase");
            Add("Browse CAs", "Fresh Talent (0–5 Yrs)", "/find-expert?exp=0-5", "", "fas fa-rocket");
            Add("Browse CAs", "Mumbai", "/find-expert?city=Mumbai", "", "fas fa-map-marker-alt");
            Add("Browse CAs", "Delhi", "/find-expert?city=Delhi", "", "fas fa-map-marker-alt");
            Add("Browse CAs", "Bangalore", "/find-expert?city=Bangalore", "", "fas fa-map-marker-alt");
            Add("Browse CAs", "Pune", "/find-expert?city=Pune", "", "fas fa-map-marker-alt");
            Add("Browse CAs", "Hyderabad", "/find-expert?city=Hyderabad", "", "fas fa-map-marker-alt");

            // By Specialisation
            Add("By Specialisation", "GST Filing", "/find-expert?service=GST+Filing", "", "fas fa-file-invoice-dollar");
            Add("By Specialisation", "Income Tax", "/find-expert?service=Income+Tax", "", "fas fa-landmark");
            Add("By Specialisation", "Audit & Assurance", "/find-expert?service=Audit", "", "fas fa-chart-line");
            Add("By Specialisation", "Company Registration", "/find-expert?service=Company+Registration", "", "fas fa-building");
            Add("By Specialisation", "Transfer Pricing", "/find-expert?service=Transfer+Pricing", "", "fas fa-handshake");
            Add("By Specialisation", "FEMA & RBI", "/find-expert?service=FEMA", "", "fas fa-globe");
            Add("By Specialisation", "Startup Finance", "/find-expert?service=Startup+Finance", "", "fas fa-rocket");
            Add("By Specialisation", "Tax Litigation", "/find-expert?service=Tax+Litigation", "", "fas fa-balance-scale");
            Add("By Specialisation", "ROC & MCA", "/find-expert?service=ROC+Filing", "", "fas fa-folder-open");
            Add("By Specialisation", "Bookkeeping & Accounting", "/find-expert?service=Bookkeeping", "", "fas fa-book");

            return list;
        }
    }
}
