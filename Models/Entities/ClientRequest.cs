namespace caportal.Models.Entities
{
    public class ClientRequest
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ClientType { get; set; } = "Individual"; // Individual, Startup, SME, Corporate
        public string ServiceRequired { get; set; } = string.Empty;
        public string AssignedCA { get; set; } = "Unassigned";
        public string Status { get; set; } = "Pending"; // Pending, Assigned, In Progress, Completed, Cancelled
        public string Description { get; set; } = string.Empty;
        public string PreferredTime { get; set; } = string.Empty;
        public string Source { get; set; } = "Contact Form"; // Contact Form, Expert Profile, Homepage
        public DateTime RequestedOn { get; set; } = DateTime.UtcNow;
    }
}
