using System;
using System.Collections.Generic;
using InventarioAPI.Models.Seguridad;

namespace InventarioAPI.Models.Auth
{
    public class Empresa
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? NIT_RUT { get; set; } // Coincide con tu Lucidspark
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? CorreoContacto { get; set; } // Coincide con tu Lucidspark
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public TipoEmpresa TipoNegocio { get; set; } // El 1, 2 o 3

        // Relación con el Dueño
        public int PropietarioId { get; set; }
        public Usuario? Propietario { get; set; }

        // Relación con la lista de usuarios
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}