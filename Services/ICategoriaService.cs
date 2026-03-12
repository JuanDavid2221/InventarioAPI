namespace InventarioAPI.Services
{
    using InventarioAPI.Models;
    public interface ICategoriaService
    {
        Task<Categoria> CrearCategoria(Categoria categoria);

        Task<IEnumerable<Categoria>> ObtenerPorNegocio(int negocioId);

        Task<bool> ActivarCategoria(int id);
        Task<bool> DesactivarCategoria(int id);
    }
}
