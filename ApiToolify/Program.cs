using ApiToolify.ChatHub;
using Microsoft.AspNetCore.SignalR;
using ProyectoDSWToolify.Data.Contratos;
using ProyectoDSWToolify.Data.Repositorios;
using ProyectoDSWToolify.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5211") 
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

#region Inyeccion de dependecias

builder.Services.AddScoped<ICrud<Proveedor>, ProveRepo>();
builder.Services.AddScoped<ICrud<Distrito>, DistritoRepo>();
builder.Services.AddScoped<ICrud<Producto>, ProdRepo>();
builder.Services.AddScoped<ICrud<Categoria>, CateRepo>();

builder.Services.AddScoped<ICategoria, CategoriaRepo>();
builder.Services.AddScoped<IProducto, ProductoRepo>();
builder.Services.AddScoped<IUsuario, UsuarioRepo>();
builder.Services.AddScoped<IVenta, VentaRepo>();

#endregion

var app = builder.Build();

app.UseCors("AllowWebApp");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//Llamamos al HUB
app.MapHub<ChatHub>("/chatHub");

app.Run();



