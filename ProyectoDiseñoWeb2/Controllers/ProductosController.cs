
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoDiseñoWeb2.Dominio;

[Route("api/[controller]")]
[ApiController]
public class ProductosController : ControllerBase
{
    private readonly ProyectoDiseñoWeb2Context _context;

    public ProductosController(ProyectoDiseñoWeb2Context context)
    {
        _context = context;
    }

    [HttpGet("listaReal")]
    public async Task<IActionResult> GetListaProductosReales()
    {
        var productos = await (from p in _context.Productos
                               where p.Activo == true
                               select p).ToListAsync();

        return Ok(productos);
    }

    [HttpGet("BuscarProducto/{Id_Producto}")]
    public async Task<IActionResult> BuscarProducto(int Id_Producto)
    {
        var producto = await (from p in _context.Productos
                              where p.Id_Producto == Id_Producto
                              select p).FirstOrDefaultAsync();

        if (producto == null)
            return NotFound("Producto no existe");

        if (!producto.Activo)
            return NotFound("El producto se encuentra inactivo");

        return Ok(producto);
    }

    [HttpPut("ActualizarProducto/{Id_Producto}/{Nombre}/{Marca}/{Descripcion}/{Precio}/{PorcentajeAlcohol}/{VolumenMl}/{Stock}/{Id_Categoria}")]
    public async Task<IActionResult> ActualizarProducto(
        int Id_Producto, string Nombre, string Marca,
        string Descripcion, decimal Precio, decimal PorcentajeAlcohol,
        int VolumenMl, int Stock, int Id_Categoria)
    {
        var producto = await (from p in _context.Productos
                              where p.Id_Producto == Id_Producto
                              select p).FirstOrDefaultAsync();

        if (producto == null)
            return NotFound("Producto no encontrado");

        if (!producto.Activo)
            return BadRequest("El producto se encuentra inactivo");

        var categoria = await (from c in _context.Categorias
                               where c.Id_Categoria == Id_Categoria
                                     && c.Activo
                               select c).FirstOrDefaultAsync();

        if (categoria == null)
            return BadRequest("Categoría no encontrada o inactiva");

        if (Precio < 0 || PorcentajeAlcohol < 0 ||
            PorcentajeAlcohol > 100 || VolumenMl <= 0 || Stock < 0)
            return BadRequest("Los valores del producto no son válidos");

        producto.Nombre = Nombre;
        producto.Marca = Marca;
        producto.Descripcion = Descripcion;
        producto.Precio = Precio;
        producto.PorcentajeAlcohol = PorcentajeAlcohol;
        producto.VolumenMl = VolumenMl;
        producto.Stock = Stock;
        producto.Id_Categoria = Id_Categoria;

        await _context.SaveChangesAsync();

        return Ok(producto);
    }

    [HttpPost("Crear")]
    public async Task<IActionResult> CrearProducto(
        string Nombre, string Marca, string Descripcion,
        decimal Precio, decimal PorcentajeAlcohol,
        int VolumenMl, int Stock, int Id_Categoria)
    {
        var categoria = await (from c in _context.Categorias
                               where c.Id_Categoria == Id_Categoria
                                     && c.Activo
                               select c).FirstOrDefaultAsync();

        if (categoria == null)
            return BadRequest("Categoría no encontrada o inactiva");

        if (Precio < 0 || PorcentajeAlcohol < 0 ||
            PorcentajeAlcohol > 100 || VolumenMl <= 0 || Stock < 0)
            return BadRequest("Los valores del producto no son válidos");

        Producto producto = new Producto
        {
            Nombre = Nombre,
            Marca = Marca,
            Descripcion = Descripcion,
            Precio = Precio,
            PorcentajeAlcohol = PorcentajeAlcohol,
            VolumenMl = VolumenMl,
            Stock = Stock,
            Id_Categoria = Id_Categoria,
            Activo = true
        };

        await _context.Productos.AddAsync(producto);
        await _context.SaveChangesAsync();

        return Ok(producto);
    }

    [HttpDelete("Borrar/{Id_Producto}")]
    public async Task<IActionResult> BorrarProducto(int Id_Producto)
    {
        var producto = await (from p in _context.Productos
                              where p.Id_Producto == Id_Producto
                              select p).FirstOrDefaultAsync();

        if (producto == null)
            return NotFound("Producto no encontrado");

        if (!producto.Activo)
            return BadRequest("El producto ya está inactivo");

        producto.Activo = false;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
