using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using C.Models;
using C.Services;
using C;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<UsuarioService>();

// Usar el Singleton para el DbContext
builder.Services.AddSingleton<CContext>(provider => DbContextSingleton.Instance);

// Configuraci�n de la sesi�n
builder.Services.AddSession(options =>
{
    options.Cookie.IsEssential = true; // asegura que la cookie de sesi�n sea esencial
    options.IdleTimeout = TimeSpan.FromMinutes(30); // tiempo de expiraci�n de la sesi�n
});

// Configuraci�n de la pol�tica de autorizaci�n basada en roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Administrador"));
    options.AddPolicy("RequireTeacherOrStudentRole", policy => policy.RequireRole("Maestro", "Estudiante"));
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Habilita el uso de sesiones
app.UseSession();

// Habilita la autenticaci�n y la autorizaci�n
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
