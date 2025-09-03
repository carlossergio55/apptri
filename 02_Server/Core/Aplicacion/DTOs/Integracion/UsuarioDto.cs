using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Integracion
{
    public class UsuarioDto
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
        public string Tipo { get; set; }

        // Relación
        public int? IdSucursal { get; set; }                 // Puede ser null
        public string? NombreSucursal { get; set; }          // Para mostrar en vistas
        public SucursalDto? Sucursal { get; set; }           // Opcional: DTO completo
    }
}

