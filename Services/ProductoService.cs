namespace InventarioAPI.Services
{
    using InventarioAPI.Data;
    using Microsoft.EntityFrameworkCore;
    using InventarioAPI.Models;


    public class ProductoService : IProductoService
    {
        private readonly InventarioContext _context;

        public ProductoService(InventarioContext context)
        {
            _context = context;
        }

        public async Task<Producto> CrearProducto(Producto producto)
        {
            _context.Productos.Add(producto);

            await _context.SaveChangesAsync();

            return producto;
        }

        public async Task<IEnumerable<Producto>> ObtenerPorNegocio(int negocioId)
        {
            return await _context.Productos
                .Where(p => p.NegocioId == negocioId)
                .Include(p => p.Categoria)
                .ToListAsync();
        }

        public async Task<IEnumerable<Producto>> ObtenerPorCategoria(int categoriaId)
        {
            return await _context.Productos
                .Where(p => p.CategoriaId == categoriaId)
                .Include(p => p.Categoria)
                .ToListAsync();
        }

        public async Task<bool> ActivarProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
                return false;

            producto.Activo = true;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DesactivarProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
                return false;

            producto.Activo = false;

            await _context.SaveChangesAsync();

            return true;
        }
    }


}
