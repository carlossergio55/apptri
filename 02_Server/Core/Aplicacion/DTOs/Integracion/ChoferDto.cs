﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Integracion
{
    public class ChoferDto
    {
        public int IdChofer { get; set; }
        public string Nombre { get; set; }
        public int Carnet { get; set; }
        public int Celular { get; set; }
        public string Licencia { get; set; }
    }
}
