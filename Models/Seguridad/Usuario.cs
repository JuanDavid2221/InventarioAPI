using System.ComponentModel.DataAnnotations;
using InventarioAPI.Models.Inventario; // <--- ESTO ES LO QUE FALTA

namespace InventarioAPI.Models.Seguridad
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Correo { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public int RolId { get; set; }
        public Rol? Rol { get; set; }

        public int? EmpresaId { get; set; }
        public Empresa? Empresa { get; set; }
    }
}