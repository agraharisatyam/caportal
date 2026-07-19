namespace caportal.Models;

/// <summary>
/// Single-row DB table that persists all site settings.
/// Id is always 1 (upserted, never grows).
/// </summary>
public class SiteSettingsRecord
{
    public int    Id               { get; set; } = 1;

    // Identity
    public string SiteName         { get; set; } = "CACampus";
    public string SiteTagline      { get; set; } = "Verified CA Network";
    public string SiteEmail        { get; set; } = "hello@cacampus.work.gd";
    public string SitePhone        { get; set; } = "+91 98765 43210";

    // Logo
    public string LogoPath         { get; set; } = "";
    public string LogoSmallPath    { get; set; } = "";
    public string LogoAlt          { get; set; } = "CACampus Logo";

    // Header
    public string HeaderBgColor    { get; set; } = "#111111";
    public string HeaderFontColor  { get; set; } = "#FFFFFF";
    public string HeaderFontSize   { get; set; } = "14";
    public string HeaderFontFamily { get; set; } = "Inter";
    public bool   HeaderSticky     { get; set; } = true;
    public bool   ShowTopBar       { get; set; } = true;
    public string TopBarBgColor    { get; set; } = "#0a0a0a";
    public string TopBarFontColor  { get; set; } = "#D4AF37";

    // Body
    public string BodyBgColor      { get; set; } = "#111111";
    public string BodyFontColor    { get; set; } = "#FFFFFF";
    public string BodyFontSize     { get; set; } = "15";
    public string BodyFontFamily   { get; set; } = "Inter";

    // Hero
    public string HeroBgFrom       { get; set; } = "#0a0a0a";
    public string HeroBgTo         { get; set; } = "#1a1200";
    public string HeroTitleColor   { get; set; } = "#FFFFFF";
    public string HeroSubColor     { get; set; } = "#BDBDBD";
    public string HeroAccentColor  { get; set; } = "#D4AF37";
    public string HeroTitle        { get; set; } = "CA & Legal Compliance Platform";
    public string HeroSubtitle     { get; set; } = "Connecting businesses with ICAI-verified Chartered Accountants across India.";

    // Brand Colors
    public string AccentColor      { get; set; } = "#D4AF37";
    public string AccentColorLight { get; set; } = "#F5D060";
    public string PrimaryColor     { get; set; } = "#D4AF37";
    public string PrimaryLight     { get; set; } = "#F5D060";

    // Footer
    public string FooterBgColor    { get; set; } = "#0a0a0a";
    public string FooterFontColor  { get; set; } = "#BDBDBD";
    public string FooterText       { get; set; } = "India's premier platform connecting businesses with ICAI-verified Chartered Accountants.";

    // Typography
    public string HeadingFont      { get; set; } = "Playfair Display";
    public string HeadingColor     { get; set; } = "#FFFFFF";
    public string H1Size           { get; set; } = "48";
    public string H2Size           { get; set; } = "36";
    public string H3Size           { get; set; } = "28";

    // Buttons
    public string BtnPrimaryBg     { get; set; } = "#D4AF37";
    public string BtnPrimaryColor  { get; set; } = "#111111";
    public string BtnBorderRadius  { get; set; } = "8";
}
