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
        public string DiaSemana { get; set; }
        public int IdRuta { get; set; }
    }
}
