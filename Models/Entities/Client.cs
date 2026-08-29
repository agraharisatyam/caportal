namespace caportal.Models.Entities;

public class Client
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Corporate / Startup / SME / Individual
    public string ContactEmail { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string AssignedCA { get; set; } = string.Empty;
    public string Service { get; set; } = string.Empty;
    public string Status { get; set; } = "Active"; // Active / Pending / Inactive
    public DateTime RegisteredOn { get; set; }
}
