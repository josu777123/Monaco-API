
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoDiseñoWeb2.Dominio;

[Route("api/[controller]")]
[ApiController]
public class PedidosController : ControllerBase
{
    private readonly ProyectoDiseñoWeb2Context _context;

    public PedidosController(ProyectoDiseñoWeb2Context context)
    {
        _context = context;
    }

    [HttpGet("listaReal")]
    public async Task<IActionResult> GetListaPedidosReales()
    {
        var pedidos = await (from p in _context.Pedidos
                             where p.Estado != "Cancelado"
                             select p).ToListAsync();

        return Ok(pedidos);
    }

    [HttpGet("BuscarPedido/{Id_Pedido}")]
    public async Task<IActionResult> BuscarPedido(int Id_Pedido)
    {
        var pedido = await (from p in _context.Pedidos
                            where p.Id_Pedido == Id_Pedido
                            select p).FirstOrDefaultAsync();

        if (pedido == null)
            return NotFound("Pedido no encontrado");

        if (pedido.Estado == "Cancelado")
            return NotFound("El pedido se encuentra cancelado");

        return Ok(pedido);
    }

    [HttpPut("ActualizarPedido/{Id_Pedido}/{Id_Cliente}/{Fecha}/{Total}/{Estado}/{DireccionEntrega}/{ModalidadEntrega}/{Id_Sucursal?}")]
    public async Task<IActionResult> ActualizarPedido(
        int Id_Pedido, int Id_Cliente, DateTime Fecha,
        decimal Total, string Estado, string DireccionEntrega,
        string ModalidadEntrega, int? Id_Sucursal)
    {
        var pedido = await (from p in _context.Pedidos
                            where p.Id_Pedido == Id_Pedido
                            select p).FirstOrDefaultAsync();

        if (pedido == null)
            return NotFound("Pedido no encontrado");

        if (pedido.Estado == "Cancelado")
            return BadRequest("El pedido se encuentra cancelado");

        var cliente = await (from c in _context.Clientes
                             where c.Id_Cliente == Id_Cliente && c.Activo
                             select c).FirstOrDefaultAsync();

        if (cliente == null)
            return BadRequest("El cliente no existe o está inactivo");

        if (Id_Sucursal.HasValue)
        {
            var sucursal = await (from s in _context.Sucursales
                                  where s.Id_Sucursal == Id_Sucursal.Value
                                        && s.Activo
                                  select s).FirstOrDefaultAsync();

            if (sucursal == null)
                return BadRequest("La sucursal no existe o está inactiva");
        }

        if (Total < 0)
            return BadRequest("El total no puede ser negativo");

        pedido.Id_Cliente = Id_Cliente;
        pedido.Fecha = Fecha;
        pedido.Total = Total;
        pedido.Estado = Estado;
        pedido.DireccionEntrega = DireccionEntrega;
        pedido.ModalidadEntrega = ModalidadEntrega;
        pedido.Id_Sucursal = Id_Sucursal;

        await _context.SaveChangesAsync();

        return Ok(pedido);
    }

    [HttpPost("Crear")]
    public async Task<IActionResult> CrearPedido(
        int Id_Cliente, DateTime Fecha, decimal Total,
        string Estado, string DireccionEntrega,
        string ModalidadEntrega, int? Id_Sucursal)
    {
        var cliente = await (from c in _context.Clientes
                             where c.Id_Cliente == Id_Cliente && c.Activo
                             select c).FirstOrDefaultAsync();

        if (cliente == null)
            return BadRequest("El cliente no existe o está inactivo");

        if (Id_Sucursal.HasValue)
        {
            var sucursal = await (from s in _context.Sucursales
                                  where s.Id_Sucursal == Id_Sucursal.Value
                                        && s.Activo
                                  select s).FirstOrDefaultAsync();

            if (sucursal == null)
                return BadRequest("La sucursal no existe o está inactiva");
        }

        if (Total < 0)
            return BadRequest("El total no puede ser negativo");

        Pedido pedido = new Pedido
        {
            Id_Cliente = Id_Cliente,
            Fecha = Fecha,
            Total = Total,
            Estado = Estado,
            DireccionEntrega = DireccionEntrega,
            ModalidadEntrega = ModalidadEntrega,
            Id_Sucursal = Id_Sucursal
        };

        await _context.Pedidos.AddAsync(pedido);
        await _context.SaveChangesAsync();

        return Ok(pedido);
    }

    [HttpDelete("Borrar/{Id_Pedido}")]
    public async Task<IActionResult> BorrarPedido(int Id_Pedido)
    {
        var pedido = await (from p in _context.Pedidos
                            where p.Id_Pedido == Id_Pedido
                            select p).FirstOrDefaultAsync();

        if (pedido == null)
            return NotFound("Pedido no encontrado");

        if (pedido.Estado == "Cancelado")
            return BadRequest("El pedido ya está cancelado");

        pedido.Estado = "Cancelado";

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
