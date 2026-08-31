namespace caportal.Models.Entities;

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
    public string SiteEmail        { get; set; } = "";
    public string SitePhone        { get; set; } = "+91 90823 51112";

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
    public string PrimaryColor     { get; set; } = "#4F46E5";
    public string PrimaryLight     { get; set; } = "#6366F1";
    public string SecondaryColor   { get; set; } = "#10B981";

    // Section Visibility
    public bool ShowHeader             { get; set; } = true;
    public bool ShowHeroSection        { get; set; } = true;
    public bool ShowAboutUsSection     { get; set; } = true;
    public bool ShowServicesSection    { get; set; } = true;
    public bool ShowFeaturedCAsSection { get; set; } = true;
    public bool ShowTestimonialsSection { get; set; } = true;
    public bool ShowBlogSection        { get; set; } = true;
    public bool ShowCtaSection         { get; set; } = true;
    public bool ShowFooter             { get; set; } = true;

    // Footer
    public string FooterBgColor    { get; set; } = "#0a0a0a";
    public string FooterFontColor  { get; set; } = "#BDBDBD";
    public string FooterText       { get; set; } = "India's premier platform connecting businesses with ICAI-verified Chartered Accountants.";

    // Social Media
    public string SocialFacebook  { get; set; } = "";
    public string SocialInstagram { get; set; } = "";
    public string SocialLinkedIn  { get; set; } = "";
    public string SocialTwitter   { get; set; } = "";
    public string SocialYouTube   { get; set; } = "";
    public string SocialWhatsApp  { get; set; } = "";

    // Typography
    public string HeadingFont      { get; set; } = "Playfair Display";
    public string HeadingColor     { get; set; } = "#FFFFFF";
    public string H1Size           { get; set; } = "48";
    public string H2Size           { get; set; } = "36";
    public string H3Size           { get; set; } = "28";

    // Services Section
    public string ServicesBadge    { get; set; } = "What We Cover";
    public string ServicesTitle    { get; set; } = "Comprehensive Solutions <span>for Your Business</span>";

    // Why Choose Us Section
    public string WhyChooseUsBadge      { get; set; } = "WHY CHOOSE";
    public string WhyChooseUsTitle      { get; set; } = "CA CHARTERED CAMPUS?";
    public string WhyChooseUsSub        { get; set; } = "We combine expertise, technology and commitment to deliver reliable CA, legal and compliance solutions for individuals and businesses.";
    public string WhyChooseUsStatsTitle { get; set; } = "TRUSTED BY 50,000+ BUSINESSES";
    public string WhyChooseUsStat1Val   { get; set; } = "50,000+";
    public string WhyChooseUsStat1Lbl   { get; set; } = "Businesses Served";
    public string WhyChooseUsStat2Val   { get; set; } = "200+";
    public string WhyChooseUsStat2Lbl   { get; set; } = "Expert CAs";
    public string WhyChooseUsStat3Val   { get; set; } = "15+";
    public string WhyChooseUsStat3Lbl   { get; set; } = "Service Categories";
    public string WhyChooseUsStat4Val   { get; set; } = "24x7";
    public string WhyChooseUsStat4Lbl   { get; set; } = "Support Available";

    // Featured CAs Section
    public string FeaturedCAsBadge      { get; set; } = "Top Talent";
    public string FeaturedCAsTitle      { get; set; } = "Trusted Chartered Accountants <span>Across India</span>";
    public string FeaturedCAsSubtitle   { get; set; } = "Handpicked CAs with proven track records, top ratings, and deep domain expertise.";

    // Buttons
    public string BtnPrimaryBg     { get; set; } = "#D4AF37";
    public string BtnPrimaryColor  { get; set; } = "#111111";
    public string BtnBorderRadius  { get; set; } = "8";

    // Section Ordering
    public string SectionOrder     { get; set; } = "header,hero,about,services,portfolio,testimonials,blog,cta,footer";
}
