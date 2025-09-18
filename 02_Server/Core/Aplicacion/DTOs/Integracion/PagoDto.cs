using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Integracion
{
    public class PagoDto
    {
        public int IdPago { get; set; }

        public string TipoPago { get; set; }

        public int IdReferencia { get; set; }

        public decimal Monto { get; set; }

        public string Metodo { get; set; }

        public DateTime FechaPago { get; set; }

        public int IdUsuario { get; set; }

        public int IdCliente { get; set; }
        public string? Referencia { get; set; }
    }
}
