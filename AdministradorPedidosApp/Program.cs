using AdministradorPedidosApp.Data;
using AdministradorPedidosApp.Interfaces;
using AdministradorPedidosApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddDbContext<AdministradorPedidosAppContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("AdministradorPedidosAppContext")
        ?? throw new InvalidOperationException("Error al conectarse."))
    );

builder.Services.AddScoped<IArticuloService, ArticuloService>();
builder.Services.AddScoped<IRubroService, RubroService>();
builder.Services.AddScoped<ICuponService, CuponService>();
builder.Services.AddScoped<ICategoriaCuponService, CategoriaCuponService>();

//builder.Services.AddHttpClient("WSCuponesClient", client =>
//{
//    client.BaseAddress = new Uri("https://localhost:7159/api");
//});

builder.Services.AddHttpClient("WSCuponesClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5203/api");
});

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
