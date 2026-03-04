using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InventarioAPI.Models.Empresa;

namespace InventarioAPI.Models.Seguridad
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Correo { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Propiedad para habilitar/deshabilitar acceso (Baja Lógica)
        public bool Activo { get; set; } = true;

        // Relación con Rol
        public int RolId { get; set; }
        [ForeignKey("RolId")]
        public Rol? Rol { get; set; }

        // Clave foránea a Empresa
        public int? IdEmpresa { get; set; }

        [ForeignKey("IdEmpresa")]
        // Usamos la ruta completa para evitar el error CS0118 (conflicto con el namespace)
        public virtual InventarioAPI.Models.Empresa.Empresa? Empresa { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}