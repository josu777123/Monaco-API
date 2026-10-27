using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoDiseñoWeb2.Dominio;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ProyectoDiseñoWeb2Context _context;
    private readonly IPasswordHasher<Usuario> _hasher;

    public AuthController(ProyectoDiseñoWeb2Context context, IPasswordHasher<Usuario> hasher)
    {
        _context = context;
        _hasher = hasher;
    }

    public class LoginDto
    {
        public string Correo { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class RegistroDto
    {
        public string Nombre { get; set; } = "";
        public string Correo { get; set; } = "";
        public string Password { get; set; } = "";
        public string Telefono { get; set; } = "";
        public string CI { get; set; } = "";
        public DateTime FechaNacimiento { get; set; }
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto datos)
    {
        var correo = datos.Correo.Trim().ToLowerInvariant();
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo.ToLower() == correo && u.Activo);
        if (usuario == null || string.IsNullOrWhiteSpace(datos.Password))
            return Unauthorized("Correo o contraseña incorrectos.");

        PasswordVerificationResult resultado;
        try { resultado = _hasher.VerifyHashedPassword(usuario, usuario.PasswordHash, datos.Password); }
        catch (FormatException) { return Unauthorized("Correo o contraseña incorrectos."); }
        if (resultado == PasswordVerificationResult.Failed)
            return Unauthorized("Correo o contraseña incorrectos.");
        if (usuario.Rol != "Administrador" && usuario.Rol != "Cliente")
            return Forbid();
        if (usuario.Rol == "Cliente" &&
            (!usuario.Id_Cliente.HasValue || !await _context.Clientes.AnyAsync(c => c.Id_Cliente == usuario.Id_Cliente && c.Activo)))
            return Unauthorized("Esta cuenta no tiene un cliente activo asociado.");

        await IniciarSesion(usuario);
        return Ok(DatosUsuario(usuario));
    }

    [HttpPost("Registro")]
    public async Task<IActionResult> Registro([FromBody] RegistroDto datos)
    {
        if (User.Identity?.IsAuthenticated == true)
            return StatusCode(403, "Cierra sesión antes de crear una cuenta nueva.");
        var correo = datos.Correo.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(datos.Nombre) || string.IsNullOrWhiteSpace(datos.Telefono) ||
            string.IsNullOrWhiteSpace(datos.CI) || !correo.Contains('@') ||
            datos.Password.Length < 8 || datos.FechaNacimiento == default ||
            datos.FechaNacimiento.Date > DateTime.UtcNow.Date)
            return BadRequest("Completa nombre, correo, contraseña (mínimo 8 caracteres), teléfono, CI y fecha de nacimiento válida.");
        if (await _context.Usuarios.AnyAsync(u => u.Correo.ToLower() == correo))
            return BadRequest("El correo ya está registrado.");

        await using var transaccion = await _context.Database.BeginTransactionAsync();
        var cliente = new Cliente
        {
            Nombre = datos.Nombre.Trim(), Telefono = datos.Telefono.Trim(), CI = datos.CI.Trim(),
            FechaNacimiento = datos.FechaNacimiento.ToUniversalTime(), Activo = true
        };
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
        var usuario = new Usuario
        {
            Correo = correo, Rol = "Cliente", Id_Cliente = cliente.Id_Cliente,
            PasswordHash = "", Activo = true
        };
        usuario.PasswordHash = _hasher.HashPassword(usuario, datos.Password);
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        await transaccion.CommitAsync();

        await IniciarSesion(usuario);
        return Ok(DatosUsuario(usuario));
    }

    [HttpGet("Yo")]
    public async Task<IActionResult> Yo()
    {
        if (User.Identity?.IsAuthenticated != true) return Ok(new { autenticado = false });
        var idTexto = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(idTexto, out var id)) return Ok(new { autenticado = false });
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id_Usuario == id && u.Activo);
        if (usuario == null) return Ok(new { autenticado = false });
        return Ok(DatosUsuario(usuario));
    }

    [HttpPost("Salir")]
    public async Task<IActionResult> Salir()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { mensaje = "Sesión cerrada." });
    }

    private async Task IniciarSesion(Usuario usuario)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id_Usuario.ToString()),
            new(ClaimTypes.Name, usuario.Correo),
            new(ClaimTypes.Role, usuario.Rol)
        };
        var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identidad), new AuthenticationProperties { IsPersistent = false });
    }

    private static object DatosUsuario(Usuario usuario) => new
    {
        autenticado = true,
        id_Usuario = usuario.Id_Usuario,
        id_Rol = usuario.Rol == "Administrador" ? 1 : 2,
        rol = usuario.Rol,
        correo = usuario.Correo,
        id_Cliente = usuario.Id_Cliente
    };
}
