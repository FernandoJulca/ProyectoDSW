using ApiToolify.ChatHub;
using ApiToolify.Data.Contratos;
using ApiToolify.Data.Repositorios;
using Microsoft.AspNetCore.Authentication.Cookies;
using ProyectoDSWToolify.Data.Contratos;
using ProyectoDSWToolify.Data.Repositorios;
using ProyectoDSWToolify.Models;

var builder = WebApplication.CreateBuilder(args);

// ?? CORS para permitir al MVC acceder a la API con cookies
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp", policy =>
    {
        policy.WithOrigins("https://localhost:7108") // URL de tu proyecto MVC
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Necesario para cookies
    });
});

// ?? Autenticaci�n por cookies (asegura que SameSite=None y HTTPS)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SignalR
builder.Services.AddSignalR();

// ?? Inyecciones de dependencias
builder.Services.AddScoped<ICrud<Proveedor>, ProveRepo>();
builder.Services.AddScoped<ICrud<Distrito>, DistritoRepo>();
builder.Services.AddScoped<ICrud<Producto>, ProdRepo>();
builder.Services.AddScoped<ICrud<Categoria>, CateRepo>();
builder.Services.AddScoped<ICategoria, CategoriaRepo>();
builder.Services.AddScoped<IProducto, ProductoRepo>();
builder.Services.AddScoped<IUsuario, UsuarioRepo>();
builder.Services.AddScoped<IVenta, VentaRepo>();
builder.Services.AddScoped<IUserAuth, UserAuthRepository>();
builder.Services.AddScoped<IGrafico, GraficoRepositorio>();
builder.Services.AddScoped<IReporte, ReporteRepo>();

builder.Services.AddScoped<IReportes, RepositorioReportes>();

var app = builder.Build();

// ?? Middleware

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowWebApp");

app.UseAuthentication(); // ??? Necesario para usar claims y cookies
app.UseAuthorization();

app.MapControllers();

// ?? SignalR Hub
app.MapHub<ChatHub>("/chatHub");

app.Run();
