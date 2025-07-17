using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Models.Integracion
{
    public class RutaDto
    {
        public int IdRuta { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }
        public string Duracion { get; set; }
        public bool EsExtendido { get; set; }
    }
}
