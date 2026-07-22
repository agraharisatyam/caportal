using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using caportal.Models;

namespace caportal.Services
{
    public static class ServiceDetailsRepository
    {
        // Complete mapping of URL slugs to their exact Name and Category
        public static readonly Dictionary<string, (string Name, string Category)> ServiceMap = new(StringComparer.OrdinalIgnoreCase)
        {
            // Business Registration
            { "startup-registration", ("Startup Registration", "Business Registration") },
            { "proprietorship-registration", ("Proprietorship Registration", "Business Registration") },
            { "partnership-registration", ("Partnership Registration", "Business Registration") },
            { "llp-registration", ("LLP Registration", "Business Registration") },
            { "private-limited-company", ("Private Limited Company", "Business Registration") },
            { "opc-registration", ("OPC Registration", "Business Registration") },
            { "public-limited-company", ("Public Limited Company", "Business Registration") },
            { "section-8-company", ("Section 8 Company", "Business Registration") },
            { "producer-company", ("Producer Company", "Business Registration") },
            { "indian-subsidiary", ("Indian Subsidiary", "Business Registration") },
            { "shop-establishment", ("Shop & Establishment", "Business Registration") },
            { "trade-licence", ("Trade Licence", "Business Registration") },
            { "msme-registration", ("MSME Registration", "Business Registration") },
            { "virtual-office", ("Virtual Office", "Business Registration") },

            // GST & Tax
            { "gst-registration", ("GST Registration", "GST & Tax") },
            { "gst-return-filing", ("GST Return Filing", "GST & Tax") },
            { "gst-amendment", ("GST Amendment", "GST & Tax") },
            { "gst-cancellation", ("GST Cancellation", "GST & Tax") },
            { "gst-revocation", ("GST Revocation", "GST & Tax") },
            { "gst-notice-reply", ("GST Notice Reply", "GST & Tax") },
            { "gst-refund", ("GST Refund", "GST & Tax") },
            { "gstr-9-filing", ("GSTR-9 Filing", "GST & Tax") },
            { "gstr-10-filing", ("GSTR-10 Filing", "GST & Tax") },
            { "lut-filing", ("LUT Filing", "GST & Tax") },
            { "e-way-bill-services", ("E-Way Bill Services", "GST & Tax") },
            { "itr-filing", ("ITR Filing", "GST & Tax") },
            { "tax-planning", ("Tax Planning", "GST & Tax") },
            { "pan-tan-tds", ("PAN / TAN / TDS", "GST & Tax") },

            // MCA & Compliance
            { "roc-filing", ("ROC Filing", "MCA & Compliance") },
            { "annual-compliance", ("Annual Compliance", "MCA & Compliance") },
            { "director-kyc", ("Director KYC", "MCA & Compliance") },
            { "din-application", ("DIN Application", "MCA & Compliance") },
            { "dsc-digital-signature", ("Digital Signature (DSC)", "MCA & Compliance") },
            { "registered-office-change", ("Registered Office Change", "MCA & Compliance") },
            { "director-appointment", ("Director Appointment", "MCA & Compliance") },
            { "share-transfer", ("Share Transfer", "MCA & Compliance") },
            { "company-closure", ("Company Closure", "MCA & Compliance") },
            { "pf-registration", ("PF Registration", "MCA & Compliance") },
            { "esic-registration", ("ESIC Registration", "MCA & Compliance") },
            { "payroll-management", ("Payroll Management", "MCA & Compliance") },
            { "labour-compliance", ("Labour Compliance", "MCA & Compliance") },

            // Trademark & IPR
            { "trademark-search", ("Trademark Search", "Trademark & IPR") },
            { "trademark-registration", ("Trademark Registration", "Trademark & IPR") },
            { "trademark-objection", ("Trademark Objection", "Trademark & IPR") },
            { "trademark-hearing", ("Trademark Hearing", "Trademark & IPR") },
            { "trademark-renewal", ("Trademark Renewal", "Trademark & IPR") },
            { "trademark-assignment", ("Trademark Assignment", "Trademark & IPR") },
            { "logo-registration", ("Logo Registration", "Trademark & IPR") },
            { "copyright-registration", ("Copyright Registration", "Trademark & IPR") },
            { "patent-registration", ("Patent Registration", "Trademark & IPR") },
            { "design-registration", ("Design Registration", "Trademark & IPR") },
            { "brand-protection", ("Brand Protection", "Trademark & IPR") },

            // Accounting & Audit
            { "bookkeeping", ("Bookkeeping", "Accounting & Audit") },
            { "accounting", ("Accounting", "Accounting & Audit") },
            { "financial-statements", ("Financial Statements", "Accounting & Audit") },
            { "mis-reporting", ("MIS Reporting", "Accounting & Audit") },
            { "internal-audit", ("Internal Audit", "Accounting & Audit") },
            { "statutory-audit", ("Statutory Audit", "Accounting & Audit") },
            { "gst-audit", ("GST Audit", "Accounting & Audit") },
            { "tax-audit", ("Tax Audit", "Accounting & Audit") },
            { "stock-audit", ("Stock Audit", "Accounting & Audit") },
            { "virtual-cfo", ("Virtual CFO", "Accounting & Audit") },

            // Other Services
            { "ngo-registration", ("NGO Registration", "Other Services") },
            { "trust-registration", ("Trust Registration", "Other Services") },
            { "startup-funding", ("Startup Funding", "Other Services") },
            { "project-finance", ("Project Finance", "Other Services") },
            { "business-loan", ("Business Loan", "Other Services") },
            { "iso-certification", ("ISO Certification", "Other Services") },
            { "fssai-licence", ("FSSAI Licence", "Other Services") },
            { "iec-registration", ("IEC Registration", "Other Services") },
            { "nri-taxation", ("NRI Taxation", "Other Services") },
            { "legal-documentation", ("Legal Documentation", "Other Services") },
            { "agreements", ("Agreements", "Other Services") }
        };

