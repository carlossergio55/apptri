using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Integracion
{
    public class EncomiendaDto
    {
        public int IdEncomienda { get; set; }
        public string Remitente { get; set; } = string.Empty;
        public string Destinatario { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? Guiacarga { get; set; }
        public int IdViaje { get; set; }
        public decimal Precio { get; set; }
        public string Estado { get; set; } = "en camino";
        public decimal Peso { get; set; }
        public bool Pagado { get; set; }

 
        // Tramo
        public int? OrigenParadaId { get; set; }
        public int? DestinoParadaId { get; set; }
    }
}
