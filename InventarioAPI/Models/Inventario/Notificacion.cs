using System.ComponentModel.DataAnnotations;

namespace InventarioAPI.Models.Inventario
{
    public class Notificacion
    {
        [Key]
        public int Id { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Now;
        public bool Leida { get; set; } = false;
        public string Tipo { get; set; } = "StockBajo"; // Critico o Advertencia
    }
}