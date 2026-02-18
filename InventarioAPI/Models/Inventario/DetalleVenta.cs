using System.ComponentModel.DataAnnotations;

namespace InventarioAPI.Models.Inventario
{
    public class DetalleVenta
    {
        public int Id { get; set; }
        public int VentaId { get; set; }
        public virtual Venta? Venta { get; set; }
        public int ProductoId { get; set; }
        public virtual Producto? Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}