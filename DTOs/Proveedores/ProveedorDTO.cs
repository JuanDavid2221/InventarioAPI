namespace InventarioAPI.DTOs.Proveedores
{
    public class CrearProveedorDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string NIT { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public string? NombreContacto { get; set; }
    }
}