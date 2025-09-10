using System;
using System.ComponentModel.DataAnnotations;

namespace Infraestructura.Models.Integracion
{
    /// DTO de solo lectura para tracking público
    public class EncomiendaPublicDto
    {
        public string Guiacarga { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;

        // Tramo
        public int? OrigenParadaId { get; set; }
        public string? OrigenParadaNombre { get; set; }
        public int? DestinoParadaId { get; set; }
        public string? DestinoParadaNombre { get; set; }

        // Datos del viaje
        public DateTime? FechaViaje { get; set; }
        public string? HoraSalida { get; set; }
        public int IdRuta { get; set; }
    }
}
