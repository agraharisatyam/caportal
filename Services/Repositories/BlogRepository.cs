using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using caportal.Data;
using caportal.Models.Entities;

namespace caportal.Services.Repositories
{
    public static class BlogRepository
    {
        private static IDbContextFactory<ApplicationDbContext>? _dbFactory;

        public static void Configure(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        private static ApplicationDbContext CreateDb()
        {
            if (_dbFactory == null)
                throw new InvalidOperationException("BlogRepository is not configured with IDbContextFactory.");
            return _dbFactory.CreateDbContext();
        }

        public static List<BlogPost> GetAll(bool includeUnpublished = false)
        {
            try
            {
                using var db = CreateDb();
                var query = db.BlogPosts.AsQueryable();
                if (!includeUnpublished)
                    query = query.Where(p => p.IsPublished);
                return query.OrderByDescending(p => p.PublishedDate).ToList();
            }
            catch
            {
                return new List<BlogPost>();
            }
        }

        public static BlogPost? GetBySlug(string slug)
        {
            try
            {
                using var db = CreateDb();
                var post = db.BlogPosts.FirstOrDefault(p => p.Slug.ToLower() == slug.ToLower());
                if (post != null)
                {
                    post.ViewsCount++;
                    db.SaveChanges();
                }
                return post;
            }
            catch
            {
                return null;
            }
        }

        public static BlogPost? GetById(int id)
        {
            try
            {
                using var db = CreateDb();
                return db.BlogPosts.FirstOrDefault(p => p.Id == id);
            }
            catch
            {
                return null;
            }
        }

        public static void Add(BlogPost post)
        {
            using var db = CreateDb();
            if (string.IsNullOrEmpty(post.Slug))
            {
                post.Slug = post.Title.ToLowerInvariant()
                    .Replace(" ", "-")
                    .Replace("&", "and")
                    .Replace("?", "")
                    .Replace(":", "")
                    .Replace("/", "-");
            }
            db.BlogPosts.Add(post);
            db.SaveChanges();
        }

        public static void Update(BlogPost post)
        {
            using var db = CreateDb();
            var existing = db.BlogPosts.FirstOrDefault(p => p.Id == post.Id);
            if (existing != null)
            {
                existing.Title = post.Title;
                existing.Slug = post.Slug;
                existing.Category = post.Category;
                existing.Excerpt = post.Excerpt;
                existing.Content = post.Content;
                existing.FeaturedImagePath = post.FeaturedImagePath;
                existing.AuthorName = post.AuthorName;
                existing.AuthorRole = post.AuthorRole;
                existing.AuthorAvatar = post.AuthorAvatar;
                existing.IsPublished = post.IsPublished;
                existing.ReadTimeMinutes = post.ReadTimeMinutes;
                existing.Tags = post.Tags;
                existing.MetaTitle = post.MetaTitle;
                existing.MetaDescription = post.MetaDescription;
                existing.MetaKeywords = post.MetaKeywords;
                db.SaveChanges();
            }
        }

        public static void Delete(int id)
        {
            using var db = CreateDb();
            var post = db.BlogPosts.FirstOrDefault(p => p.Id == id);
            if (post != null)
            {
                db.BlogPosts.Remove(post);
                db.SaveChanges();
            }
        }
    }
}
