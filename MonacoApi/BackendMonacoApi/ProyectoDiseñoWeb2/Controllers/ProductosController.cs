using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoDiseñoWeb2.Dominio;

[Route("api/[controller]")]
[ApiController]
public class ProductosController : ControllerBase
{
    private readonly ProyectoDiseñoWeb2Context _context;
    public ProductosController(ProyectoDiseñoWeb2Context context) { _context = context; }

    public class ProductoDto
    {
        public string Nombre { get; set; } = "";
        public string Marca { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public decimal Precio { get; set; }
        public decimal PorcentajeAlcohol { get; set; }
        public int VolumenMl { get; set; }
        public int Stock { get; set; }
        public int Id_Categoria { get; set; }
    }

    [HttpGet("listaReal")]
    public async Task<IActionResult> GetListaProductosReales()
    {
        return Ok(await _context.Productos.Where(p => p.Activo).OrderBy(p => p.Id_Producto).ToListAsync());
    }

    [HttpGet("BuscarProducto/{Id_Producto}")]
    public async Task<IActionResult> BuscarProducto(int Id_Producto)
    {
        var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id_Producto == Id_Producto && p.Activo);
        return producto == null ? NotFound("Producto no encontrado.") : Ok(producto);
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost("Crear")]
    public async Task<IActionResult> CrearProducto([FromBody] ProductoDto datos)
    {
        var error = await Validar(datos);
        if (error != null) return BadRequest(error);
        var producto = new Producto
        {
            Nombre = datos.Nombre.Trim(), Marca = datos.Marca.Trim(), Descripcion = datos.Descripcion.Trim(),
            Precio = datos.Precio, PorcentajeAlcohol = datos.PorcentajeAlcohol,
            VolumenMl = datos.VolumenMl, Stock = datos.Stock, Id_Categoria = datos.Id_Categoria, Activo = true
        };
        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();
        return Ok(producto);
    }

    [Authorize(Roles = "Administrador")]
    [HttpPut("ActualizarProducto/{Id_Producto}")]
    public async Task<IActionResult> ActualizarProducto(int Id_Producto, [FromBody] ProductoDto datos)
    {
        var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id_Producto == Id_Producto && p.Activo);
        if (producto == null) return NotFound("Producto no encontrado.");
        var error = await Validar(datos);
        if (error != null) return BadRequest(error);
        producto.Nombre = datos.Nombre.Trim();
        producto.Marca = datos.Marca.Trim();
        producto.Descripcion = datos.Descripcion.Trim();
        producto.Precio = datos.Precio;
        producto.PorcentajeAlcohol = datos.PorcentajeAlcohol;
        producto.VolumenMl = datos.VolumenMl;
        producto.Stock = datos.Stock;
        producto.Id_Categoria = datos.Id_Categoria;
        await _context.SaveChangesAsync();
        return Ok(producto);
    }

    [Authorize(Roles = "Administrador")]
    [HttpDelete("Borrar/{Id_Producto}")]
    public async Task<IActionResult> BorrarProducto(int Id_Producto)
    {
        var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id_Producto == Id_Producto && p.Activo);
        if (producto == null) return NotFound("Producto no encontrado.");
        producto.Activo = false; // Borrado lógico, conserva histórico de pedidos.
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private async Task<string?> Validar(ProductoDto datos)
    {
        if (string.IsNullOrWhiteSpace(datos.Nombre) || string.IsNullOrWhiteSpace(datos.Marca) ||
            datos.Precio < 0 || datos.PorcentajeAlcohol < 0 || datos.PorcentajeAlcohol > 100 ||
            datos.VolumenMl <= 0 || datos.Stock < 0) return "Los datos del producto no son válidos.";
        if (!await _context.Categorias.AnyAsync(c => c.Id_Categoria == datos.Id_Categoria && c.Activo))
            return "La categoría no existe o está inactiva.";
        return null;
    }
}
