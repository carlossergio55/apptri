using System;

namespace Aplicacion.DTOs.Integracion
{
    public class BoletoDto
    {
        public int IdBoleto { get; set; }

        public decimal Precio { get; set; }

        public string Estado { get; set; } = "BLOQUEADO";

        public DateTime? FechaCompra { get; set; }

        //NUevas propiedades para la expiración de reservas
        public DateTime? FechaReservaUtc { get; set; }

        public DateTime? FechaConfirmacionUtc { get; set; }
        //------------------------------------------------


        public int IdViaje { get; set; }
        public int IdCliente { get; set; }
        public int IdAsiento { get; set; }

        public int? IdPago { get; set; }
        public int? OrigenParadaId { get; set; }
        public int? DestinoParadaId { get; set; }
    }
}
