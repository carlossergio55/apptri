using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Integracion
{
    public class BoletoDto
    {
        public int IdBoleto { get; set; }
        public decimal Precio { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCompra { get; set; }

        public int IdViaje { get; set; }
        public int IdCliente { get; set; }
        public int IdAsiento { get; set; }
        public int IdVendedor { get; set; }
    }
}
