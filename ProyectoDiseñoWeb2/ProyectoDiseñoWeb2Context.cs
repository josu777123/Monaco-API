using Microsoft.EntityFrameworkCore;
using ProyectoDiseñoWeb2.Dominio;

public class ProyectoDiseñoWeb2Context(DbContextOptions<ProyectoDiseñoWeb2Context> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes { get; set; } = default!;
    public DbSet<Categoria> Categorias { get; set; } = default!;
    public DbSet<DetallePedido> DetallePedidos { get; set; } = default!;
    public DbSet<Pago> Pagos { get; set; } = default!;
    public DbSet<Pedido> Pedidos { get; set; } = default!;
    public DbSet<Producto> Productos { get; set; } = default!;
    public DbSet<SolicitudSoporte> SolicitudesSoporte   { get; set; } = default!;
    public DbSet<Sucursal> Sucursales { get; set; } = default!;
    public DbSet<Usuario> Usuarios { get; set; } = default!;
}
