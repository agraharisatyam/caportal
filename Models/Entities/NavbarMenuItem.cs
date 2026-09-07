namespace caportal.Models.Entities
{
    public class NavbarMenuItem
    {
        public int Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int Order { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public string MegaMenuType { get; set; } = "None"; // "None", "CustomDropdown", "Services", "TaxLegal", "FindExpert"
        public bool OpenInNewTab { get; set; } = false;

        // Child dropdown links for Custom Dropdowns
        public List<NavbarDropdownItem> DropdownItems { get; set; } = new();
    }

    public class NavbarDropdownItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? Category { get; set; } = string.Empty; // Grouping / section name
        public string? Icon { get; set; } = string.Empty; // e.g. "fas fa-star"
        public string? Badge { get; set; } = string.Empty; // e.g. "New", "Popular"
        public int Order { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public bool OpenInNewTab { get; set; } = false;
    }
}
