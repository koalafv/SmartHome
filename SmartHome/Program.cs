using Hubs; // Zwróæ uwagê, czy na pewno masz taki namespace dla PotHub, wczeœniej by³o Api.Hubs
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SmartHome.Models;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. REJESTRACJA US£UG (Kolejnoœæ dowolna)
// ==========================================
builder.Services.AddSignalR();

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.MapStaticAssets();

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<PotHub>("/pothub");
app.MapRazorPages().WithStaticAssets();

app.Run();