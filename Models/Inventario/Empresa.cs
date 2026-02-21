using System.Collections.Generic;
using InventarioAPI.Models.Seguridad;

namespace InventarioAPI.Models.Inventario
{
    public class Empresa
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Nit { get; set; }

        // Agregamos estos campos para que coincidan con el controlador
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }

        public TipoEmpresa Tipo { get; set; }

        public int PropietarioId { get; set; }
        public Usuario? Propietario { get; set; }

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}