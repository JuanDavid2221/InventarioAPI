using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InventarioAPI.Models.Proveedores
{
    public class Proveedor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string NIT { get; set; } = string.Empty;

        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public string? NombreContacto { get; set; }

        public bool Activo { get; set; } = true;

        [Required]
        public int IdEmpresa { get; set; }

        // PROPIEDAD DE NAVEGACIÓN
        [JsonIgnore]
        [ForeignKey("IdEmpresa")]
        // Usamos la ruta completa para evitar el error de "es un espacio de nombres pero se usa como tipo"
        public virtual InventarioAPI.Models.Empresa.Empresa? Empresa { get; set; }
    }
}