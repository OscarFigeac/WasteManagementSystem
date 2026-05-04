using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WasteManagementSystem.Data;
using WasteManagementSystem.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; 
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<WasteContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("WasteContext")));

builder.Services.AddScoped<IPasswordHasher<HouseDetails>, PasswordHasher<HouseDetails>>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

StripeConfiguration.ApiKey = "sk_test_51TNL8dGnoXtKQ2PxB5RqdbfD7VTu7rMbwCw86U4Ii0UGZ2bdpIwmlBKRF4X122fnsLp7R16w320hBWbgSjkadNN800qflddwWm";

app.Run();