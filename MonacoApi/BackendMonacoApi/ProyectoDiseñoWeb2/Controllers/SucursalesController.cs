using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoDiseñoWeb2.Dominio;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "NoAcceso")]
public class SucursalesController : ControllerBase
{
    private readonly ProyectoDiseñoWeb2Context _context;

    public SucursalesController(ProyectoDiseñoWeb2Context context)
    {
        _context = context;
    }

    [HttpGet("listaReal")]
    public async Task<IActionResult> GetListaSucursalesReales()
    {
        var sucursales = await (from s in _context.Sucursales
                                where s.Activo == true
                                select s).ToListAsync();

        return Ok(sucursales);
    }

    [HttpGet("BuscarSucursal/{Id_Sucursal}")]
    public async Task<IActionResult> BuscarSucursal(int Id_Sucursal)
    {
        var sucursal = await (from s in _context.Sucursales
                              where s.Id_Sucursal == Id_Sucursal
                              select s).FirstOrDefaultAsync();

        if (sucursal == null)
            return NotFound("Sucursal no encontrada");

        if (!sucursal.Activo)
            return NotFound("La sucursal se encuentra inactiva");

        return Ok(sucursal);
    }

    [HttpPut("ActualizarSucursal/{Id_Sucursal}/{Nombre}/{Direccion}/{Ciudad}/{Telefono}/{Horario}")]
    public async Task<IActionResult> ActualizarSucursal(
        int Id_Sucursal, string Nombre, string Direccion,
        string Ciudad, string Telefono, string Horario)
    {
        var sucursal = await (from s in _context.Sucursales
                              where s.Id_Sucursal == Id_Sucursal
                              select s).FirstOrDefaultAsync();

        if (sucursal == null)
            return NotFound("Sucursal no encontrada");

        if (!sucursal.Activo)
            return BadRequest("La sucursal se encuentra inactiva");

        sucursal.Nombre = Nombre;
        sucursal.Direccion = Direccion;
        sucursal.Ciudad = Ciudad;
        sucursal.Telefono = Telefono;
        sucursal.Horario = Horario;

        await _context.SaveChangesAsync();

        return Ok(sucursal);
    }

    [HttpPost("Crear")]
    public async Task<IActionResult> CrearSucursal(
        string Nombre, string Direccion, string Ciudad,
        string Telefono, string Horario)
    {
        var existente = await (from s in _context.Sucursales
                               where s.Nombre == Nombre && s.Activo
                               select s).FirstOrDefaultAsync();

        if (existente != null)
            return BadRequest("Ya existe una sucursal activa con ese nombre");

        Sucursal sucursal = new Sucursal
        {
            Nombre = Nombre,
            Direccion = Direccion,
            Ciudad = Ciudad,
            Telefono = Telefono,
            Horario = Horario,
            Activo = true
        };

        await _context.Sucursales.AddAsync(sucursal);
        await _context.SaveChangesAsync();

        return Ok(sucursal);
    }

    [HttpDelete("Borrar/{Id_Sucursal}")]
    public async Task<IActionResult> BorrarSucursal(int Id_Sucursal)
    {
        var sucursal = await (from s in _context.Sucursales
                              where s.Id_Sucursal == Id_Sucursal
                              select s).FirstOrDefaultAsync();

        if (sucursal == null)
            return NotFound("Sucursal no encontrada");

        if (!sucursal.Activo)
            return BadRequest("La sucursal ya está inactiva");

        sucursal.Activo = false;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
