using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoDiseñoWeb2.Dominio;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Cliente")]
public class PedidosController : ControllerBase
{
    private readonly ProyectoDiseñoWeb2Context _context;
    public PedidosController(ProyectoDiseñoWeb2Context context) { _context = context; }

    public class CrearPedidoDto
    {
        public int Id_Producto { get; set; }
        public int Cantidad { get; set; }
    }

    private async Task<int?> MiCliente()
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)) return null;
        return await _context.Usuarios.Where(u => u.Id_Usuario == id && u.Activo && u.Rol == "Cliente")
            .Join(_context.Clientes.Where(c => c.Activo), u => u.Id_Cliente, c => (int?)c.Id_Cliente, (u, c) => (int?)c.Id_Cliente)
            .FirstOrDefaultAsync();
    }

    [HttpGet("listaReal")]
    public async Task<IActionResult> GetListaPedidosReales()
    {
        var miCliente = await MiCliente();
        if (!miCliente.HasValue) return Unauthorized();
        var pedidos = await _context.Pedidos
            .Where(p => p.Id_Cliente == miCliente.Value)
            .OrderByDescending(p => p.Fecha).ToListAsync();
        return Ok(pedidos);
    }

    [HttpGet("BuscarPedido/{Id_Pedido}")]
    public async Task<IActionResult> BuscarPedido(int Id_Pedido)
    {
        var miCliente = await MiCliente();
        if (!miCliente.HasValue) return Unauthorized();
        var pedido = await _context.Pedidos.FirstOrDefaultAsync(p => p.Id_Pedido == Id_Pedido && p.Id_Cliente == miCliente.Value);
        return pedido == null ? NotFound("Pedido no encontrado.") : Ok(pedido);
    }

    [HttpPost("Crear")]
    public async Task<IActionResult> CrearPedido([FromBody] CrearPedidoDto datos)
    {
        var miCliente = await MiCliente();
        if (!miCliente.HasValue) return Unauthorized();
        if (datos.Cantidad < 1 || datos.Cantidad > 100) return BadRequest("La cantidad debe ser de 1 a 100.");

        await using var transaccion = await _context.Database.BeginTransactionAsync();
        var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id_Producto == datos.Id_Producto && p.Activo);
        if (producto == null) return NotFound("Producto no encontrado.");

        // Se descuenta stock solo si hay suficientes unidades, también con pedidos simultáneos.
        var actualizados = await _context.Productos
            .Where(p => p.Id_Producto == datos.Id_Producto && p.Activo && p.Stock >= datos.Cantidad)
            .ExecuteUpdateAsync(p => p.SetProperty(x => x.Stock, x => x.Stock - datos.Cantidad));
        if (actualizados == 0) return BadRequest("No hay stock suficiente.");

        var pedido = new Pedido
        {
            Id_Cliente = miCliente.Value, Fecha = DateTime.UtcNow,
            Total = producto.Precio * datos.Cantidad, Estado = "Pendiente",
            DireccionEntrega = "", ModalidadEntrega = "Recoger", Id_Sucursal = null
        };
        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();
        _context.DetallePedidos.Add(new DetallePedido
        {
            Id_Pedido = pedido.Id_Pedido, Id_Producto = producto.Id_Producto,
            Cantidad = datos.Cantidad, PrecioUnitario = producto.Precio
        });
        await _context.SaveChangesAsync();
        await transaccion.CommitAsync();
        return Ok(new { pedido.Id_Pedido, pedido.Id_Cliente, pedido.Total, pedido.Estado });
    }
}
