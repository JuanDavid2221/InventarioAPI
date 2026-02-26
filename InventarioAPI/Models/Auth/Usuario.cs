using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InventarioAPI.Models.Auth;

namespace InventarioAPI.Models.Seguridad
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; } // Coincide con tu Lucidspark
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;

        // Clave foránea a Empresa
        public int? IdEmpresa { get; set; }

        [ForeignKey("IdEmpresa")]
        public Empresa? Empresa { get; set; }
    }
}