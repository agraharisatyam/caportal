namespace caportal.Models;

public class WhyChooseUsItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty; // FontAwesome class
    public string ImagePath { get; set; } = string.Empty; // Optional uploaded image path
    public int DisplayOrder { get; set; } = 0;
}
