using System.Collections.Generic;

namespace caportal.Models.Entities
{
    public class ServiceDetail
    {
        public string Slug { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string CategorySlug { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string Overview { get; set; } = string.Empty;
        public string PriceRange { get; set; } = string.Empty;
        public string Timeline { get; set; } = string.Empty;
        public List<string> Benefits { get; set; } = new();
        public List<string> DocumentsRequired { get; set; } = new();
        public List<(string StepName, string StepDesc)> ProcessSteps { get; set; } = new();
        public List<(string Question, string Answer)> Faqs { get; set; } = new();
    }
}
