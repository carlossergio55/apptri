using System.ComponentModel.DataAnnotations;

namespace Infraestructura.Models.Integracion
{
    public class HorarioDto
    {
        public int IdHorario { get; set; }

        [Required(ErrorMessage = "La hora de salida es obligatoria")]
        [RegularExpression(@"^\d{2}:\d{2}$", ErrorMessage = "Formato HH:mm (ej. 04:30)")]
        public string HoraSalida { get; set; } = "00:00";

        [Required(ErrorMessage = "El día de la semana es obligatorio")]
        public string DiaSemana { get; set; } = "Lun"; // Lun, Mar, Mié, Jue, Vie, Sáb, Dom

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [RegularExpression("^(IDA|VUELTA)$", ErrorMessage = "Dirección inválida (IDA o VUELTA)")]
        public string Direccion { get; set; } = "IDA";

        public int? DesdeParadaId { get; set; }
        public int? HastaParadaId { get; set; }

        [Required(ErrorMessage = "La ruta es obligatoria")]
        public int IdRuta { get; set; }
    }
}
