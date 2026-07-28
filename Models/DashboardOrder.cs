namespace caportal.Models
{
    public class DashboardOrder
    {
        public string OrderId { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public string Service { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ColorClass { get; set; } = string.Empty;
    }
}
