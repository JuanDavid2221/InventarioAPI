using System.ComponentModel.DataAnnotations;
using InventarioAPI.Models.Seguridad;

namespace InventarioAPI.Models.Inventario
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public decimal Total { get; set; }
        public int UsuarioId { get; set; }
        public virtual Usuario? Usuario { get; set; }
        public virtual ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}