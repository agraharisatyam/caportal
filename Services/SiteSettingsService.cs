using caportal.Data;
using caportal.Models;
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
  --accent:           {s.AccentColor};
  --accent-light:     {s.AccentColorLight};
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
  font-family: '{s.BodyFontFamily}', sans-serif;
  font-size: {s.BodyFontSize}px;
}}
h1,h2,h3,h4,h5,h6 {{
  font-family: '{s.HeadingFont}', serif;
  color: {s.HeadingColor};
}}
h1 {{ font-size: {s.H1Size}px; }}
h2 {{ font-size: {s.H2Size}px; }}
h3 {{ font-size: {s.H3Size}px; }}
.ca-navbar {{
  background: {s.HeaderBgColor} !important;
  border-bottom: 1px solid rgba(212,175,55,0.2) !important;
}}
.ca-navbar .nav-link,
.ca-navbar .navbar-nav .nav-link {{
  color: {s.HeaderFontColor} !important;
  font-size: {s.HeaderFontSize}px !important;
  font-family: '{s.HeaderFontFamily}', sans-serif !important;
}}
.ca-navbar .nav-link:hover,
.ca-navbar .navbar-nav .nav-link:hover {{
  color: {s.AccentColor} !important;
  background: rgba(212,175,55,0.08) !important;
}}
.brand-name {{ color: {s.HeaderFontColor} !important; }}
.brand-ca   {{ color: {s.AccentColor} !important; }}
.nav-mega-menu {{
  background: #1E1E1E !important;
  border-color: rgba(212,175,55,0.2) !important;
}}
.nav-mega-menu::before {{ background: #1E1E1E !important; border-color: rgba(212,175,55,0.2) !important; }}
.mega-item {{ color: #FFFFFF !important; }}
.mega-item:hover {{ background: rgba(212,175,55,0.1) !important; color: {s.AccentColor} !important; }}
.mega-section-title {{ color: {s.AccentColor} !important; border-color: rgba(212,175,55,0.15) !important; }}
.btn-nav-login {{
  color: {s.AccentColor} !important;
  border-color: {s.AccentColor} !important;
  background: transparent !important;
}}
.btn-nav-login:hover {{ background: {s.AccentColor} !important; color: #111111 !important; }}
.btn-nav-cta {{
  background: linear-gradient(135deg, {s.AccentColor}, {s.AccentColorLight}) !important;
  color: {s.BtnPrimaryColor} !important;
  border-radius: {s.BtnBorderRadius}px !important;
}}
.hero-v2 {{
  background: linear-gradient(135deg, {s.HeroBgFrom} 0%, {s.HeroBgTo} 100%) !important;
}}
.hero-v2-title  {{ color: {s.HeroTitleColor} !important; }}
.hero-v2-sub    {{ color: {s.HeroSubColor} !important; }}
.hero-v2-gold   {{ background: linear-gradient(135deg,{s.AccentColor},{s.AccentColorLight}) !important; -webkit-background-clip:text !important; -webkit-text-fill-color:transparent !important; background-clip:text !important; }}
.ca-footer {{ background: {s.FooterBgColor} !important; color: {s.FooterFontColor} !important; }}
.footer-heading {{ color: {s.AccentColor} !important; }}
.btn-hero-primary {{
  background: linear-gradient(135deg,{s.AccentColor},{s.AccentColorLight}) !important;
  color: #111111 !important;
  border-radius: {s.BtnBorderRadius}px !important;
}}
.float-trigger-btn {{
  background: linear-gradient(135deg,{s.AccentColor},{s.AccentColorLight}) !important;
  color: #111111 !important;
}}
.float-form-title {{ color: {s.AccentColor} !important; }}
.float-label {{ color: {s.AccentColor} !important; }}
.float-submit {{
  background: linear-gradient(135deg,{s.AccentColor},{s.AccentColorLight}) !important;
  color: #111111 !important;
}}
";
    }

    // ── Private helpers ───────────────────────────────────────────────────
    private SiteSettings LoadFromDb()
    {
        using var db = _factory.CreateDbContext();
        var rec = db.SiteSettings.AsNoTracking().FirstOrDefault(r => r.Id == 1);
        return rec == null ? new SiteSettings() : MapToModel(rec);
    }

    private void PersistToDb(SiteSettings s)
    {
        using var db = _factory.CreateDbContext();
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
        HeroBgFrom       = r.HeroBgFrom,       HeroBgTo         = r.HeroBgTo,
        HeroTitleColor   = r.HeroTitleColor,   HeroSubColor     = r.HeroSubColor,
        HeroAccentColor  = r.HeroAccentColor,  HeroTitle        = r.HeroTitle,
        HeroSubtitle     = r.HeroSubtitle,
        AccentColor      = r.AccentColor,      AccentColorLight = r.AccentColorLight,
        PrimaryColor     = r.PrimaryColor,     PrimaryLight     = r.PrimaryLight,
        FooterBgColor    = r.FooterBgColor,    FooterFontColor  = r.FooterFontColor,
        FooterText       = r.FooterText,
        HeadingFont      = r.HeadingFont,      HeadingColor     = r.HeadingColor,
        H1Size           = r.H1Size,           H2Size           = r.H2Size,
        H3Size           = r.H3Size,
        BtnPrimaryBg     = r.BtnPrimaryBg,     BtnPrimaryColor  = r.BtnPrimaryColor,
        BtnBorderRadius  = r.BtnBorderRadius,
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
        r.HeroBgFrom       = s.HeroBgFrom;       r.HeroBgTo         = s.HeroBgTo;
        r.HeroTitleColor   = s.HeroTitleColor;   r.HeroSubColor     = s.HeroSubColor;
        r.HeroAccentColor  = s.HeroAccentColor;  r.HeroTitle        = s.HeroTitle;
        r.HeroSubtitle     = s.HeroSubtitle;
        r.AccentColor      = s.AccentColor;      r.AccentColorLight = s.AccentColorLight;
        r.PrimaryColor     = s.PrimaryColor;     r.PrimaryLight     = s.PrimaryLight;
        r.FooterBgColor    = s.FooterBgColor;    r.FooterFontColor  = s.FooterFontColor;
        r.FooterText       = s.FooterText;
        r.HeadingFont      = s.HeadingFont;      r.HeadingColor     = s.HeadingColor;
        r.H1Size           = s.H1Size;           r.H2Size           = s.H2Size;
        r.H3Size           = s.H3Size;
        r.BtnPrimaryBg     = s.BtnPrimaryBg;     r.BtnPrimaryColor  = s.BtnPrimaryColor;
        r.BtnBorderRadius  = s.BtnBorderRadius;
    }
}
