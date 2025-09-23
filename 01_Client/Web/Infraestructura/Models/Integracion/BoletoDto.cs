using System;
using System.ComponentModel.DataAnnotations;

namespace Infraestructura.Models.Integracion
{

    public class BoletoDto
    {
        public int IdBoleto { get; set; }

        [Range(0.01, 10000, ErrorMessage = "Precio inválido")]
        public decimal Precio { get; set; }

        [Required]
        public string Estado { get; set; } = "RESERVADO";

        public DateTime? FechaReservaUtc { get; set; }
      
        public DateTime? FechaConfirmacionUtc { get; set; }
       
        public DateTime? FechaCompra { get; set; }

        [Required]
        public int IdViaje { get; set; }

        [Required]
        public int IdCliente { get; set; }

        [Required]
        public int IdAsiento { get; set; }

        public int? IdPago { get; set; }
        public int? OrigenParadaId { get; set; }
        public int? DestinoParadaId { get; set; }
    }
}
