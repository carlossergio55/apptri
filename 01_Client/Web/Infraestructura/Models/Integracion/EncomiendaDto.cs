using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Models.Integracion
{
    public class EncomiendaDto
    {
        public int IdEncomienda { get; set; }

        [Required(ErrorMessage = "El remitente es obligatorio")]
        public string Remitente { get; set; }

        [Required(ErrorMessage = "El destinatario es obligatorio")]
        public string Destinatario { get; set; }

        [Required]
        public string Descripcion { get; set; }

        [Range(0.01, 1000, ErrorMessage = "Peso inválido")]
        public decimal Peso { get; set; }

        [Range(0.01, 10000, ErrorMessage = "Precio inválido")]
        public decimal Precio { get; set; }

        [Required]
        public int IdViaje { get; set; }

        public string GuiaCarga { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public bool Pagado { get; set; }
    }
}
