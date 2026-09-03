using caportal.Models.Entities;

namespace caportal.Models.ViewModels;

public class HomeViewModel
{
    public List<CaProfessional> FeaturedProfessionals { get; set; } = [];
    public SiteStats Stats { get; set; } = new();
    public List<Testimonial> Testimonials { get; set; } = [];
    public List<FaqItem> Faqs { get; set; } = [];
    public List<CoveredService> Services { get; set; } = [];
    public List<WhyChooseUsItem> WhyChooseUsItems { get; set; } = [];
    public List<HeroBannerSlide> HeroBannerSlides { get; set; } = [];
    public List<PricingPlanEntity> PricingPlans { get; set; } = [];
}

public class SiteStats
{
    public string TotalCAs { get; set; } = "12K+";
    public string ClientSatisfaction { get; set; } = "98%";
    public string CasesHandled { get; set; } = "50K+";
    public string Cities { get; set; } = "200+";
}

public class Testimonial
{
    public string Text { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorRole { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
    public int Rating { get; set; } = 5;
}

public class FaqItem
{
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}
