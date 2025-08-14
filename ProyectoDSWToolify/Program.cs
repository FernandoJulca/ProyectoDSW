
using ProyectoDSWToolify.Models;
using ProyectoDSWToolify.Services.Contratos;
using ProyectoDSWToolify.Services.Implementacion;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache(); 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
#region Inyeccion de URl
builder.Services.AddHttpClient<IClienteService, ClienteService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:URL_API"]);
});
builder.Services.AddHttpClient<IVentaService, VentaService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:URL_API"]);
});
#endregion 
builder.Services.AddControllersWithViews();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cliente}/{action=Index}/{id?}");


app.Run();
