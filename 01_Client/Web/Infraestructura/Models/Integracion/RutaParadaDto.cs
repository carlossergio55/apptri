using System.ComponentModel.DataAnnotations;

namespace Infraestructura.Models.Integracion
{
    public class RutaParadaDto
    {
        [Required]
        public int IdRuta { get; set; }

        [Required]
        public int IdParada { get; set; }

        [Range(0, 999, ErrorMessage = "Orden inválido")]
        public int Orden { get; set; }
    }
}
