namespace caportal.Models.Entities;

public class CoveredService
{
    public int    Id           { get; set; }
    public string Icon         { get; set; } = "fas fa-briefcase";
    public string ImagePath    { get; set; } = "";
    public string Title        { get; set; } = string.Empty;
    public string Description  { get; set; } = string.Empty;
    public int    DisplayOrder { get; set; } = 0;
    public string PageUrl      { get; set; } = "#professionals"; // click destination
}
