using LegelisteApp.Components;
using LegelisteApp.Data;
using LegelisteApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var adminSeedPassword = builder.Configuration["AdminSeed:Password"] ?? throw new InvalidOperationException("AdminSeed:Password not configured.");

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "LegelisteAuthCookie";
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/access-denied";
    });
    
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ExportService>();
builder.Services.AddScoped<DailyEntryService>();
builder.Services.AddScoped<FlockCalculationService>();
builder.Services.AddSingleton<PerformanceNormService>();
builder.Services.AddScoped<ChangelogService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
var supportedCultures = new[] { "de-DE" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo("de-DE");
System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo("de-DE");

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});

using (var scope = app.Services.CreateScope())
{
    var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    using var dbContext = dbContextFactory.CreateDbContext();
    dbContext.Database.Migrate();

    if (!dbContext.Users.Any(u => u.Username == "admin"))
    {
        var adminUser = new LegelisteApp.Data.Models.User
        {
            Username = "admin",
            Role = LegelisteApp.Data.Models.UserRole.Admin,
            PasswordHash = ""
        };
        var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<LegelisteApp.Data.Models.User>();
        adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, adminSeedPassword);
        dbContext.Users.Add(adminUser);
        dbContext.SaveChanges();
    }

    // --- Datenbank-Reparatur (Datenrettung) ---
    var currentAdmin = dbContext.Users.FirstOrDefault(u => u.Username == "admin");
    if (currentAdmin != null)
    {
        // Notfall-Rettung: Admin Account reaktivieren
        currentAdmin.IsActive = true;
        var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<LegelisteApp.Data.Models.User>();
        currentAdmin.PasswordHash = passwordHasher.HashPassword(currentAdmin, adminSeedPassword);
        dbContext.SaveChanges();

        var validUserIds = dbContext.Users.Select(u => u.Id).ToList();

        var orphanedCreators = dbContext.DailyEntries
            .Where(e => !validUserIds.Contains(e.CreatorId))
            .ToList();

        foreach (var entry in orphanedCreators)
        {
            entry.CreatorId = currentAdmin.Id;
        }

        var orphanedApprovers = dbContext.DailyEntries
            .Where(e => e.ApprovedById.HasValue && !validUserIds.Contains(e.ApprovedById.Value))
            .ToList();

        foreach (var entry in orphanedApprovers)
        {
            entry.ApprovedById = currentAdmin.Id;
        }

        if (orphanedCreators.Any() || orphanedApprovers.Any())
        {
            dbContext.SaveChanges();
        }
    }
}

app.Run();
