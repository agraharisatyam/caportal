namespace caportal.Models;

public class CaProfessional
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty; // FCA / ACA
    public int YearsExp { get; set; }
    public string City { get; set; } = string.Empty;
    public string[] Specialisations { get; set; } = [];
    public decimal Rating { get; set; }
    public int CasesHandled { get; set; }
    public string ResponseTime { get; set; } = string.Empty;
    public string MembershipNo { get; set; } = string.Empty;
    public string Status { get; set; } = "Active"; // Active / Pending / Suspended
    public bool IsVerified { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
    public DateTime JoinedOn { get; set; }
}
