
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoDiseñoWeb2.Dominio;

[Route("api/[controller]")]
[ApiController]
public class UsuariosController : ControllerBase
{
    private readonly ProyectoDiseñoWeb2Context _context;

    public UsuariosController(ProyectoDiseñoWeb2Context context)
    {
        _context = context;
    }

    [HttpGet("listaReal")]
    public async Task<IActionResult> GetListaUsuariosReales()
    {
        var usuarios = await (from u in _context.Usuarios
                              where u.Activo == true
                              select new
                              {
                                  u.Id_Usuario,
                                  u.Correo,
                                  u.Rol,
                                  u.Id_Cliente,
                                  u.Activo
                              }).ToListAsync();

        return Ok(usuarios);
    }

    [HttpGet("BuscarUsuario/{Id_Usuario}")]
    public async Task<IActionResult> BuscarUsuario(int Id_Usuario)
    {
        var usuario = await (from u in _context.Usuarios
                             where u.Id_Usuario == Id_Usuario
                             select new
                             {
                                 u.Id_Usuario,
                                 u.Correo,
                                 u.Rol,
                                 u.Id_Cliente,
                                 u.Activo
                             }).FirstOrDefaultAsync();

        if (usuario == null)
            return NotFound("Usuario no encontrado");

        if (!usuario.Activo)
            return NotFound("El usuario se encuentra inactivo");

        return Ok(usuario);
    }

    [HttpPut("ActualizarUsuario/{Id_Usuario}")]
    public async Task<IActionResult> ActualizarUsuario(
        int Id_Usuario, [FromBody] Usuario datos)
    {
        var usuario = await (from u in _context.Usuarios
                             where u.Id_Usuario == Id_Usuario
                             select u).FirstOrDefaultAsync();

        if (usuario == null)
            return NotFound("Usuario no encontrado");

        if (!usuario.Activo)
            return BadRequest("El usuario se encuentra inactivo");

        var correoExistente = await _context.Usuarios
            .AnyAsync(u => u.Correo == datos.Correo
                        && u.Id_Usuario != Id_Usuario);

        if (correoExistente)
            return BadRequest("Ya existe un usuario con ese correo");

        if (datos.Id_Cliente.HasValue)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c =>
                    c.Id_Cliente == datos.Id_Cliente.Value && c.Activo);

            if (cliente == null)
                return BadRequest("El cliente no existe o está inactivo");
        }

        usuario.Correo = datos.Correo;
        usuario.Rol = datos.Rol;
        usuario.Id_Cliente = datos.Id_Cliente;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            usuario.Id_Usuario,
            usuario.Correo,
            usuario.Rol,
            usuario.Id_Cliente,
            usuario.Activo
        });
    }

    [HttpPost("Crear")]
    public async Task<IActionResult> CrearUsuario([FromBody] Usuario datos)
    {
        var correoExistente = await _context.Usuarios
            .AnyAsync(u => u.Correo == datos.Correo);

        if (correoExistente)
            return BadRequest("Ya existe un usuario con ese correo");

        if (string.IsNullOrWhiteSpace(datos.PasswordHash))
            return BadRequest("La contraseña es obligatoria");

        if (datos.Id_Cliente.HasValue)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c =>
                    c.Id_Cliente == datos.Id_Cliente.Value && c.Activo);

            if (cliente == null)
                return BadRequest("El cliente no existe o está inactivo");
        }

        Usuario usuario = new Usuario
        {
            Correo = datos.Correo,
            PasswordHash = datos.PasswordHash,
            Rol = datos.Rol,
            Id_Cliente = datos.Id_Cliente,
            Activo = true
        };

        await _context.Usuarios.AddAsync(usuario);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            usuario.Id_Usuario,
            usuario.Correo,
            usuario.Rol,
            usuario.Id_Cliente,
            usuario.Activo
        });
    }

    [HttpDelete("Borrar/{Id_Usuario}")]
    public async Task<IActionResult> BorrarUsuario(int Id_Usuario)
    {
        var usuario = await (from u in _context.Usuarios
                             where u.Id_Usuario == Id_Usuario
                             select u).FirstOrDefaultAsync();

        if (usuario == null)
            return NotFound("Usuario no encontrado");

        if (!usuario.Activo)
            return BadRequest("El usuario ya está inactivo");

        usuario.Activo = false;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
