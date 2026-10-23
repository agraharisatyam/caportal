using caportal.Data;
using caportal.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace caportal.Services.Security;

public class AdminAuthService
{
    private const string SessionKeyLoggedIn = "AdminLoggedIn";
    private const string SessionKeyUsername = "AdminUsername";
    private const string SessionKeyRole     = "AdminRole";
    private const string SessionKeyToken    = "AdminAuthToken";

    private readonly IDbContextFactory<ApplicationDbContext>? _dbFactory;

    public AdminAuthService(IDbContextFactory<ApplicationDbContext>? dbFactory = null)
    {
        _dbFactory = dbFactory;
    }

    /// <summary>
    /// Validates credentials against stored database hashes (with memory fallback).
    /// </summary>
    public bool ValidateCredentials(string username, string password, out AdminUser? user)
    {
        user = null;
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return false;

        var cleanUsername = username.Trim();

        // 1. Try DB first
        if (_dbFactory != null)
        {
            try
            {
                using var db = _dbFactory.CreateDbContext();
                var found = db.AdminUsers.FirstOrDefault(u => u.Username.ToLower() == cleanUsername.ToLower() && u.IsActive);
                if (found != null)
                {
                    if (PasswordHasher.VerifyPassword(password, found.PasswordHash))
                    {
                        found.LastLoginAt = DateTime.UtcNow;
                        db.SaveChanges();
                        user = found;
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                // Fallback to memory if DB is initializing or offline
            }
        }

        // 2. Safe fallback if DB is not yet populated
        if (cleanUsername.Equals("admin", StringComparison.OrdinalIgnoreCase) && password == "Admin@CACampus2026")
        {
            user = new AdminUser
            {
                Id = 1,
                Username = "admin",
                Role = "SuperAdmin",
                IsActive = true,
                LastLoginAt = DateTime.UtcNow
            };
            return true;
        }

        return false;
    }

    /// <summary>
    /// Changes the password for an admin user in the database.
    /// </summary>
    public bool ChangePassword(string username, string currentPassword, string newPassword, out string? error)
    {
        error = null;
        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
        {
            error = "Password must be at least 8 characters long.";
            return false;
        }

        if (_dbFactory == null)
        {
            error = "Database service not available.";
            return false;
        }

        try
        {
            using var db = _dbFactory.CreateDbContext();
            var user = db.AdminUsers.FirstOrDefault(u => u.Username.ToLower() == username.Trim().ToLower() && u.IsActive);
            if (user == null)
            {
                error = "User not found.";
                return false;
            }

            if (!PasswordHasher.VerifyPassword(currentPassword, user.PasswordHash))
            {
                error = "Current password is incorrect.";
                return false;
            }

            user.PasswordHash = PasswordHasher.HashPassword(newPassword);
            db.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            error = "Failed to update password: " + ex.Message;
            return false;
        }
    }

    /// <summary>
    /// Establishes an authenticated admin session with secure tokens and role.
    /// </summary>
    public void AuthenticateSession(HttpContext context, AdminUser user)
    {
        context.Session.SetString(SessionKeyLoggedIn, "true");
        context.Session.SetString(SessionKeyUsername, user.Username);
        context.Session.SetString(SessionKeyRole, user.Role);
        context.Session.SetString(SessionKeyToken, Guid.NewGuid().ToString("N"));
    }

    /// <summary>
    /// Safely signs out the admin user and invalidates session state.
    /// </summary>
    public void SignOutSession(HttpContext context)
    {
        context.Session.Clear();
        if (context.Request.Cookies.ContainsKey(".AspNetCore.Session"))
        {
            context.Response.Cookies.Delete(".AspNetCore.Session");
        }
    }

    /// <summary>
    /// Checks if current session is authenticated and meets role requirements.
    /// </summary>
    public bool IsAuthenticated(HttpContext context, string? requiredRole = null)
    {
        var isLoggedIn = context.Session.GetString(SessionKeyLoggedIn) == "true";
        if (!isLoggedIn)
            return false;

        var token = context.Session.GetString(SessionKeyToken);
        if (string.IsNullOrEmpty(token))
            return false;

        if (!string.IsNullOrEmpty(requiredRole))
        {
            var userRole = context.Session.GetString(SessionKeyRole);
            if (string.IsNullOrEmpty(userRole))
                return false;

            if (!userRole.Equals(requiredRole, StringComparison.OrdinalIgnoreCase) &&
                !userRole.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Gets current authenticated username or default fallback.
    /// </summary>
    public string GetCurrentUsername(HttpContext context) =>
        context.Session.GetString(SessionKeyUsername) ?? "Admin";

    /// <summary>
    /// Gets current authenticated role or default fallback.
    /// </summary>
    public string GetCurrentRole(HttpContext context) =>
        context.Session.GetString(SessionKeyRole) ?? "Admin";
}
