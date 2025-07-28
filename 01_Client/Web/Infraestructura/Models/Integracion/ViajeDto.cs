﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Models.Integracion
{
    internal class ViajeDto
    {
        public int IdViaje { get; set; }
        public DateTime Fecha { get; set; }
        public string HoraSalida { get; set; }
        public string Estado { get; set; }
        public int IdRuta { get; set; }
        public int IdChofer { get; set; }
        public int IdBus { get; set; }
    }
}
