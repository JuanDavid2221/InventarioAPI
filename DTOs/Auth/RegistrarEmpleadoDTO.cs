namespace InventarioAPI.DTOs.Auth
{
    public class RegistrarEmpleadoDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}   