namespace InventarioAPI.Models.Seguridad
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        // Relación con Rol
        public int RolId { get; set; }           // <-- Aquí estaba el problema
        public Rol Rol { get; set; } = null!;   // Propiedad de navegación
    }
}
