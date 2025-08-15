using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Integracion
{
    public class HorarioDto
    {
        public int IdHorario { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public string DiaSemana { get; set; }    // si luego migras a SMALLINT, aquí cambiamos a short
        public string Direccion { get; set; }    // "IDA" | "VUELTA"
        public int? DesdeParadaId { get; set; }
        public int? HastaParadaId { get; set; }
        public int IdRuta { get; set; }
    }
}
