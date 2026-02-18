namespace InventarioAPI.DTOs.Auth
{
    public class RegistrarEmpleadoDTO
    {
        public string Nombre { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
