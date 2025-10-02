using System;
using System.ComponentModel.DataAnnotations;

namespace Infraestructura.Models.Integracion
{
    public class ViajeDto
    {
        public int IdViaje { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

       
        public string HoraSalida { get; set; } = "00:00";

        public string Estado { get; set; } = "PROGRAMADO";

        public string Direccion { get; set; } = "IDA"; // "IDA" | "VUELTA"
        public int? DesdeParadaId { get; set; }
        public int? HastaParadaId { get; set; }

        [Required]
        public int IdRuta { get; set; }

        [Required]
        public int IdChofer { get; set; }

        [Required]
        public int IdBus { get; set; }
        public int DestinoParadaId { get; set; }
        public int OrigenParadaId { get; set; }
    }
}
