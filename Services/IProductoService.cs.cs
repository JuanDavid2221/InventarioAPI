namespace InventarioAPI.Services
{
    using InventarioAPI.Models;
    public interface IProductoService
    {
        Task<IEnumerable<Producto>> ObtenerPorNegocio(int negocioId);
        Task<Producto> CrearProducto(Producto producto);
        Task<IEnumerable<Producto>> ObtenerPorCategoria(int categoriaId);
        Task<bool> ActivarProducto(int id);
        Task<bool> DesactivarProducto(int id);

    }
}
