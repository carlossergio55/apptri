using System.ComponentModel.DataAnnotations;

namespace Infraestructura.Models.Integracion
{
    public class RutaParadaDto
    {
        public int IdRutaParada { get; set; }  

        [Required(ErrorMessage = "La ruta es obligatoria")]
        public int IdRuta { get; set; }

        [Required(ErrorMessage = "La parada es obligatoria")]
        public int IdParada { get; set; }

        [Range(0, 999, ErrorMessage = "Orden inválido")]
        public int Orden { get; set; }
    }
}
