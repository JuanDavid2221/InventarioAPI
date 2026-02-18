using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioAPI.Models.Inventario
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        // 🔴 CLAVE PARA EL LECTOR MÓVIL
        public string CodigoBarras { get; set; } = string.Empty;

        public string? Marca { get; set; } // Ej: Postobón, Coca-Cola, Diana

        public string? UnidadMedida { get; set; } // Ej: 1.5L, 500gr, Unidad

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioCompra { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioVenta { get; set; }

        // Campo calculado para ver la utilidad rápido en SQL
        [NotMapped]
        public decimal GananciaPorUnidad => PrecioVenta - PrecioCompra;

        public int Stock { get; set; }

        public int StockMinimo { get; set; }

        // 🟢 Fecha de vencimiento (opcional, por si vendes tornillos o cosas que no vencen)
        public DateTime? FechaVencimiento { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public bool Activo { get; set; } = true;

        // RELACION
        [Required]
        public int CategoriaId { get; set; }

        [ForeignKey("CategoriaId")]
        public Categoria? Categoria { get; set; }
    }
}