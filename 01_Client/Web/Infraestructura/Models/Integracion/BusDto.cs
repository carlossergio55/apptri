using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Models.Integracion
{
    internal class BusDto
    {
        public int IdBus { get; set; }
        public string Placa { get; set; }
        public string Modelo { get; set; }
        public int Capacidad { get; set; }
    }
}
