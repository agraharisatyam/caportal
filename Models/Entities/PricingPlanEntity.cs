using System.Collections.Generic;

namespace caportal.Models.Entities
{
    public class PricingPlanEntity
    {
        public int Id { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public string PriceDisplay { get; set; } = string.Empty;
        public string BillingCycle { get; set; } = "/month";
        public string Description { get; set; } = string.Empty;
        public List<string> Features { get; set; } = new();
        public bool IsPopular { get; set; } = false;
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
