
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoDiseñoWeb2.Dominio;

[Route("api/[controller]")]
[ApiController]
public class CategoriasController : ControllerBase
{
    private readonly ProyectoDiseñoWeb2Context _context;

    public CategoriasController(ProyectoDiseñoWeb2Context context)
    {
        _context = context;
    }

    [HttpGet("listaReal")]
    public async Task<IActionResult> GetListaCategoriasReales()
    {
        var categorias = await (from c in _context.Categorias
                                where c.Activo == true
                                select c).ToListAsync();

        return Ok(categorias);
    }

    [HttpGet("BuscarCategoria/{Id_Categoria}")]
    public async Task<IActionResult> BuscarCategoria(int Id_Categoria)
    {
        var categoria = await (from c in _context.Categorias
                               where c.Id_Categoria == Id_Categoria
                               select c).FirstOrDefaultAsync();

        if (categoria == null)
            return NotFound("Categoría no existe");

        if (!categoria.Activo)
            return NotFound("La categoría se encuentra inactiva");

        return Ok(categoria);
    }

    [HttpPut("ActualizarCategoria/{Id_Categoria}/{Nombre}/{Descripcion}")]
    public async Task<IActionResult> ActualizarCategoria(
        int Id_Categoria, string Nombre, string Descripcion)
    {
        var categoria = await (from c in _context.Categorias
                               where c.Id_Categoria == Id_Categoria
                               select c).FirstOrDefaultAsync();

        if (categoria == null)
            return NotFound("Categoría no encontrada");

        if (!categoria.Activo)
            return BadRequest("La categoría se encuentra inactiva");

        categoria.Nombre = Nombre;
        categoria.Descripcion = Descripcion;

        await _context.SaveChangesAsync();

        return Ok(categoria);
    }

    [HttpPost("Crear")]
    public async Task<IActionResult> CrearCategoria(
        string Nombre, string Descripcion)
    {
        var existente = await (from c in _context.Categorias
                               where c.Nombre == Nombre
                               select c).FirstOrDefaultAsync();

        if (existente != null)
            return BadRequest("Ya existe una categoría con ese nombre");

        Categoria categoria = new Categoria
        {
            Nombre = Nombre,
            Descripcion = Descripcion,
            Activo = true
        };

        await _context.Categorias.AddAsync(categoria);
        await _context.SaveChangesAsync();

        return Ok(categoria);
    }

    [HttpDelete("Borrar/{Id_Categoria}")]
    public async Task<IActionResult> BorrarCategoria(int Id_Categoria)
    {
        var categoria = await (from c in _context.Categorias
                               where c.Id_Categoria == Id_Categoria
                               select c).FirstOrDefaultAsync();

        if (categoria == null)
            return NotFound("Categoría no encontrada");

        if (!categoria.Activo)
            return BadRequest("La categoría ya está inactiva");

        categoria.Activo = false;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
