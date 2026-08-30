namespace caportal.Models.Entities
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Category { get; set; } = "Taxation"; // GST & Tax, Corporate Law, Startup Advisory, Audit & Assurance
        public string Excerpt { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string FeaturedImagePath { get; set; } = "/images/services/gst-tax.svg";
        public string AuthorName { get; set; } = "CA Priya Mehta";
        public string AuthorRole { get; set; } = "FCA, Senior Partner";
        public string AuthorAvatar { get; set; } = "/images/ca/ca-priya-mehta.svg";
        public DateTime PublishedDate { get; set; } = DateTime.UtcNow;
        public bool IsPublished { get; set; } = true;
        public int ViewsCount { get; set; } = 120;
        public int ReadTimeMinutes { get; set; } = 5;
        public List<string> Tags { get; set; } = new();
        public string MetaTitle { get; set; } = string.Empty;
        public string MetaDescription { get; set; } = string.Empty;
        public string MetaKeywords { get; set; } = string.Empty;
    }
}
