using caportal.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace caportal.Services.Security;

public class AdminAuthService
{
    private const string SessionKeyLoggedIn = "AdminLoggedIn";
    private const string SessionKeyUsername = "AdminUsername";
    private const string SessionKeyRole     = "AdminRole";
    private const string SessionKeyToken    = "AdminAuthToken";

    // Pre-hashed default admin users with salted PBKDF2 hashes
    private readonly List<AdminUser> _users = new()
    {
        new AdminUser
        {
            Id = 1,
            Username = "ajs",
            // Hash for "ajs@1503"
            PasswordHash = PasswordHasher.HashPassword("ajs@1503"),
            Role = "SuperAdmin",
            IsActive = true
        },
        new AdminUser
        {
            Id = 2,
            Username = "admin",
            // Hash for "Admin@CACampus2026"
            PasswordHash = PasswordHasher.HashPassword("Admin@CACampus2026"),
            Role = "Admin",
            IsActive = true
        }
    };

    /// <summary>
    /// Validates credentials against stored salted hashes.
    /// </summary>
    public bool ValidateCredentials(string username, string password, out AdminUser? user)
    {
        user = null;
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return false;

        var found = _users.FirstOrDefault(u => u.Username.Equals(username.Trim(), StringComparison.OrdinalIgnoreCase) && u.IsActive);
        if (found == null)
            return false;

        if (PasswordHasher.VerifyPassword(password, found.PasswordHash))
        {
            user = found;
            user.LastLoginAt = DateTime.UtcNow;
            return true;
        }

        return false;
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
