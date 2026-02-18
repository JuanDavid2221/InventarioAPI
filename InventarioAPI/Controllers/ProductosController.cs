using InventarioAPI.Data;
using InventarioAPI.Models.Inventario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // 🔒 Nadie entra sin Token
    public class ProductosController : ControllerBase
    {
        private readonly InventarioContext _context;

        public ProductosController(InventarioContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerProductos()
        {
            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .OrderBy(p => p.Nombre)
                .ToListAsync();
            return Ok(productos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null) return NotFound("Producto no encontrado");
            return Ok(producto);
        }

        [HttpGet("buscar/{codigo}")]
        public async Task<IActionResult> BuscarPorCodigo(string codigo)
        {
            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.CodigoBarras == codigo);

            if (producto == null) return NotFound("Producto no encontrado con ese código");
            return Ok(producto);
        }

        [HttpGet("alertas")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ObtenerAlertasStock()
        {
            var productosBajos = await _context.Productos
                .Where(p => p.Stock <= p.StockMinimo && p.Activo)
                .Select(p => new {
                    p.Id,
                    p.Nombre,
                    p.Stock,
                    p.StockMinimo,
                    Estado = p.Stock == 0 ? "Agotado" : "Bajo Stock",
                    SugerenciaCompra = (p.StockMinimo * 2) - p.Stock
                })
                .ToListAsync();

            return Ok(productosBajos);
        }

        // 🟢 NUEVO: Descargar Lista para Excel (CSV)
        [HttpGet("descargar-lista-compras")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DescargarListaCompras()
        {
            var productosParaSurtir = await _context.Productos
                .Where(p => p.Stock <= p.StockMinimo && p.Activo)
                .ToListAsync();

            var csv = new System.Text.StringBuilder();
            // Encabezados del Excel
            csv.AppendLine("Producto;Stock Actual;Stock Minimo;Cuanto Comprar");

            foreach (var p in productosParaSurtir)
            {
                int cuantoComprar = (p.StockMinimo * 2) - p.Stock;
                csv.AppendLine($"{p.Nombre};{p.Stock};{p.StockMinimo};{cuantoComprar}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            string nombreArchivo = $"Plan_Surtido_{DateTime.Now:dd-MM-yyyy}.csv";

            return File(bytes, "text/csv", nombreArchivo);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CrearProducto(Producto producto)
        {
            var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == producto.CategoriaId);
            if (!categoriaExiste) return BadRequest("La categoría especificada no existe");

            var codigoDuplicado = await _context.Productos.AnyAsync(p => p.CodigoBarras == producto.CodigoBarras);
            if (codigoDuplicado) return BadRequest("Ya existe otro producto con este mismo código de barras");

            if (producto.PrecioVenta < producto.PrecioCompra)
                return BadRequest("El precio de venta no puede ser menor al costo");

            producto.FechaRegistro = DateTime.Now;
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Producto registrado correctamente", producto });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditarProducto(int id, Producto producto)
        {
            if (id != producto.Id) return BadRequest("El ID del producto no coincide");

            var productoDb = await _context.Productos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (productoDb == null) return NotFound("Producto no encontrado");

            _context.Entry(producto).State = EntityState.Modified;

            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException) { return StatusCode(500, "Error de concurrencia"); }

            return Ok(new { mensaje = "Producto actualizado exitosamente" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound("El producto no existe");

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"Producto '{producto.Nombre}' eliminado correctamente" });
        }
    }
}