using caportal.Models;

namespace caportal.Services;

/// <summary>
/// Singleton in-memory settings store.
/// Replace Get/Save with DB reads/writes when ready.
/// </summary>
public class SiteSettingsService
{
    private SiteSettings _settings = new();

    public SiteSettings Get() => _settings;

    public void Save(SiteSettings updated) => _settings = updated;

    /// <summary>Generates a dynamic CSS block from current settings.</summary>
    public string GenerateCss()
    {
        var s = _settings;
        return $@"
:root {{
  --primary:         {s.PrimaryColor};
  --primary-light:   {s.PrimaryLight};
  --accent:          {s.AccentColor};
  --accent-light:    {s.AccentColorLight};
  --body-bg:         {s.BodyBgColor};
  --body-color:      {s.BodyFontColor};
  --body-font-size:  {s.BodyFontSize}px;
  --body-font:       '{s.BodyFontFamily}', sans-serif;
  --heading-font:    '{s.HeadingFont}', serif;
  --heading-color:   {s.HeadingColor};
  --h1-size:         {s.H1Size}px;
  --h2-size:         {s.H2Size}px;
  --h3-size:         {s.H3Size}px;
  --header-bg:       {s.HeaderBgColor};
  --header-color:    {s.HeaderFontColor};
  --header-fs:       {s.HeaderFontSize}px;
  --topbar-bg:       {s.TopBarBgColor};
  --topbar-color:    {s.TopBarFontColor};
  --hero-bg-from:    {s.HeroBgFrom};
  --hero-bg-to:      {s.HeroBgTo};
  --hero-title:      {s.HeroTitleColor};
  --hero-sub:        {s.HeroSubColor};
  --footer-bg:       {s.FooterBgColor};
  --footer-color:    {s.FooterFontColor};
  --btn-primary-bg:  {s.BtnPrimaryBg};
  --btn-primary-clr: {s.BtnPrimaryColor};
  --btn-radius:      {s.BtnBorderRadius}px;
}}
body {{
  background-color: {s.BodyBgColor};
  color:            {s.BodyFontColor};
  font-family:      '{s.BodyFontFamily}', sans-serif;
  font-size:        {s.BodyFontSize}px;
}}
h1,h2,h3,h4,h5,h6 {{
  font-family: '{s.HeadingFont}', serif;
  color:       {s.HeadingColor};
}}
h1 {{ font-size: {s.H1Size}px; }}
h2 {{ font-size: {s.H2Size}px; }}
h3 {{ font-size: {s.H3Size}px; }}
.ca-navbar  {{ background: {s.HeaderBgColor} !important; }}
.ca-navbar .nav-link,
.ca-navbar .navbar-nav .nav-link  {{ color: {s.HeaderFontColor} !important; font-size: {s.HeaderFontSize}px !important; font-family: '{s.HeaderFontFamily}', sans-serif !important; }}
.topbar     {{ background: {s.TopBarBgColor} !important; }}
.topbar .topbar-btn {{ color: {s.TopBarFontColor} !important; }}
.hero-v2    {{ background: linear-gradient(135deg, {s.HeroBgFrom} 0%, {s.HeroBgTo} 100%) !important; }}
.hero-v2-title {{ color: {s.HeroTitleColor} !important; }}
.hero-v2-sub   {{ color: {s.HeroSubColor}   !important; }}
.hero-v2-gold,
.highlight     {{ color: {s.AccentColor}    !important; -webkit-text-fill-color: {s.AccentColor} !important; }}
.ca-footer  {{ background: {s.FooterBgColor} !important; color: {s.FooterFontColor} !important; }}
.btn-nav-cta, .consult-submit, .hero-v2-search-btn {{
  background: {s.BtnPrimaryBg} !important;
  color:      {s.BtnPrimaryColor} !important;
  border-radius: {s.BtnBorderRadius}px !important;
}}
.service-card:hover {{ border-color: {s.AccentColor} !important; }}
.section-badge {{ border-color: {s.AccentColor}; color: {s.PrimaryColor}; }}
";
    }
}
