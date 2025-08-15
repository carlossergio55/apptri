using System.ComponentModel.DataAnnotations;

namespace Infraestructura.Models.Integracion
{
    public class HorarioDto
    {
        public int IdHorario { get; set; }

        
        public string HoraSalida { get; set; } = "00:00";

        [Required]
        public string DiaSemana { get; set; } = "Lun"; // o "0..6" según tu UI

        // Nuevos
        public string Direccion { get; set; } = "IDA"; // "IDA" | "VUELTA"
        public int? DesdeParadaId { get; set; }
        public int? HastaParadaId { get; set; }

        [Required]
        public int IdRuta { get; set; }
    }
}