        // Static cache of highly curated details for popular services
        private static readonly Dictionary<string, ServiceDetail> CuratedDetails = new(StringComparer.OrdinalIgnoreCase);

        static ServiceDetailsRepository()
        {
            // Seed Curated Details for Popular Services (GST, Ptd Ltd, Trademark, Startup, ITR)
            SeedCurated();
        }

        public static ServiceDetail? GetBySlug(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return null;

            // Check if we have curated data first
            if (CuratedDetails.TryGetValue(slug, out var curated))
            {
                return curated;
            }

            // Fallback: Check if slug exists in the complete map
            if (ServiceMap.TryGetValue(slug, out var mapped))
            {
                // Generate detailed relevant mock content dynamically based on the Category
                return GenerateFallbackDetail(slug, mapped.Name, mapped.Category);
            }

            return null; // Not found in service map
        }

        private static ServiceDetail GenerateFallbackDetail(string slug, string name, string category)
        {
            var detail = new ServiceDetail
            {
                Slug = slug,
                Name = name,
                Category = category,
                CategorySlug = GetCategorySlug(category)
            };

            // Set content based on category templates to ensure highly professional and realistic layout
            switch (category)
            {
                case "Business Registration":
                    detail.ShortDescription = $"Get your {name} registered quickly and legally. Professional documentation, government filing, and standard compliances sorted.";
                    detail.Overview = $"Setting up a {name} is the first major step towards building your business. Our experts handle the entire paperwork, registration portals, and local licensing compliance, allowing you to focus on your core strategy. We ensure the application complies fully with all federal and state guidelines.";
                    detail.PriceRange = "₹2,499 - ₹8,999";
                    detail.Timeline = "4 to 7 Business Days";
                    detail.Benefits = new List<string>
                    {
                        "Establish legal corporate identity and brand structure",
                        "Easily open bank accounts and secure merchant services",
                        "Qualify for MSME benefits, tax exemptions, and government tenders",
                        "Build credibility and trust with vendors, partners, and clients",
                        "Protect owners from individual personal liabilities"
                    };
                    detail.DocumentsRequired = new List<string>
                    {
                        "PAN Card of all promoters, partners, or directors",
                        "Identity Proof (Aadhaar, Passport, or Voter ID) of all partners",
                        "Registered Office Address Proof (Utility bill / Rent Agreement)",
                        "NOC (No Objection Certificate) from the owner of premises",
                        "Passport-sized photos of all promoters"
                    };
                    detail.ProcessSteps = new List<(string, string)>
                    {
                        ("Data Collection", "Submit your identity proofs, company name suggestions, and address details to our portal."),
                        ("Name Reservation", "We file the name approval application to ensure your desired brand name is reserved exclusively."),
                        ("Drafting & Uploading", "We prepare incorporation deeds, charter documents, and submit filings to the registry."),
                        ("Government Verification", "Registrar officials verify documentation. We respond to any clarification requests."),
                        ("License Issuance", "Your registration certificate, business tax numbers (PAN/TAN), and approvals are delivered.")
                    };
                    detail.Faqs = new List<(string, string)>
                    {
                        ($"What is the validity of the {name} certificate?", "It is valid lifelong, subject to annual regulatory compliances and filing requirements."),
                        ("Can a salaried employee register a business?", "Yes, in most cases, though they must review employment contracts for non-compete covenants."),
                        ("Do I need a commercial office space?", "No, you can register the business at a residential address using a utility bill and an NOC.")
                    };
                    break;

                case "GST & Tax":
                    detail.ShortDescription = $"Secure your {name} compliance today. Handled by verified tax consultants to ensure precision and prevent litigation.";
                    detail.Overview = $"Tax compliance can be tedious. With {name}, we offer end-to-end filing, corrections, advisory, and submission support to keep your business fully compliant. Our team handles calculations, input tax reconciliation, and direct representations before tax authorities.";
                    detail.PriceRange = "₹999 - ₹4,999";
                    detail.Timeline = "2 to 4 Business Days";
                    detail.Benefits = new List<string>
                    {
                        "Ensure 100% compliance with current national tax laws",
                        "Avoid hefty penalties, late fees, and operational blocks",
                        "Avail input tax credits (ITC) seamlessly on business expenses",
                        "Gain legal protection and structured replies to department notices",
                        "Present clean compliance records to auditors and investors"
                    };
                    detail.DocumentsRequired = new List<string>
                    {
                        "PAN Card of the Business Entity / Proprietor",
                        "Aadhaar Card of the Applicant / Authorized Signatory",
                        "Address Proof of the registered business location",
                        "Bank Account details (Cancelled cheque / Bank statement)",
                        "Authorization letter signed by the board or partners"
                    };
                    detail.ProcessSteps = new List<(string, string)>
                    {
                        ("Document Upload", "Securely upload your bills, bank feeds, and entity details to our dashboard."),
                        ("Data Reconciliation", "Our tax team validates sales books, ledgers, and reconciles input tax balances."),
                        ("Draft Review", "We share tax drafts and calculations for your approval before final submission."),
                        ("Portal Filing", "We file your application or return directly on the national tax networks."),
                        ("Receipt Delivery", "We share ARN receipt numbers and compliance certificates for your records.")
                    };
                    detail.Faqs = new List<(string, string)>
                    {
                        ("What happens if compliance is delayed?", "Delayed filings incur statutory daily penalties and interest charges on unpaid dues."),
                        ("Can filing details be corrected?", "Yes, through amendment filings, depending on specific regulatory timelines."),
                        ("Is a physical visit to the department required?", "No, the entire process is conducted online through our virtual CA assistance.")
                    };
                    break;

                case "MCA & Compliance":
                    detail.ShortDescription = $"Keep your business records updated and compliant. Avoid MCA fines with timely filings of DIR, ROC, and labour registers.";
                    detail.Overview = $"Ministry of Corporate Affairs (MCA) guidelines require periodic, event-based, and annual filings for LLPs and companies. Our {name} service guarantees error-free filings, expert documentation, and compliant board registers to protect your active status.";
                    detail.PriceRange = "₹1,499 - ₹12,999";
                    detail.Timeline = "3 to 5 Business Days";
                    detail.Benefits = new List<string>
                    {
                        "Maintain active corporate status with the Ministry registry",
                        "Prevent disqualification of company directors and partners",
                        "Avoid massive default penalties (up to ₹100/day per form)",
                        "Streamline legal audits and investor due diligence processes",
                        "Validate changes in corporate structure smoothly"
                    };
                    detail.DocumentsRequired = new List<string>
                    {
                        "Company Incorporation Certificate / LLP Agreement",
                        "Active Digital Signature Certificate (DSC) of directors",
                        "Financial statement draft (Balance Sheet & Profit & Loss)",
                        "Director KYC updates (PAN, Aadhaar, email, phone)",
                        "Board resolutions or meeting minutes relative to changes"
                    };
                    detail.ProcessSteps = new List<(string, string)>
                    {
                        ("Information Gathering", "Provide details of your filings, active directors, or required corporate changes."),
                        ("Drafting & Formatting", "We draft relevant resolutions, forms (AOC-4, MGT-7, DIR, etc.), and prepare XML files."),
                        ("Digital Signing", "Promoters sign forms using their authorized Digital Signature Certificates (DSC)."),
                        ("MCA Portal Upload", "We upload forms to the central portal and pay the statutory filing fees."),
                        ("Challan Delivery", "Receipts and processed approvals are archived and delivered to you.")
                    };
                    detail.Faqs = new List<(string, string)>
                    {
                        ("What is the penalty for missed filings?", "Late filings attract standard penalty slabs, which escalate exponentially over time."),
                        ("Do I need to maintain physical records?", "Yes, certain records like minute books and member registers must be maintained at the registered office."),
                        ("Who signs the compliance forms?", "Designated Directors/Partners and, in specific cases, a practicing CS/CA/CMA.")
                    };
                    break;

                case "Trademark & IPR":
                    detail.ShortDescription = $"Protect your name, logo, and ideas. Secure exclusive intellectual property rights with specialized TM and design filings.";
                    detail.Overview = $"Intellectual property is a company's greatest asset. {name} protects your unique brand identifiers, inventions, or creative works from infringements. We handle search clearance, application drafting, and reply to objections from the IP Registry.";
                    detail.PriceRange = "₹1,999 - ₹9,999";
                    detail.Timeline = "2 to 3 Business Days (App filed)";
                    detail.Benefits = new List<string>
                    {
                        "Acquire exclusive rights to use your brand name and logo",
                        "Legally sue third-parties for copyright or trademark infringement",
                        "Build intangible brand equity and license IP for royalty incomes",
                        "Deter competitors from using similar logos or names",
                        "Use the TM symbol immediately upon filing application"
                    };
                    detail.DocumentsRequired = new List<string>
                    {
                        "Soft copy of the Logo / Brand Name / Artwork",
                        "Trademark Authorization Letter (Form TM-48)",
                        "Identity Proof of the applicant (PAN, Aadhaar)",
                        "Business Certificate (if applying under MSME/Startup category)",
                        "Affidavit of user date (if mark is already in use)"
                    };
                    detail.ProcessSteps = new List<(string, string)>
                    {
                        ("IP Search", "We perform a thorough search to ensure your mark is unique and clear of conflicts."),
                        ("Form Preparation", "We compile form templates, descriptions of classes, and format logos for registry specifications."),
                        ("Filing Application", "The application is filed online, generating an instant application number."),
                        ("Registry Examination", "IP Registry examines the application. We monitor for objections or reports."),
                        ("Certificate Issuance", "Upon publication in the Trademark Journal and clearing oppositions, the certificate is issued.")
                    };
                    detail.Faqs = new List<(string, string)>
                    {
                        ("How long does trademark registration take?", "Filing takes 2 days (allowing TM use), but final registration takes 8-12 months due to government timelines."),
                        ("What are trademark classes?", "Goods and services are divided into 45 classes. You must file in classes matching your business."),
                        ("How long is a trademark valid?", "Once registered, it is valid for 10 years, renewable indefinitely.")
                    };
                    break;

                case "Accounting & Audit":
                    detail.ShortDescription = $"Ensure financial transparency and tax audit readiness. Bookkeeping, ledger scrutiny, and signed audit reports with UDIN.";
                    detail.Overview = $"Accurate books of accounts are vital for tracking profitability, obtaining finance, and fulfilling statutory duties. Our {name} service helps you organize invoices, manage ledgers, compile balance sheets, and conduct independent audits with qualified CAs.";
                    detail.PriceRange = "₹3,999 - ₹24,999";
                    detail.Timeline = "5 to 10 Business Days";
                    detail.Benefits = new List<string>
                    {
                        "Gain clear visibility into business revenues, costs, and profit margins",
                        "Prepare flawless books to support bank loans and equity investments",
                        "Ensure error-free compliance with tax, GST, and corporate laws",
                        "Detect internal errors, bookkeeping frauds, or asset leakages",
                        "Receive professional advisory on cost control and tax deductions"
                    };
                    detail.DocumentsRequired = new List<string>
                    {
                        "Bank Statements / Bank feeds for the financial period",
                        "Sales and purchase invoices and expense vouchers",
                        "Previous year's audited balance sheet and returns",
                        "Details of assets acquired, loans, and major agreements",
                        "GST and TDS returns filed during the period"
                    };
                    detail.ProcessSteps = new List<(string, string)>
                    {
                        ("Data Sync", "Connect your accounting software or share physical statements securely with us."),
                        ("Scrutiny & Entry", "We reconcile invoices, clean up ledger errors, and post accounting adjustments."),
                        ("Financial Drafting", "We draft the Trial Balance, Balance Sheet, and P&L Statements."),
                        ("Scrutiny & Review", "Independent CAs audit the balances, verify compliance, and draft remarks."),
                        ("Final Sign-off", "We issue the signed Audit Report / Bookkeeping sign-off with necessary UDIN logs.")
                    };
                    detail.Faqs = new List<(string, string)>
                    {
                        ("Who performs the audit?", "Audits are conducted exclusively by qualified, independent CAs holding active ICAI certificates of practice."),
                        ("What is a UDIN?", "UDIN is a unique document identification number generated on the ICAI portal to verify authenticity."),
                        ("Can you handle remote bookkeeping?", "Yes, we work with all major cloud platforms like QuickBooks, Tally, and Zoho Books.")
                    };
                    break;

                default:
                    detail.ShortDescription = $"Expert assistance for {name}. Streamlined processes, transparent pricing, and direct consultation with experienced professionals.";
                    detail.Overview = $"Fulfill your {name} requirements efficiently. Our team of verified professionals guides you through the necessary documentation, regulatory platforms, and licensing requirements, ensuring quick processing and complete peace of mind.";
                    detail.PriceRange = "Custom Pricing";
                    detail.Timeline = "3 to 6 Business Days";
                    detail.Benefits = new List<string>
                    {
                        "Get guided by industry-specific CA and legal experts",
                        "Avoid common filing mistakes and regulatory rejections",
                        "Save weeks of manual research and government portal struggles",
                        "Enjoy transparent milestone tracking and escrow protections",
                        "Receive comprehensive advisory beyond the basic filing scope"
                    };
                    detail.DocumentsRequired = new List<string>
                    {
                        "PAN and Aadhaar card of the primary applicant",
                        "Business identity proof (if registering as a company)",
                        "Registered address proofs (Utility bills / NOC)",
                        "Specialized documents depending on the selected category"
                    };
                    detail.ProcessSteps = new List<(string, string)>
                    {
                        ("Discovery Call", "Consult with our CA to explain your requirement and finalize custom scope."),
                        ("Document Checklist", "Gather and share the list of required proofs on your private dashboard."),
                        ("Filing Preparation", "We compile the applications, drafts, or documentation folders."),
                        ("Review & Approval", "You review the final application files before they are submitted."),
                        ("Delivery", "Receive processed approvals, certificates, or final signed documents.")
                    };
                    detail.Faqs = new List<(string, string)>
                    {
                        ("How do I request a custom quotation?", "You can fill out the contact form on this page, and a consultant will reach out within 2 hours."),
                        ("Are government fees included in the price?", "Government fees depend on your state/entity type. We provide a transparent breakup before filing."),
                        ("Is my data secure?", "Yes, we use bank-grade encryption and secure NDAs to protect your corporate information.")
                    };
                    break;
            }

            return detail;
        }

