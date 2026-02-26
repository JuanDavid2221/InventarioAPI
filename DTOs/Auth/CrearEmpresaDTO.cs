using InventarioAPI.Models.Auth;

namespace InventarioAPI.DTO.Auth
{
    public class CrearEmpresaDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string? NIT_RUT { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? CorreoContacto { get; set; }
        public TipoEmpresa TipoNegocio { get; set; }
    }
}