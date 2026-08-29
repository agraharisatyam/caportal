using Microsoft.EntityFrameworkCore;
using caportal.Models;
using caportal.Models.Entities;

namespace caportal.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<TodoItem> TodoItems { get; set; } = null!;
    public DbSet<SiteSettingsRecord> SiteSettings { get; set; } = null!;
    public DbSet<CoveredService> CoveredServices { get; set; } = null!;
    public DbSet<WhyChooseUsItem> WhyChooseUsItems { get; set; } = null!;
}

public class TodoItem
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public bool IsDone { get; set; }
}
