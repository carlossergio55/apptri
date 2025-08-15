using System.ComponentModel.DataAnnotations;

namespace Infraestructura.Models.Integracion
{
    public class TarifaTramoDto
    {
        [Required]
        public int IdRuta { get; set; }

        [Required]
        public int OrigenParadaId { get; set; }

        [Required]
        public int DestinoParadaId { get; set; }

        [Range(0.01, 10000, ErrorMessage = "Precio inválido")]
        public decimal Precio { get; set; }
    }
}
