namespace caportal.Models.Entities
{
    public class DashboardOrder
    {
        public int Id { get; set; }
        public string OrderId { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public string Service { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
        public decimal AmountValue { get; set; } = 0;
        public string Status { get; set; } = "Processing";
        public string PaymentStatus { get; set; } = "Paid";
        public string ColorClass { get; set; } = "bg-warning";
        public string AssignedCA { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
