using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InventarioAPI.Models.Inventario
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        public bool Activa { get; set; } = true;

        [JsonIgnore] // 🔴 Evita que el JSON se vuelva infinito
        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}