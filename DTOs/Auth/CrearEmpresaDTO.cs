using InventarioAPI.Models;
using InventarioAPI.Models.Inventario;

namespace InventarioAPI.DTO.Auth
{
    public class CrearEmpresaDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public TipoEmpresa Tipo { get; set; } // 1: Tienda, 2: Supermercado, 3: Ventanilla
    }
}