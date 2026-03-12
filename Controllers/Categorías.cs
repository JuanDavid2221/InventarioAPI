using InventarioAPI.Models;
using InventarioAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventarioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }


        [HttpPost]
        public async Task<IActionResult> CrearCategoria([FromBody] Categoria categoria)
        {
            var nuevaCategoria = await _categoriaService.CrearCategoria(categoria);

            return Ok(nuevaCategoria);
        }

        // GET api/categorias/negocio/1
        [HttpGet("negocio/{negocioId}")]
        public async Task<IActionResult> ObtenerPorNegocio(int negocioId)
        {
            var categorias = await _categoriaService.ObtenerPorNegocio(negocioId);
            return Ok(categorias);
        }

        [HttpPut("activar/{id}")]
        public async Task<IActionResult> ActivarCategoria(int id)
        {
            var resultado = await _categoriaService.ActivarCategoria(id);

            if (!resultado)
                return NotFound();

            return Ok("Categoría activada");
        }

        [HttpPut("desactivar/{id}")]
        public async Task<IActionResult> DesactivarCategoria(int id)
        {
            var resultado = await _categoriaService.DesactivarCategoria(id);

            if (!resultado)
                return NotFound();

            return Ok("Categoría desactivada");
        }
    }
}