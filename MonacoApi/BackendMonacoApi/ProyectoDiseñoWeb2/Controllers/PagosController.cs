using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoDiseñoWeb2.Dominio;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "NoAcceso")]
public class PagosController : ControllerBase
{
    private readonly ProyectoDiseñoWeb2Context _context;

    public PagosController(ProyectoDiseñoWeb2Context context)
    {
        _context = context;
    }

    [HttpGet("listaReal")]
    public async Task<IActionResult> GetListaPagosReales()
    {
        var pagos = await (from p in _context.Pagos
                           select p).ToListAsync();

        return Ok(pagos);
    }

    [HttpGet("BuscarPago/{Id_Pago}")]
    public async Task<IActionResult> BuscarPago(int Id_Pago)
    {
        var pago = await (from p in _context.Pagos
                          where p.Id_Pago == Id_Pago
                          select p).FirstOrDefaultAsync();

        if (pago == null)
            return NotFound("Pago no encontrado");

        return Ok(pago);
    }

    [HttpPut("ActualizarPago/{Id_Pago}/{Id_Pedido}/{Monto}/{Metodo}/{Estado}/{Fecha}")]
    public async Task<IActionResult> ActualizarPago(
        int Id_Pago, int Id_Pedido, decimal Monto,
        string Metodo, string Estado, DateTime Fecha)
    {
        var pago = await (from p in _context.Pagos
                          where p.Id_Pago == Id_Pago
                          select p).FirstOrDefaultAsync();

        if (pago == null)
            return NotFound("Pago no encontrado");

        var pedido = await (from p in _context.Pedidos
                            where p.Id_Pedido == Id_Pedido
                            select p).FirstOrDefaultAsync();

        if (pedido == null)
            return BadRequest("El pedido no existe");

        if (Monto <= 0)
            return BadRequest("El monto debe ser mayor que cero");

        pago.Id_Pedido = Id_Pedido;
        pago.Monto = Monto;
        pago.Metodo = Metodo;
        pago.Estado = Estado;
        pago.Fecha = Fecha;

        await _context.SaveChangesAsync();

        return Ok(pago);
    }

    [HttpPost("Crear")]
    public async Task<IActionResult> CrearPago(
        int Id_Pedido, decimal Monto,
        string Metodo, string Estado, DateTime Fecha)
    {
        var pedido = await (from p in _context.Pedidos
                            where p.Id_Pedido == Id_Pedido
                            select p).FirstOrDefaultAsync();

        if (pedido == null)
            return BadRequest("El pedido no existe");

        if (Monto <= 0)
            return BadRequest("El monto debe ser mayor que cero");

        Pago pago = new Pago
        {
            Id_Pedido = Id_Pedido,
            Monto = Monto,
            Metodo = Metodo,
            Estado = Estado,
            Fecha = Fecha
        };

        await _context.Pagos.AddAsync(pago);
        await _context.SaveChangesAsync();

        return Ok(pago);
    }

    [HttpDelete("Borrar/{Id_Pago}")]
    public async Task<IActionResult> BorrarPago(int Id_Pago)
    {
        var pago = await (from p in _context.Pagos
                          where p.Id_Pago == Id_Pago
                          select p).FirstOrDefaultAsync();

        if (pago == null)
            return NotFound("Pago no encontrado");

        _context.Pagos.Remove(pago);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
