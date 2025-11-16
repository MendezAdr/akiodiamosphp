using CrudCafeteria.Components;
using Microsoft.EntityFrameworkCore;
using CrudCafeteria.Data;
using CrudCafeteria.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
// -------------------------

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "auth_cookie";
        options.LoginPath = "/login";
        options.Cookie.HttpOnly = true;
    });

builder.Services.AddAntiforgery(); // <-- Asegúrate de que esté aquí
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();// Necesario para iniciar/cerrar sesión
// ------------------------------------------

// Add DbContext
builder.Services.AddDbContext<CafeteriaContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21))
    ));

// Tus servicios (esto está perfecto)
builder.Services.AddScoped<GastoService>();
builder.Services.AddScoped<IngresoService>();
builder.Services.AddScoped<UsuarioService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

//app.UseHttpsRedirection(); // Comentado (como lo tienes)
app.UseStaticFiles();

// --- ¡ESTE ES EL ORDEN CRÍTICO! ---
app.UseRouting(); // <-- Debe ir antes de Antiforgery, Authentication, Authorization

app.UseAntiforgery(); // <-- Aquí
app.UseAuthentication(); // <-- Aquí
app.UseAuthorization(); // <-- Aquí

app.MapRazorPages(); // <-- Después de los anteriores
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();