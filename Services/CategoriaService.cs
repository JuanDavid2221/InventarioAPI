using InventarioAPI.Data;
// Borra el using InventarioAPI.Services de aquí, no es necesario.
using Microsoft.EntityFrameworkCore;
using InventarioAPI.Models;

namespace InventarioAPI.Services; // <--- AGREGA ESTA LÍNEA AQUÍ


public class CategoriaService : ICategoriaService
{
    private readonly InventarioContext _context;

    public CategoriaService(InventarioContext context)
    {
        _context = context;
    }

    public async Task<Categoria> CrearCategoria(Categoria categoria)
    {
        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();

        return categoria;
    }

    public async Task<IEnumerable<Categoria>> ObtenerPorNegocio(int negocioId)
    {
        return await _context.Categorias
            .Where(c => c.NegocioId == negocioId)
            .ToListAsync();
    }

    public async Task<bool> ActivarCategoria(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);

        if (categoria == null)
            return false;

        categoria.Activo = true;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DesactivarCategoria(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);

        if (categoria == null)
            return false;

        categoria.Activo = false;

        await _context.SaveChangesAsync();

        return true;
    }

}




