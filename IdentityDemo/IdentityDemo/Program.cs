using IdentityDemo.Data;
using IdentityDemo.Filters;
using IdentityDemo.Initializers;
using IdentityDemo.Extensions;
using IdentityDemo.Services;
using IdentityDemo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<LogActionFilter>(); // Register LogActionFilter globally
});

// Configure the database context
var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(connection, ServerVersion.AutoDetect(connection));
});

// Configure ASP.NET Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Register your services
builder.Services.AddTransient<IRoleInitializer, RoleInitializer>(); // RoleInitializer
builder.Services.AddScoped<IEmailService, EmailService>(); // Register EmailService
builder.Services.AddScoped<LogActionFilter>(); // Register LogActionFilter

// Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Make the session cookie HTTP-only
    options.Cookie.IsEssential = true; // Make the session cookie essential
});

// Add distributed memory cache (required for session state)
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

// Seed roles and admin user on startup
using (var scope = app.Services.CreateScope())
{
    var roleInitializer = scope.ServiceProvider.GetRequiredService<IRoleInitializer>();
    await roleInitializer.SeedRolesAsync();
}

// Middleware pipeline setup
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
// Use session middleware
app.UseSession();
// Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Endpoint routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
