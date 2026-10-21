
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoDiseñoWeb2.Dominio;

[Route("api/[controller]")]
[ApiController]
public class SolicitudesSoporteController : ControllerBase
{
    private readonly ProyectoDiseñoWeb2Context _context;

    public SolicitudesSoporteController(ProyectoDiseñoWeb2Context context)
    {
        _context = context;
    }

    [HttpGet("listaReal")]
    public async Task<IActionResult> GetListaSolicitudesReales()
    {
        var solicitudes = await (from s in _context.SolicitudesSoporte
                                 select s).ToListAsync();

        return Ok(solicitudes);
    }

    [HttpGet("BuscarSolicitud/{Id_Solicitud}")]
    public async Task<IActionResult> BuscarSolicitud(int Id_Solicitud)
    {
        var solicitud = await (from s in _context.SolicitudesSoporte
                               where s.Id_Solicitud == Id_Solicitud
                               select s).FirstOrDefaultAsync();

        if (solicitud == null)
            return NotFound("Solicitud de soporte no encontrada");

        return Ok(solicitud);
    }

    [HttpPut("ActualizarSolicitud/{Id_Solicitud}/{Nombre}/{Correo}/{Asunto}/{Mensaje}/{Estado}")]
    public async Task<IActionResult> ActualizarSolicitud(
        int Id_Solicitud, string Nombre, string Correo,
        string Asunto, string Mensaje, string Estado)
    {
        var solicitud = await (from s in _context.SolicitudesSoporte
                               where s.Id_Solicitud == Id_Solicitud
                               select s).FirstOrDefaultAsync();

        if (solicitud == null)
            return NotFound("Solicitud de soporte no encontrada");

        solicitud.Nombre = Nombre;
        solicitud.Correo = Correo;
        solicitud.Asunto = Asunto;
        solicitud.Mensaje = Mensaje;
        solicitud.Estado = Estado;

        await _context.SaveChangesAsync();

        return Ok(solicitud);
    }

    [HttpPost("Crear")]
    public async Task<IActionResult> CrearSolicitud(
        string Nombre, string Correo, string Asunto,
        string Mensaje, int? Id_Cliente)
    {
        if (Id_Cliente.HasValue)
        {
            var cliente = await (from c in _context.Clientes
                                 where c.Id_Cliente == Id_Cliente.Value
                                       && c.Activo
                                 select c).FirstOrDefaultAsync();

            if (cliente == null)
                return BadRequest("El cliente no existe o está inactivo");
        }

        SolicitudSoporte solicitud = new SolicitudSoporte
        {
            Nombre = Nombre,
            Correo = Correo,
            Asunto = Asunto,
            Mensaje = Mensaje,
            Fecha = DateTime.Now,
            Estado = "Pendiente",
            Id_Cliente = Id_Cliente
        };

        await _context.SolicitudesSoporte.AddAsync(solicitud);
        await _context.SaveChangesAsync();

        return Ok(solicitud);
    }

    [HttpDelete("Borrar/{Id_Solicitud}")]
    public async Task<IActionResult> BorrarSolicitud(int Id_Solicitud)
    {
        var solicitud = await (from s in _context.SolicitudesSoporte
                               where s.Id_Solicitud == Id_Solicitud
                               select s).FirstOrDefaultAsync();

        if (solicitud == null)
            return NotFound("Solicitud de soporte no encontrada");

        _context.SolicitudesSoporte.Remove(solicitud);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
