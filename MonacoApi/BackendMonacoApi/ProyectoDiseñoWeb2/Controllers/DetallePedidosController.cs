using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoDiseñoWeb2.Dominio;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "NoAcceso")]
public class DetallePedidosController : ControllerBase
{
    private readonly ProyectoDiseñoWeb2Context _context;

    public DetallePedidosController(ProyectoDiseñoWeb2Context context)
    {
        _context = context;
    }

    [HttpGet("listaReal")]
    public async Task<IActionResult> GetListaDetallesReales()
    {
        var detalles = await (from d in _context.DetallePedidos
                              select d).ToListAsync();

        return Ok(detalles);
    }

    [HttpGet("BuscarDetalle/{Id_Detalle}")]
    public async Task<IActionResult> BuscarDetalle(int Id_Detalle)
    {
        var detalle = await (from d in _context.DetallePedidos
                             where d.Id_Detalle == Id_Detalle
                             select d).FirstOrDefaultAsync();

        if (detalle == null)
            return NotFound("Detalle de pedido no encontrado");

        return Ok(detalle);
    }

    [HttpPut("ActualizarDetalle/{Id_Detalle}/{Id_Pedido}/{Id_Producto}/{Cantidad}/{PrecioUnitario}")]
    public async Task<IActionResult> ActualizarDetalle(
        int Id_Detalle, int Id_Pedido, int Id_Producto,
        int Cantidad, decimal PrecioUnitario)
    {
        var detalle = await (from d in _context.DetallePedidos
                             where d.Id_Detalle == Id_Detalle
                             select d).FirstOrDefaultAsync();

        if (detalle == null)
            return NotFound("Detalle de pedido no encontrado");

        var pedido = await (from p in _context.Pedidos
                            where p.Id_Pedido == Id_Pedido
                            select p).FirstOrDefaultAsync();

        if (pedido == null || pedido.Estado == "Cancelado")
            return BadRequest("El pedido no existe o está cancelado");

        var producto = await (from p in _context.Productos
                              where p.Id_Producto == Id_Producto && p.Activo
                              select p).FirstOrDefaultAsync();

        if (producto == null)
            return BadRequest("El producto no existe o está inactivo");

        if (Cantidad <= 0 || PrecioUnitario < 0)
            return BadRequest("La cantidad o el precio no son válidos");

        detalle.Id_Pedido = Id_Pedido;
        detalle.Id_Producto = Id_Producto;
        detalle.Cantidad = Cantidad;
        detalle.PrecioUnitario = PrecioUnitario;

        await _context.SaveChangesAsync();

        return Ok(detalle);
    }

    [HttpPost("Crear")]
    public async Task<IActionResult> CrearDetalle(
        int Id_Pedido, int Id_Producto,
        int Cantidad, decimal PrecioUnitario)
    {
        var pedido = await (from p in _context.Pedidos
                            where p.Id_Pedido == Id_Pedido
                            select p).FirstOrDefaultAsync();

        if (pedido == null || pedido.Estado == "Cancelado")
            return BadRequest("El pedido no existe o está cancelado");

        var producto = await (from p in _context.Productos
                              where p.Id_Producto == Id_Producto && p.Activo
                              select p).FirstOrDefaultAsync();

        if (producto == null)
            return BadRequest("El producto no existe o está inactivo");

        if (Cantidad <= 0 || PrecioUnitario < 0)
            return BadRequest("La cantidad o el precio no son válidos");

        DetallePedido detalle = new DetallePedido
        {
            Id_Pedido = Id_Pedido,
            Id_Producto = Id_Producto,
            Cantidad = Cantidad,
            PrecioUnitario = PrecioUnitario
        };

        await _context.DetallePedidos.AddAsync(detalle);
        await _context.SaveChangesAsync();

        return Ok(detalle);
    }

    [HttpDelete("Borrar/{Id_Detalle}")]
    public async Task<IActionResult> BorrarDetalle(int Id_Detalle)
    {
        var detalle = await (from d in _context.DetallePedidos
                             where d.Id_Detalle == Id_Detalle
                             select d).FirstOrDefaultAsync();

        if (detalle == null)
            return NotFound("Detalle de pedido no encontrado");

        _context.DetallePedidos.Remove(detalle);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
