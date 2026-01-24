using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RestoranProje1.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabaný Baðlantýsý (Zaten Vardý - Scoped)
builder.Services.AddDbContext<RestoranContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- EKLENEN KISIM BAÞLANGIÇ (Dependency Injection Tanýmlarý) ---

// [Ýster 15]: Dependency Injection
// Singleton: Uygulama kapanana kadar yaþayan servis (Ýstatistik Sayacý için)
builder.Services.AddSingleton<RestoranProje1.Services.IIstatistikService, RestoranProje1.Services.IstatistikService>();

// Transient: Her istekte yeniden oluþturulan servis (Resim Yükleme için hafif servis)
builder.Services.AddTransient<RestoranProje1.Services.IResimService, RestoranProje1.Services.ResimService>();

// --- EKLENEN KISIM BÝTÝÞ ---

// 2. Authentication (Giriþ Sistemi)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Pipeline Ayarlarý...
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
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();