using System;

namespace caportal.Models.Entities
{
    public class ContentPage
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string MetaDescription { get; set; } = string.Empty;
        public string HtmlContent { get; set; } = string.Empty;
        public bool IsPublished { get; set; } = true;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
