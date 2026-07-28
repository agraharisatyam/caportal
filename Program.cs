using Microsoft.EntityFrameworkCore;
using caportal.Data;
using caportal.Services;

var builder = WebApplication.CreateBuilder(args);

// ── MVC + Razor runtime compilation ──────────────────────────────────────
var mvcBuilder = builder.Services.AddControllersWithViews();
if (builder.Environment.IsDevelopment())
    mvcBuilder.AddRazorRuntimeCompilation();

// ── EF Core ───────────────────────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register scoped DbContext for controllers that inject it directly
builder.Services.AddScoped<ApplicationDbContext>(sp =>
    sp.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());

// ── Site Settings (DB-backed singleton cache) ─────────────────────────────
builder.Services.AddSingleton<SiteSettingsService>();

// ── Session (for simple admin auth) ──────────────────────────────────────
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout        = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly    = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite    = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

var app = builder.Build();

// ── Pipeline ──────────────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();          // must be BEFORE MapControllerRoute
app.UseAuthorization();

// ── SEO: sitemap and robots served dynamically ────────────────────────────

app.MapStaticAssets();

// ── Area route: /ajs  → Admin/Dashboard/Index ─────────────────────────
app.MapControllerRoute(
    name: "ajs_shortcut",
    pattern: "ajs",
    defaults: new { area = "Admin", controller = "Dashboard", action = "Index" });

// ── Area routes ───────────────────────────────────────────────────────────
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

// ── Explicit Admin routes (no area:exists constraint needed) ──────────────
app.MapControllerRoute(
    name: "admin_explicit",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}",
    defaults: new { area = "Admin" });

// ── Default route ─────────────────────────────────────────────────────────
app.MapControllerRoute(
    name: "service_detail",
    pattern: "service/{id:int}",
    defaults: new { controller = "Service", action = "Detail" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
