namespace caportal.Models;

public class SiteSettings
{
    // ── Website Identity ──────────────────────────────
    public string SiteName        { get; set; } = "CACampus";
    public string SiteTagline     { get; set; } = "Verified CA Network";
    public string SiteEmail       { get; set; } = "";
    public string SitePhone       { get; set; } = "+91 98765 43210";

    // ── Logo ──────────────────────────────────────────
    public string LogoPath        { get; set; } = "";   // e.g. /images/logo.png
    public string LogoSmallPath   { get; set; } = "";   // favicon/small logo
    public string LogoAlt         { get; set; } = "CACampus Logo";

    // ── Header ────────────────────────────────────────
    public string HeaderBgColor   { get; set; } = "#ffffff";
    public string HeaderFontColor { get; set; } = "#1a3c5e";
    public string HeaderFontSize  { get; set; } = "14";   // px
    public string HeaderFontFamily{ get; set; } = "Inter";
    public bool   HeaderSticky    { get; set; } = true;
    public bool   ShowTopBar      { get; set; } = true;
    public string TopBarBgColor   { get; set; } = "#0a1628";
    public string TopBarFontColor { get; set; } = "#ffffff";

    // ── Body ──────────────────────────────────────────
    public string BodyBgColor     { get; set; } = "#ffffff";
    public string BodyFontColor   { get; set; } = "#374151";
    public string BodyFontSize    { get; set; } = "15";   // px
    public string BodyFontFamily  { get; set; } = "Inter";

    // ── Hero Section ──────────────────────────────────
    public string HeroBgFrom      { get; set; } = "#0a1628";
    public string HeroBgTo        { get; set; } = "#1a2a4a";
    public string HeroTitleColor  { get; set; } = "#ffffff";
    public string HeroSubColor    { get; set; } = "rgba(255,255,255,0.72)";
    public string HeroAccentColor { get; set; } = "#d4a017";
    public string HeroTitle       { get; set; } = "CA & Legal Compliance Platform";
    public string HeroSubtitle    { get; set; } = "Connecting businesses with ICAI-verified Chartered Accountants across India.";

    // ── Accent / Brand Colors ─────────────────────────
    public string AccentColor     { get; set; } = "#d4a017";
    public string AccentColorLight{ get; set; } = "#f0c040";
    public string PrimaryColor    { get; set; } = "#1a3c5e";
    public string PrimaryLight    { get; set; } = "#2557a7";

    // ── Footer ────────────────────────────────────────
    public string FooterBgColor   { get; set; } = "#0d1f33";
    public string FooterFontColor { get; set; } = "rgba(255,255,255,0.7)";
    public string FooterText      { get; set; } = "India's premier platform connecting businesses with ICAI-verified Chartered Accountants.";

    // ── Typography ────────────────────────────────────
    public string HeadingFont     { get; set; } = "Playfair Display";
    public string HeadingColor    { get; set; } = "#0d1f33";
    public string H1Size          { get; set; } = "48";  // px
    public string H2Size          { get; set; } = "36";  // px
    public string H3Size          { get; set; } = "28";  // px

    // ── Buttons ───────────────────────────────────────
    public string BtnPrimaryBg    { get; set; } = "#d4a017";
    public string BtnPrimaryColor { get; set; } = "#0d1f33";
    public string BtnBorderRadius { get; set; } = "8";   // px
}
