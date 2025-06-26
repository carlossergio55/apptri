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
        public string Remitente { get; set; }
        public string Destinatario { get; set; }
        public string Descripcion { get; set; }
        public int IdViaje { get; set; }
        public decimal Precio { get; set; }
        public string Estado { get; set; }
        public decimal Peso { get; set; }
        public bool Pagado { get; set; }
        public string GuiaCarga { get; set; }
    }
}
