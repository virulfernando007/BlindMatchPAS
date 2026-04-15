using BlindMatchPAS.Data;
using BlindMatchPAS.Models;
using BlindMatchPAS.Repositories;
using BlindMatchPAS.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ─── Database ─────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ─── Identity & Authentication ────────────────────────────────────────────────
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit           = true;
    options.Password.RequiredLength         = 6;
    options.Password.RequireUppercase       = true;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail         = true;
    options.SignIn.RequireConfirmedEmail    = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ─── Cookie Configuration ─────────────────────────────────────────────────────
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath       = "/Account/Login";
    options.LogoutPath      = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan  = TimeSpan.FromHours(8);
});

// ─── Repositories ─────────────────────────────────────────────────────────────
builder.Services.AddScoped<IProjectRepository,     ProjectRepository>();
builder.Services.AddScoped<IMatchRepository,       MatchRepository>();
builder.Services.AddScoped<IResearchAreaRepository, ResearchAreaRepository>();

// ─── Services ─────────────────────────────────────────────────────────────────
builder.Services.AddScoped<IMatchingService, MatchingService>();
builder.Services.AddScoped<IProjectService,  ProjectService>(); // Restored your team's real service
builder.Services.AddScoped<IBlindReviewService, MockProjectService>(); // Added your isolated mock service

// ─── MVC ──────────────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ─── Middleware Pipeline ──────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Supervisor}/{action=BlindReview}/{id?}");

// ---- Database Initialisation ----
// (Still commented out so it doesn't crash trying to find SQL Server!)
/* using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    await DbInitializer.SeedAsync(scope.ServiceProvider);
} */

app.Run();