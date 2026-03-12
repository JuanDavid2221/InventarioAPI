using InventarioAPI.Models;
using InventarioAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace InventarioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;


        public ProductosController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpPost]
        public async Task<IActionResult> CrearProducto([FromBody] Producto producto)
        {
            var nuevoProducto = await _productoService.CrearProducto(producto);

            return Ok(nuevoProducto);
        }

        // GET api/productos/negocio/1
        [HttpGet("negocio/{negocioId}")]
        public async Task<IActionResult> ObtenerPorNegocio(int negocioId)
        {
            var productos = await _productoService.ObtenerPorNegocio(negocioId);
            return Ok(productos);
        }

        // GET api/productos/categoria/3
        [HttpGet("categoria/{categoriaId}")]
        public async Task<IActionResult> ObtenerPorCategoria(int categoriaId)
        {
            var productos = await _productoService.ObtenerPorCategoria(categoriaId);
            return Ok(productos);
        }

        [HttpPut("activar/{id}")]
        public async Task<IActionResult> ActivarProducto(int id)
        {
            var resultado = await _productoService.ActivarProducto(id);

            if (!resultado)
                return NotFound();

            return Ok("Producto activado");
        }

        [HttpPut("desactivar/{id}")]
        public async Task<IActionResult> DesactivarProducto(int id)
        {
            var resultado = await _productoService.DesactivarProducto(id);

            if (!resultado)
                return NotFound();

            return Ok("Producto desactivado");
        }
    }
}
