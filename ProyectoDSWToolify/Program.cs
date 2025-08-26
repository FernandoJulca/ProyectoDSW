using Microsoft.AspNetCore.Authentication.Cookies;
using ProyectoDSWToolify.Models;
using ProyectoDSWToolify.Services.Contratos;
using ProyectoDSWToolify.Services.Implementacion;

var builder = WebApplication.CreateBuilder(args);

// 👇 Agrega servicios para MVC + sesiones
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 👇 Seguridad: Cookies compartidas con la API
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/UserAuth/Login";
        options.LogoutPath = "/Producto/Index";
        options.AccessDeniedPath = "/AccessDenied/Error";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

// 👇 Configurar consumo de la API (base URL en appsettings.json)
builder.Services.AddHttpClient<IClienteService, ClienteService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:URL_API"]);
});
builder.Services.AddHttpClient<IVentaService, VentaService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:URL_API"]);
});
builder.Services.AddHttpClient<IUserAuthService, UserAuthService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:URL_API"]);
});

var app = builder.Build();

// 👇 Middlewares de ejecución
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // 🟡 Para guardar datos de usuario
app.UseAuthentication(); // 🛡️ Cookies de login
app.UseAuthorization();

// 👇 MVC routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cliente}/{action=Index}/{id?}");

app.Run();
