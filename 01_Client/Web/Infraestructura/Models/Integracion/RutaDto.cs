using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Infraestructura.Models.Integracion
{
    public class RutaDto
    {
        public int IdRuta { get; set; }

        [Required(ErrorMessage = "El origen es obligatorio")]
        public string Origen { get; set; } = string.Empty;

        [Required(ErrorMessage = "El destino es obligatorio")]
        public string Destino { get; set; } = string.Empty;

        [RegularExpression(@"^\d{2}:\d{2}$", ErrorMessage = "Formato HH:mm (ej. 04:30)")]
        public string Duracion { get; set; } = string.Empty;

        public bool EsExtendido { get; set; }
    }
}
