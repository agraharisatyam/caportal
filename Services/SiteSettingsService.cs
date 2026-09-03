using caportal.Data;
using caportal.Models;
using caportal.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace caportal.Services;

/// <summary>
/// Loads SiteSettings from SQL on first use, caches in memory,
/// and writes back to SQL on every Save / UploadLogo call.
/// </summary>
public class SiteSettingsService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;
    private SiteSettings? _cache;
    private readonly object _lock = new();

    public SiteSettingsService(IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
    }

    // ── Read ─────────────────────────────────────────────────────────────
    public SiteSettings Get()
    {
        if (_cache != null) return _cache;
        lock (_lock)
        {
            if (_cache != null) return _cache;
            _cache = LoadFromDb();
            return _cache;
        }
    }

    // ── Write ─────────────────────────────────────────────────────────────
    public void Save(SiteSettings updated)
    {
        // Keep logos if not explicitly overwritten
        if (_cache != null)
        {
            if (string.IsNullOrEmpty(updated.LogoPath))      updated.LogoPath      = _cache.LogoPath;
            if (string.IsNullOrEmpty(updated.LogoSmallPath)) updated.LogoSmallPath = _cache.LogoSmallPath;
        }

        lock (_lock) { _cache = updated; }
        PersistToDb(updated);
    }

    // ── CSS generator (unchanged) ─────────────────────────────────────────
    public string GenerateCss()
    {
        var s = Get();
        return $@"
/* ── CACampus Dynamic Theme ── */
:root {{
  --primary:          {s.PrimaryColor};
  --primary-light:    {s.PrimaryLight};
  --secondary:        {s.SecondaryColor};
  --accent:           {s.AccentColor};
  --accent-light:     {s.AccentColorLight};
  --dark:             {s.HeadingColor};
  --body-bg:          {s.BodyBgColor};
  --body-color:       {s.BodyFontColor};
  --body-font-size:   {s.BodyFontSize}px;
  --body-font:        '{s.BodyFontFamily}', sans-serif;
  --heading-font:     '{s.HeadingFont}', serif;
  --heading-color:    {s.HeadingColor};
  --h1-size:          {s.H1Size}px;
  --h2-size:          {s.H2Size}px;
  --h3-size:          {s.H3Size}px;
  --header-bg:        {s.HeaderBgColor};
  --header-color:     {s.HeaderFontColor};
  --topbar-bg:        {s.TopBarBgColor};
  --topbar-color:     {s.TopBarFontColor};
  --hero-bg-from:     {s.HeroBgFrom};
  --hero-bg-to:       {s.HeroBgTo};
  --footer-bg:        {s.FooterBgColor};
  --footer-color:     {s.FooterFontColor};
  --btn-bg:           {s.BtnPrimaryBg};
  --btn-color:        {s.BtnPrimaryColor};
  --btn-radius:       {s.BtnBorderRadius}px;
  --card-bg:          #1E1E1E;
  --card-border:      rgba(212,175,55,0.15);
  --secondary-text:   #BDBDBD;
}}

body {{
  background: {s.BodyBgColor};
  color: {s.BodyFontColor};
  font-family: '{s.BodyFontFamily}', sans-serif !important;
  font-size: {s.BodyFontSize}px;
}}

/* All Headings & Titles */
h1, h2, h3, h4, h5, h6,
.section-title,
.wcu-title,
.hero-title,
.hero-v2-title,
.step-card h5,
.feature-card h5,
.prof-name,
.testi-name,
.footer-heading,
.hcard-title,
.service-title {{
  font-family: '{s.HeadingFont}', serif !important;
  color: {s.HeadingColor} !important;
}}

/* Expert Band — Dark navy background requires white text & button */
.expert-band-title {{
  color: #ffffff !important;
}}
.expert-band-sub {{
  color: rgba(255, 255, 255, 0.88) !important;
}}
.expert-band .btn-hero-secondary {{
  color: #ffffff !important;
  border-color: rgba(255, 255, 255, 0.5) !important;
  background: rgba(255, 255, 255, 0.08) !important;
}}

/* CTA Section — Dark navy background requires white text & button */
.cta-section .section-title {{
  color: #ffffff !important;
}}
.cta-section .section-title span {{
  color: #f0c040 !important;
  background: linear-gradient(135deg, #ffd700 0%, #d4a017 100%) !important;
  -webkit-background-clip: text !important;
  -webkit-text-fill-color: transparent !important;
}}
.cta-section .section-desc {{
  color: rgba(255, 255, 255, 0.88) !important;
}}
.cta-section .section-badge {{
  color: #f0c040 !important;
  border-color: rgba(212, 160, 23, 0.45) !important;
  background: rgba(212, 160, 23, 0.15) !important;
}}
.cta-section .btn-cta-outline {{
  color: #ffffff !important;
  border-color: rgba(255, 255, 255, 0.45) !important;
  background: rgba(255, 255, 255, 0.08) !important;
}}

h1 {{ font-size: {s.H1Size}px !important; }}
h2, .section-title, .wcu-title {{ font-size: {s.H2Size}px !important; }}
h3 {{ font-size: {s.H3Size}px !important; }}

/* Subheaders / Spans / Highlights */
.section-title span,
.wcu-title span,
.hero-title .highlight,
.hero-v2-gold {{
  color: {s.SecondaryColor} !important;
  background: none !important;
  -webkit-text-fill-color: {s.SecondaryColor} !important;
}}

/* Subheaders / Descriptions / Paragraphs */
.section-desc,
.wcu-subtitle,
.hero-subtitle,
.hero-v2-sub,
.service-desc,
.wcu-card-desc,
.prof-designation,
.prof-meta,
.testi-text,
.testi-role,
.footer-tagline,
.footer-links a,
.footer-copy {{
  font-family: '{s.BodyFontFamily}', sans-serif !important;
}}

/* Section Badges / Accents */
.section-badge,
.wcu-badge-main,
.hero-badge,
.popular-badge {{
  color: {s.PrimaryColor} !important;
  border-color: {s.PrimaryColor} !important;
}}
.wcu-line, .wcu-header-icon {{
  background: {s.SecondaryColor} !important;
  color: {s.SecondaryColor} !important;
}}

/* Navbar & Header */
.ca-navbar {{
  background: {s.HeaderBgColor} !important;
  border-bottom: 1px solid rgba(212,175,55,0.2) !important;
}}
.ca-navbar.scrolled {{
  background: {s.HeaderBgColor} !important;
}}
.ca-navbar .nav-link,
.ca-navbar .navbar-nav .nav-link {{
  color: {s.HeaderFontColor} !important;
  font-size: {s.HeaderFontSize}px !important;
  font-family: '{s.BodyFontFamily}', sans-serif !important;
}}
.ca-navbar .nav-link:hover,
.ca-navbar .navbar-nav .nav-link:hover {{
  color: {s.SecondaryColor} !important;
}}
.brand-name {{ color: {s.HeaderFontColor} !important; }}
.brand-ca   {{ color: {s.SecondaryColor} !important; }}
.brand-icon {{
  background: linear-gradient(135deg, {s.PrimaryColor}, {s.SecondaryColor}) !important;
}}

/* Buttons */
.btn-hero-primary,
.btn-primary,
.btn-nav-cta,
.btn-pricing-primary,
.btn-connect,
.btn-cta-white {{
  background: {s.BtnPrimaryBg} !important;
  color: {s.BtnPrimaryColor} !important;
  border-radius: {s.BtnBorderRadius}px !important;
  border-color: {s.BtnPrimaryBg} !important;
}}
.btn-hero-primary:hover,
.btn-primary:hover,
.btn-nav-cta:hover,
.btn-pricing-primary:hover,
.btn-connect:hover,
.btn-cta-white:hover {{
  filter: brightness(1.1);
  color: {s.BtnPrimaryColor} !important;
}}
.btn-hero-secondary,
.btn-nav-login {{
  border-color: {s.PrimaryColor} !important;
  color: {s.PrimaryColor} !important;
  border-radius: {s.BtnBorderRadius}px !important;
}}

/* Cards, Icons & Hover Borders */
.service-card:hover,
.wcu-card:hover,
.prof-card:hover,
.testi-card:hover,
.feature-card:hover,
.pricing-card:hover {{
  border-color: {s.PrimaryColor} !important;
}}
.service-icon,
.wcu-card-icon-wrap {{
  border-color: {s.SecondaryColor} !important;
  color: {s.SecondaryColor} !important;
}}
.wcu-card:hover .wcu-card-icon-wrap {{
  background: {s.SecondaryColor} !important;
  color: #ffffff !important;
}}
.step-card:nth-child(odd) .step-number {{
  background: {s.PrimaryColor} !important;
  color: #ffffff !important;
}}
.step-card:nth-child(even) .step-number {{
  background: {s.SecondaryColor} !important;
  color: #ffffff !important;
}}
.prof-card-header {{
  background: linear-gradient(135deg, {s.PrimaryColor} 0%, {s.SecondaryColor} 100%) !important;
}}
.prof-tag {{
  color: {s.PrimaryColor} !important;
}}
.testi-quote,
.testi-stars,
.prof-rating {{
  color: {s.SecondaryColor} !important;
}}
.faq-accordion .accordion-button:not(.collapsed) {{
  color: {s.PrimaryColor} !important;
}}

/* Hero & CTA */
.hero-v2 {{
  background: linear-gradient(135deg, {s.HeroBgFrom} 0%, {s.HeroBgTo} 100%) !important;
}}
.hero-v2-title  {{ color: {s.HeroTitleColor} !important; }}
.hero-v2-sub    {{ color: {s.HeroSubColor} !important; }}
.ca-footer {{ background: {s.FooterBgColor} !important; color: {s.FooterFontColor} !important; }}
.footer-heading {{ color: {s.SecondaryColor} !important; }}
.footer-links a:hover {{ color: {s.SecondaryColor} !important; }}
.btn-gold-compact {{ background: {s.SecondaryColor} !important; }}
";
    }

    // ── Private helpers ───────────────────────────────────────────────────
    private static bool _columnsChecked = false;
    private static readonly object _columnLock = new();

    private static void EnsureColumnsExist(ApplicationDbContext db)
    {
        if (_columnsChecked) return;
        lock (_columnLock)
        {
            if (_columnsChecked) return;
            try
            {
                var cols = new[]
                {
                    ("SecondaryColor", "nvarchar(max) NOT NULL DEFAULT '#10B981'"),
                    ("ShowHeader", "bit NOT NULL DEFAULT 1"),
                    ("ShowHeroSection", "bit NOT NULL DEFAULT 1"),
                    ("ShowAboutUsSection", "bit NOT NULL DEFAULT 1"),
                    ("ShowServicesSection", "bit NOT NULL DEFAULT 1"),
                    ("ShowFeaturedCAsSection", "bit NOT NULL DEFAULT 1"),
                    ("ShowTestimonialsSection", "bit NOT NULL DEFAULT 1"),
                    ("ShowBlogSection", "bit NOT NULL DEFAULT 1"),
                    ("ShowCtaSection", "bit NOT NULL DEFAULT 1"),
                    ("ShowFooter", "bit NOT NULL DEFAULT 1"),
                    ("SectionOrder", "nvarchar(max) NOT NULL DEFAULT 'header,hero,about,services,portfolio,testimonials,blog,cta,footer'"),
                    ("HeroMode", "nvarchar(max) NOT NULL DEFAULT 'image'"),
                    ("HeroBannerImage", "nvarchar(max) NOT NULL DEFAULT '/images/hero-banner.png'"),
                    ("HeroBannerMobileImage", "nvarchar(max) NOT NULL DEFAULT ''"),
                    ("HeroBannerLink", "nvarchar(max) NOT NULL DEFAULT ''"),
                    ("HeroBannerAlt", "nvarchar(max) NOT NULL DEFAULT 'CA & Legal Compliance Platform'"),
                    ("HeroBadge", "nvarchar(max) NOT NULL DEFAULT '⭐ India''s #1 Verified CA Network'"),
                    ("HeroTitleHighlight", "nvarchar(max) NOT NULL DEFAULT 'Verified Chartered Accountants'"),
                    ("HeroPrimaryCtaText", "nvarchar(max) NOT NULL DEFAULT 'Find a CA'"),
                    ("HeroPrimaryCtaUrl", "nvarchar(max) NOT NULL DEFAULT '/find-expert'"),
                    ("HeroSecondaryCtaText", "nvarchar(max) NOT NULL DEFAULT 'Explore Services'"),
                    ("HeroSecondaryCtaUrl", "nvarchar(max) NOT NULL DEFAULT '/#features'"),
                    ("HeroShowSearch", "bit NOT NULL DEFAULT 1"),
                    ("HeroShowStats", "bit NOT NULL DEFAULT 1"),
                    ("HeroStat1Count", "nvarchar(max) NOT NULL DEFAULT '500+'"),
                    ("HeroStat1Label", "nvarchar(max) NOT NULL DEFAULT 'Verified CAs'"),
                    ("HeroStat2Count", "nvarchar(max) NOT NULL DEFAULT '10,000+'"),
                    ("HeroStat2Label", "nvarchar(max) NOT NULL DEFAULT 'Clients Served'"),
                    ("HeroStat3Count", "nvarchar(max) NOT NULL DEFAULT '99.4%'"),
                    ("HeroStat3Label", "nvarchar(max) NOT NULL DEFAULT 'Satisfaction Rate'"),
                    ("HeroStat4Count", "nvarchar(max) NOT NULL DEFAULT '150+'"),
                    ("HeroStat4Label", "nvarchar(max) NOT NULL DEFAULT 'Cities Across India'")
                };

                foreach (var (colName, colDef) in cols)
                {
                    var sql = $@"
                        IF NOT EXISTS (
                            SELECT 1 FROM sys.columns 
                            WHERE object_id = OBJECT_ID(N'SiteSettings') AND name = N'{colName}'
                        )
                        BEGIN
                            ALTER TABLE [SiteSettings] ADD [{colName}] {colDef};
                        END";
                    db.Database.ExecuteSqlRaw(sql);
                }
                _columnsChecked = true;
            }
            catch
            {
                // Fallback catch if columns exist or permissions differ
            }
        }
    }

    private SiteSettings LoadFromDb()
    {
        using var db = _factory.CreateDbContext();
        EnsureColumnsExist(db);
        try
        {
            var rec = db.SiteSettings.AsNoTracking().FirstOrDefault(r => r.Id == 1);
            return rec == null ? new SiteSettings() : MapToModel(rec);
        }
        catch
        {
            return new SiteSettings();
        }
    }

    private void PersistToDb(SiteSettings s)
    {
        using var db = _factory.CreateDbContext();
        EnsureColumnsExist(db);
        var rec = db.SiteSettings.Find(1);
        if (rec == null)
        {
            rec = new SiteSettingsRecord { Id = 1 };
            db.SiteSettings.Add(rec);
        }
        MapToRecord(s, rec);
        db.SaveChanges();
    }

    private static SiteSettings MapToModel(SiteSettingsRecord r) => new()
    {
        SiteName         = r.SiteName,        SiteTagline      = r.SiteTagline,
        SiteEmail        = r.SiteEmail,        SitePhone        = r.SitePhone,
        LogoPath         = r.LogoPath,         LogoSmallPath    = r.LogoSmallPath,
        LogoAlt          = r.LogoAlt,
        HeaderBgColor    = r.HeaderBgColor,    HeaderFontColor  = r.HeaderFontColor,
        HeaderFontSize   = r.HeaderFontSize,   HeaderFontFamily = r.HeaderFontFamily,
        HeaderSticky     = r.HeaderSticky,     ShowTopBar       = r.ShowTopBar,
        TopBarBgColor    = r.TopBarBgColor,    TopBarFontColor  = r.TopBarFontColor,
        BodyBgColor      = r.BodyBgColor,      BodyFontColor    = r.BodyFontColor,
        BodyFontSize     = r.BodyFontSize,     BodyFontFamily   = r.BodyFontFamily,
        HeroMode         = r.HeroMode,         HeroBannerImage  = r.HeroBannerImage,
        HeroBannerMobileImage = r.HeroBannerMobileImage,
        HeroBannerLink   = r.HeroBannerLink,   HeroBannerAlt    = r.HeroBannerAlt,
        HeroBgFrom       = r.HeroBgFrom,       HeroBgTo         = r.HeroBgTo,
        HeroTitleColor   = r.HeroTitleColor,   HeroSubColor     = r.HeroSubColor,
        HeroAccentColor  = r.HeroAccentColor,  HeroTitle        = r.HeroTitle,
        HeroTitleHighlight = r.HeroTitleHighlight,
        HeroSubtitle     = r.HeroSubtitle,     HeroBadge        = r.HeroBadge,
        HeroPrimaryCtaText = r.HeroPrimaryCtaText, HeroPrimaryCtaUrl = r.HeroPrimaryCtaUrl,
        HeroSecondaryCtaText = r.HeroSecondaryCtaText, HeroSecondaryCtaUrl = r.HeroSecondaryCtaUrl,
        HeroShowSearch   = r.HeroShowSearch,   HeroShowStats    = r.HeroShowStats,
        HeroStat1Count   = r.HeroStat1Count,   HeroStat1Label   = r.HeroStat1Label,
        HeroStat2Count   = r.HeroStat2Count,   HeroStat2Label   = r.HeroStat2Label,
        HeroStat3Count   = r.HeroStat3Count,   HeroStat3Label   = r.HeroStat3Label,
        HeroStat4Count   = r.HeroStat4Count,   HeroStat4Label   = r.HeroStat4Label,
        AccentColor      = r.AccentColor,      AccentColorLight = r.AccentColorLight,
        PrimaryColor     = r.PrimaryColor,     PrimaryLight     = r.PrimaryLight,
        SecondaryColor   = r.SecondaryColor,
        ShowHeader             = r.ShowHeader,
        ShowHeroSection        = r.ShowHeroSection,
        ShowAboutUsSection     = r.ShowAboutUsSection,
        ShowServicesSection    = r.ShowServicesSection,
        ShowFeaturedCAsSection = r.ShowFeaturedCAsSection,
        ShowTestimonialsSection = r.ShowTestimonialsSection,
        ShowBlogSection        = r.ShowBlogSection,
        ShowCtaSection         = r.ShowCtaSection,
        ShowFooter             = r.ShowFooter,
        FooterBgColor    = r.FooterBgColor,    FooterFontColor  = r.FooterFontColor,
        FooterText       = r.FooterText,
        HeadingFont      = r.HeadingFont,      HeadingColor     = r.HeadingColor,
        H1Size           = r.H1Size,           H2Size           = r.H2Size,
        H3Size           = r.H3Size,
        BtnPrimaryBg     = r.BtnPrimaryBg,     BtnPrimaryColor  = r.BtnPrimaryColor,
        BtnBorderRadius  = r.BtnBorderRadius,
        ServicesBadge    = r.ServicesBadge,    ServicesTitle    = r.ServicesTitle,
        WhyChooseUsBadge      = r.WhyChooseUsBadge,
        WhyChooseUsTitle      = r.WhyChooseUsTitle,
        WhyChooseUsSub        = r.WhyChooseUsSub,
        WhyChooseUsStatsTitle = r.WhyChooseUsStatsTitle,
        WhyChooseUsStat1Val   = r.WhyChooseUsStat1Val,
        WhyChooseUsStat1Lbl   = r.WhyChooseUsStat1Lbl,
        WhyChooseUsStat2Val   = r.WhyChooseUsStat2Val,
        WhyChooseUsStat2Lbl   = r.WhyChooseUsStat2Lbl,
        WhyChooseUsStat3Val   = r.WhyChooseUsStat3Val,
        WhyChooseUsStat3Lbl   = r.WhyChooseUsStat3Lbl,
        WhyChooseUsStat4Val   = r.WhyChooseUsStat4Val,
        WhyChooseUsStat4Lbl   = r.WhyChooseUsStat4Lbl,
        FeaturedCAsBadge      = r.FeaturedCAsBadge,
        FeaturedCAsTitle      = r.FeaturedCAsTitle,
        FeaturedCAsSubtitle   = r.FeaturedCAsSubtitle,
        SocialFacebook  = r.SocialFacebook,  SocialInstagram = r.SocialInstagram,
        SocialLinkedIn  = r.SocialLinkedIn,  SocialTwitter   = r.SocialTwitter,
        SocialYouTube   = r.SocialYouTube,   SocialWhatsApp  = r.SocialWhatsApp,
        SectionOrder          = r.SectionOrder,
    };

    private static void MapToRecord(SiteSettings s, SiteSettingsRecord r)
    {
        r.SiteName         = s.SiteName;        r.SiteTagline      = s.SiteTagline;
        r.SiteEmail        = s.SiteEmail;        r.SitePhone        = s.SitePhone;
        r.LogoPath         = s.LogoPath;         r.LogoSmallPath    = s.LogoSmallPath;
        r.LogoAlt          = s.LogoAlt;
        r.HeaderBgColor    = s.HeaderBgColor;    r.HeaderFontColor  = s.HeaderFontColor;
        r.HeaderFontSize   = s.HeaderFontSize;   r.HeaderFontFamily = s.HeaderFontFamily;
        r.HeaderSticky     = s.HeaderSticky;     r.ShowTopBar       = s.ShowTopBar;
        r.TopBarBgColor    = s.TopBarBgColor;    r.TopBarFontColor  = s.TopBarFontColor;
        r.BodyBgColor      = s.BodyBgColor;      r.BodyFontColor    = s.BodyFontColor;
        r.BodyFontSize     = s.BodyFontSize;     r.BodyFontFamily   = s.BodyFontFamily;
        r.HeroMode         = s.HeroMode;         r.HeroBannerImage  = s.HeroBannerImage;
        r.HeroBannerMobileImage = s.HeroBannerMobileImage;
        r.HeroBannerLink   = s.HeroBannerLink;   r.HeroBannerAlt    = s.HeroBannerAlt;
        r.HeroBgFrom       = s.HeroBgFrom;       r.HeroBgTo         = s.HeroBgTo;
        r.HeroTitleColor   = s.HeroTitleColor;   r.HeroSubColor     = s.HeroSubColor;
        r.HeroAccentColor  = s.HeroAccentColor;  r.HeroTitle        = s.HeroTitle;
        r.HeroTitleHighlight = s.HeroTitleHighlight;
        r.HeroSubtitle     = s.HeroSubtitle;     r.HeroBadge        = s.HeroBadge;
        r.HeroPrimaryCtaText = s.HeroPrimaryCtaText; r.HeroPrimaryCtaUrl = s.HeroPrimaryCtaUrl;
        r.HeroSecondaryCtaText = s.HeroSecondaryCtaText; r.HeroSecondaryCtaUrl = s.HeroSecondaryCtaUrl;
        r.HeroShowSearch   = s.HeroShowSearch;   r.HeroShowStats    = s.HeroShowStats;
        r.HeroStat1Count   = s.HeroStat1Count;   r.HeroStat1Label   = s.HeroStat1Label;
        r.HeroStat2Count   = s.HeroStat2Count;   r.HeroStat2Label   = s.HeroStat2Label;
        r.HeroStat3Count   = s.HeroStat3Count;   r.HeroStat3Label   = s.HeroStat3Label;
        r.HeroStat4Count   = s.HeroStat4Count;   r.HeroStat4Label   = s.HeroStat4Label;
        r.AccentColor      = s.AccentColor;      r.AccentColorLight = s.AccentColorLight;
        r.PrimaryColor     = s.PrimaryColor;     r.PrimaryLight     = s.PrimaryLight;
        r.SecondaryColor   = s.SecondaryColor;
        r.ShowHeader             = s.ShowHeader;
        r.ShowHeroSection        = s.ShowHeroSection;
        r.ShowAboutUsSection     = s.ShowAboutUsSection;
        r.ShowServicesSection    = s.ShowServicesSection;
        r.ShowFeaturedCAsSection = s.ShowFeaturedCAsSection;
        r.ShowTestimonialsSection = s.ShowTestimonialsSection;
        r.ShowBlogSection        = s.ShowBlogSection;
        r.ShowCtaSection         = s.ShowCtaSection;
        r.ShowFooter             = s.ShowFooter;
        r.FooterBgColor    = s.FooterBgColor;    r.FooterFontColor  = s.FooterFontColor;
        r.FooterText       = s.FooterText;
        r.HeadingFont      = s.HeadingFont;      r.HeadingColor     = s.HeadingColor;
        r.H1Size           = s.H1Size;           r.H2Size           = s.H2Size;
        r.H3Size           = s.H3Size;
        r.BtnPrimaryBg     = s.BtnPrimaryBg;     r.BtnPrimaryColor  = s.BtnPrimaryColor;
        r.BtnBorderRadius  = s.BtnBorderRadius;
        r.ServicesBadge    = s.ServicesBadge;    r.ServicesTitle    = s.ServicesTitle;
        r.WhyChooseUsBadge      = s.WhyChooseUsBadge;
        r.WhyChooseUsTitle      = s.WhyChooseUsTitle;
        r.WhyChooseUsSub        = s.WhyChooseUsSub;
        r.WhyChooseUsStatsTitle = s.WhyChooseUsStatsTitle;
        r.WhyChooseUsStat1Val   = s.WhyChooseUsStat1Val;
        r.WhyChooseUsStat1Lbl   = s.WhyChooseUsStat1Lbl;
        r.WhyChooseUsStat2Val   = s.WhyChooseUsStat2Val;
        r.WhyChooseUsStat2Lbl   = s.WhyChooseUsStat2Lbl;
        r.WhyChooseUsStat3Val   = s.WhyChooseUsStat3Val;
        r.WhyChooseUsStat3Lbl   = s.WhyChooseUsStat3Lbl;
        r.WhyChooseUsStat4Val   = s.WhyChooseUsStat4Val;
        r.WhyChooseUsStat4Lbl   = s.WhyChooseUsStat4Lbl;
        r.FeaturedCAsBadge      = s.FeaturedCAsBadge;
        r.FeaturedCAsTitle      = s.FeaturedCAsTitle;
        r.FeaturedCAsSubtitle   = s.FeaturedCAsSubtitle;
        r.SocialFacebook  = s.SocialFacebook;  r.SocialInstagram = s.SocialInstagram;
        r.SocialLinkedIn  = s.SocialLinkedIn;  r.SocialTwitter   = s.SocialTwitter;
        r.SocialYouTube   = s.SocialYouTube;   r.SocialWhatsApp  = s.SocialWhatsApp;
        r.SectionOrder    = s.SectionOrder;
    }
}
