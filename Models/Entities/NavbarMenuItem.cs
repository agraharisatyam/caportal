namespace caportal.Models.Entities
{
    public class NavbarMenuItem
    {
        public int Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int Order { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public string MegaMenuType { get; set; } = "None"; // "None", "Services", "FindExpert"
        public bool OpenInNewTab { get; set; } = false;
    }
}