        private static string GetCategorySlug(string category)
        {
            return category.ToLower()
                .Replace(" & ", "-and-")
                .Replace(" ", "-")
                .Replace("/", "-");
        }

        private static void SeedCurated()
        {
            // GST Registration
            CuratedDetails.Add("gst-registration", new ServiceDetail
            {
                Slug = "gst-registration",
                Name = "GST Registration",
                Category = "GST & Tax",
                CategorySlug = "gst-and-tax",
                ShortDescription = "Get your GST Registration done online in 3 days. Includes complete documentation, registration certificate, and expert CA advice.",
                Overview = "Goods and Services Tax (GST) registration is mandatory for businesses whose turnover exceeds the threshold limits (e.g. ₹40 Lakhs for goods, ₹20 Lakhs for services in India) or who engage in inter-state supply, e-commerce, or online trading. Registering for GST gives your business legal authorization, enables you to claim Input Tax Credit (ITC) on inputs, and allows you to charge tax from customers legally.",
                PriceRange = "₹999 - ₹1,499",
                Timeline = "3 to 5 Business Days",
                Benefits = new List<string>
                {
                    "Become a legally recognized supplier of goods or services",
                    "Claim Input Tax Credit (ITC) on business purchases and save 18% average cost",
                    "Sell products online on Amazon, Flipkart, or your own e-commerce portal",
                    "Conduct seamless inter-state transactions without restrictions",
                    "Open current bank accounts and apply for corporate credit easily"
                },
                DocumentsRequired = new List<string>
                {
                    "PAN Card of the Proprietor / Company / Partnership",
                    "Aadhaar Card of the Proprietor / Directors / Partners",
                    "Electricity Bill / Rent Agreement of the business premises",
                    "No Objection Certificate (NOC) from the property owner",
                    "Cancelled cheque / Bank statement of the business account",
                    "Board Resolution / Authorization letter for Authorized Signatory"
                },
                ProcessSteps = new List<(string, string)>
                {
                    ("Submit Details", "Provide your business type, directors' IDs, and proof of address on our secure client portal."),
                    ("Document Scrutiny", "Our tax compliance team reviews your files to ensure they meet GST portal specifications."),
                    ("Filing Form GST REG-01", "We draft and submit the registration form with all enclosures on the GST portal."),
                    ("ARN Validation", "We monitor the Application Reference Number (ARN) for any queries or clarifications from tax officers."),
                    ("GSTIN Issued", "The GST registration certificate (Form REG-06) is issued online. We deliver it directly to you.")
                },
                Faqs = new List<(string, string)>
                {
                    ("Is GST registration mandatory for all businesses?", "It is mandatory if your aggregate annual turnover exceeds the threshold (₹40 lakhs for goods, ₹20 lakhs for services), or if you sell across states, or sell via e-commerce portals."),
                    ("Can I register for GST voluntarily?", "Yes, you can register voluntarily to claim Input Tax Credit or if your corporate clients demand GST invoices."),
                    ("Do I need a commercial space to get GST?", "No. You can register your GST using a residential address by providing a utility bill and an NOC from the owner.")
                }
            });

            // Private Limited Company
            CuratedDetails.Add("private-limited-company", new ServiceDetail
            {
                Slug = "private-limited-company",
                Name = "Private Limited Company",
                Category = "Business Registration",
                CategorySlug = "business-registration",
                ShortDescription = "Incorporate your Private Limited Company with expert assistance. Includes name approval, DSC, SPICe+ filing, PAN, TAN & corporate bank account assistance.",
                Overview = "A Private Limited Company (Pvt Ltd) is India's most popular business structure, preferred by startups, investors, and growing enterprises. It offers limited liability protection to its shareholders, lets you raise equity capital easily, operates as a separate legal entity, and builds significant trust with global partners. Incorporating a company requires MCA approval via the SPICe+ web form.",
                PriceRange = "₹5,999 - ₹7,499",
                Timeline = "5 to 8 Business Days",
                Benefits = new List<string>
                {
                    "Limit personal liability of directors and shareholders to their unpaid share value",
                    "Build corporate credibility to attract venture capital, angel investment, and bank loans",
                    "Separate legal entity status allowing the company to hold property and sign contracts in its own name",
                    "Perpetual succession — the company continues to exist even if shareholders change",
                    "Tax efficiencies, structured equity ownership, and easy ESOP implementation"
                },
                DocumentsRequired = new List<string>
                {
                    "PAN Card of all proposed Directors (Minimum 2 directors required)",
                    "Aadhaar Card / Passport / Voter ID of all proposed Directors",
                    "Latest Bank Statement / Telephone Bill / Mobile Bill of all directors (not older than 2 months)",
                    "Utility Bill (Electricity/Gas/Water) of the registered office premises",
                    "No Objection Certificate (NOC) from the landlord of the premises",
                    "Passport-sized photographs of all directors"
                },
                ProcessSteps = new List<(string, string)>
                {
                    ("Apply for DSC", "We obtain Digital Signature Certificates (DSC) for both directors, required to sign electronic forms."),
                    ("Name Reservation (RUN)", "We apply for the approval of a unique company name on the MCA portal."),
                    ("Drafting Charter Docs", "We draft the Memorandum of Association (MOA) and Articles of Association (AOA) tailored to your business model."),
                    ("SPICe+ Incorporation Filing", "We file the comprehensive SPICe+ form including PAN, TAN, EPFO, ESIC, and Professional Tax details."),
                    ("Certificate Issued", "The Registrar of Companies (ROC) processes the files and issues the Certificate of Incorporation (COI).")
                },
                Faqs = new List<(string, string)>
                {
                    ("What is the minimum number of directors needed?", "You need a minimum of 2 directors, and at least one must be a resident of India."),
                    ("Can a Private Limited Company be registered at a residential address?", "Yes. The registered office can be a residential property. A utility bill and NOC are required."),
                    ("Is there a minimum capital requirement?", "No, there is no longer a statutory minimum paid-up capital requirement to start a Pvt Ltd company.")
                }
            });

            // Trademark Registration
            CuratedDetails.Add("trademark-registration", new ServiceDetail
            {
                Slug = "trademark-registration",
                Name = "Trademark Registration",
                Category = "Trademark & IPR",
                CategorySlug = "trademark-and-ipr",
                ShortDescription = "Protect your brand name, logo, and slogan. File your trademark application today to secure your intellectual property rights.",
                Overview = "A Trademark (TM) is a unique visual symbol, logo, word, name, or phrase that distinguishes your products or services from competitors. Registering your trademark grants you exclusive ownership rights, builds brand recognition, protects your reputation, and prevents unauthorized third parties from using similar marks that could confuse consumers.",
                PriceRange = "₹1,999 - ₹2,499 (Excluding Govt Fees)",
                Timeline = "2 to 3 Business Days (To use TM symbol)",
                Benefits = new List<string>
                {
                    "Acquire exclusive rights to use the brand name, logo, or slogan nationwide",
                    "Stop competitors from copying or riding on your established brand reputation",
                    "Create a valuable intangible asset that can be leased, franchised, or sold",
                    "Display the registered trademark symbol (®) once the registration certificate is issued",
                    "Establish global brand recognition and secure digital domains and social handles easily"
                },
                DocumentsRequired = new List<string>
                {
                    "Logo design or wordmark in high resolution (JPEG/PNG format)",
                    "Signed Authorization Letter (Form TM-48) allowing us to represent you",
                    "PAN Card and Aadhaar Card of the applicant",
                    "Partnership Deed / Incorporation Certificate (if applying as an entity)",
                    "MSME / Startup Certificate (optional, saves 50% on government registration fees)",
                    "Affidavit of brand usage proof (if the mark has been in use prior to the filing date)"
                },
                ProcessSteps = new List<(string, string)>
                {
                    ("Trademark Search", "We perform a thorough search in the government database to check for phonetic or visual conflicts."),
                    ("Select Class", "We identify the correct trademark class (1 to 45) that fits your specific business activities."),
                    ("Filing TM-A Application", "We draft and submit the online trademark application to the Controller General of Patents, Designs and Trademarks."),
                    ("Use TM Symbol", "As soon as the filing receipt is generated, you can legally start using the 'TM' symbol next to your logo/name."),
                    ("Objections & Publication", "We monitor the registry for examination reports. Once cleared, it is published in the TM Journal, leading to final registration.")
                },
                Faqs = new List<(string, string)>
                {
                    ("How long is a trademark valid?", "A registered trademark is valid for 10 years and can be renewed indefinitely every 10 years."),
                    ("Can I register a trademark globally?", "No, trademark registration is territorial. A registration in India protects you in India. For global rights, you file under the Madrid Protocol."),
                    ("What is a trademark objection?", "An objection is raised by the Trademark Examiner if the mark is generic, descriptive, or similar to an existing mark. We represent you to file a reply.")
                }
            });
        }
    }
}
