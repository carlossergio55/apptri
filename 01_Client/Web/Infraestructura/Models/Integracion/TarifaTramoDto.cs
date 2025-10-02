using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Infraestructura.Models.Integracion
{
    public class TarifaTramoDto
    {
        
        public int? IdTarifaTramo { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Ruta inválida")]
        public int IdRuta { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Parada de origen inválida")]
        public int OrigenParadaId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Parada de destino inválida")]
        public int DestinoParadaId { get; set; }

        [Range(0.01, 1000000, ErrorMessage = "Precio inválido")]
        public decimal Precio { get; set; }

     
    }
}
