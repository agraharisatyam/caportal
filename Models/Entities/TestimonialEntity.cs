namespace caportal.Models.Entities
{
    public class TestimonialEntity
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorRole { get; set; } = string.Empty;
        public string Initials { get; set; } = string.Empty;
        public int Rating { get; set; } = 5;
        public string AvatarPath { get; set; } = string.Empty;
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
