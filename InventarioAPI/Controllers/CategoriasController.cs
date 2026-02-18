using InventarioAPI.Data;
using InventarioAPI.Models.Inventario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriasController : ControllerBase
    {
        private readonly InventarioContext _context;

        public CategoriasController(InventarioContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerCategorias()
        {
            return Ok(await _context.Categorias.ToListAsync());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CrearCategoria(Categoria categoria)
        {
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Categoría creada correctamente", categoria });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditarCategoria(int id, Categoria categoria)
        {
            if (id != categoria.Id) return BadRequest("ID incorrecto");
            _context.Entry(categoria).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok("Categoría actualizada");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EliminarCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return NotFound("No existe");
            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
            return Ok("Categoría eliminada");
        }
    }
}