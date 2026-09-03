using System;
using System.Collections.Generic;
using System.Linq;
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
    public DbSet<HeroBannerSlide> HeroBannerSlides { get; set; } = null!;
    public DbSet<CaProfessional> CaProfessionals { get; set; } = null!;
    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<DashboardOrder> DashboardOrders { get; set; } = null!;
    public DbSet<ClientRequest> ClientRequests { get; set; } = null!;
    public DbSet<BlogPost> BlogPosts { get; set; } = null!;
    public DbSet<FaqItemEntity> Faqs { get; set; } = null!;
    public DbSet<TestimonialEntity> Testimonials { get; set; } = null!;
    public DbSet<PricingPlanEntity> PricingPlans { get; set; } = null!;
    public DbSet<ContentPage> ContentPages { get; set; } = null!;
    public DbSet<NewsletterSubscriber> NewsletterSubscribers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // CaProfessional
        modelBuilder.Entity<CaProfessional>(entity =>
        {
            entity.Property(e => e.Specialisations)
                .HasConversion(
                    v => string.Join(",", v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                );
            entity.Property(e => e.Rating)
                .HasPrecision(3, 2);
        });

        // BlogPost Tags
        modelBuilder.Entity<BlogPost>(entity =>
        {
            entity.Property(e => e.Tags)
                .HasConversion(
                    v => string.Join(",", v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList()
                );
        });

        // PricingPlan Features
        modelBuilder.Entity<PricingPlanEntity>(entity =>
        {
            entity.Property(e => e.Features)
                .HasConversion(
                    v => string.Join("\n", v),
                    v => v.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList()
                );
        });

        // DashboardOrder
        modelBuilder.Entity<DashboardOrder>(entity =>
        {
            entity.Property(e => e.AmountValue)
                .HasPrecision(18, 2);
        });
    }
}

public class TodoItem
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public bool IsDone { get; set; }
}
