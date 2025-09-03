using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Models.Integracion
{
    public class SucursalDto
    {
        public int IdSucursal { get; set; }
        public int IdParada { get; set; }
        public string Nombre { get; set; }
        public string Ciudad { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string Estado { get; set; }
    }
}
