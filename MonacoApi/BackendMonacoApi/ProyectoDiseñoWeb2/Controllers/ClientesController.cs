using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoDiseñoWeb2.Dominio;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "NoAcceso")]
public class ClientesController : ControllerBase
{
    private readonly ProyectoDiseñoWeb2Context _context;

    public ClientesController(ProyectoDiseñoWeb2Context context)
    {
        _context = context;
    }

    [HttpGet("listaReal")]
    public async Task<IActionResult> GetListaClientesReales()
    {
        var clientes = await (from c in _context.Clientes
                              where c.Activo == true
                              select c).ToListAsync();

        return Ok(clientes);
    }

    [HttpGet("BuscarCliente/{Id_Cliente}")]
    public async Task<IActionResult> BuscarCliente(int Id_Cliente)
    {
        var cliente = await (from c in _context.Clientes
                             where c.Id_Cliente == Id_Cliente
                             select c).FirstOrDefaultAsync();

        if (cliente == null)
            return NotFound("Cliente no existe");

        if (!cliente.Activo)
            return NotFound("El cliente se encuentra inactivo");

        return Ok(cliente);
    }

    [HttpPut("ActualizarCliente/{Id_Cliente}/{Nombre}/{Telefono}/{CI}/{FechaNacimiento}")]
    public async Task<IActionResult> ActualizarCliente(
        int Id_Cliente, string Nombre, string Telefono,
        string CI, DateTime FechaNacimiento)
    {
        var cliente = await (from c in _context.Clientes
                             where c.Id_Cliente == Id_Cliente
                             select c).FirstOrDefaultAsync();

        if (cliente == null)
            return NotFound("Cliente no encontrado");

        if (!cliente.Activo)
            return BadRequest("El cliente se encuentra inactivo");

        cliente.Nombre = Nombre;
        cliente.Telefono = Telefono;
        cliente.CI = CI;
        cliente.FechaNacimiento = FechaNacimiento;

        await _context.SaveChangesAsync();

        return Ok(cliente);
    }

    [HttpPost("Crear")]
    public async Task<IActionResult> CrearCliente(
        string Nombre, string Telefono, string CI,
        DateTime FechaNacimiento)
    {
        var existente = await (from c in _context.Clientes
                               where c.CI == CI
                               select c).FirstOrDefaultAsync();

        if (existente != null)
            return BadRequest("Ya existe un cliente con ese CI");

        Cliente cliente = new Cliente
        {
            Nombre = Nombre,
            Telefono = Telefono,
            CI = CI,
            FechaNacimiento = FechaNacimiento,
            Activo = true
        };

        await _context.Clientes.AddAsync(cliente);
        await _context.SaveChangesAsync();

        return Ok(cliente);
    }

    [HttpDelete("Borrar/{Id_Cliente}")]
    public async Task<IActionResult> BorrarCliente(int Id_Cliente)
    {
        var cliente = await (from c in _context.Clientes
                             where c.Id_Cliente == Id_Cliente
                             select c).FirstOrDefaultAsync();

        if (cliente == null)
            return NotFound("Cliente no encontrado");

        if (!cliente.Activo)
            return BadRequest("El cliente ya está inactivo");

        cliente.Activo = false;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
