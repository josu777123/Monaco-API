using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProyectoDiseñoWeb2.Dominio;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ProyectoDiseñoWeb2Context")
    ?? throw new InvalidOperationException("Configura la conexión a PostgreSQL en appsettings.json.");

builder.Services.AddDbContext<ProyectoDiseñoWeb2Context>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "MonacoSesion";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // En producción utilizar HTTPS.
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.Events.OnRedirectToLogin = ctx => { ctx.Response.StatusCode = 401; return Task.CompletedTask; };
        options.Events.OnRedirectToAccessDenied = ctx => { ctx.Response.StatusCode = 403; return Task.CompletedTask; };
    });
builder.Services.AddAuthorization(options =>
{
    // Deshabilita los controladores CRUD antiguos no contemplados en los tres roles.
    options.AddPolicy("NoAcceso", policy => policy.RequireAssertion(_ => false));
});
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// No se cambian las tablas: la estructura proviene de las migraciones originales.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProyectoDiseñoWeb2Context>();
    await db.Database.MigrateAsync();

    if (!await db.Categorias.AnyAsync())
    {
        db.Categorias.Add(new Categoria { Nombre = "General", Descripcion = "Bebidas", Activo = true });
        await db.SaveChangesAsync();
    }

    // Cuenta de DEMOSTRACIÓN. Se crea solamente si no existe ese correo.
    const string adminEmail = "admin@monaco.local";
    if (!await db.Usuarios.AnyAsync(u => u.Correo == adminEmail))
    {
        var administrador = new Usuario
        {
            Correo = adminEmail,
            Rol = "Administrador", // Rol ID 1
            Activo = true,
            Id_Cliente = null,
            PasswordHash = ""
        };
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Usuario>>();
        administrador.PasswordHash = hasher.HashPassword(administrador, "Monaco2026!");
        db.Usuarios.Add(administrador);
        await db.SaveChangesAsync();
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Protección complementaria CSRF para todas las operaciones con cookies.
// Los formularios frontend van por el proxy /api desde localhost:4200.
app.Use(async (ctx, next) =>
{
    if (ctx.Request.Path.StartsWithSegments("/api") &&
        !HttpMethods.IsGet(ctx.Request.Method) &&
        !HttpMethods.IsHead(ctx.Request.Method) &&
        !HttpMethods.IsOptions(ctx.Request.Method))
    {
        var origin = ctx.Request.Headers.Origin.ToString();
        if (!string.IsNullOrEmpty(origin))
        {
            var allowed = new[] { "http://localhost:4200", "http://127.0.0.1:4200", "http://localhost:5051", "http://127.0.0.1:5051" };
            if (!allowed.Contains(origin, StringComparer.OrdinalIgnoreCase))
            {
                ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }
        }
    }
    await next();
});

app.MapControllers();
app.Run();
