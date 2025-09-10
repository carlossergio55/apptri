using System;

namespace Aplicacion.DTOs.Integracion
{
    
    /// DTO de solo lectura para que el cliente rastree su encomienda por código (guiacarga).
 
    public class EncomiendaPublicDto
    {
        public string Guiacarga { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;

        // Tramo
        public int? OrigenParadaId { get; set; }
        public string? OrigenParadaNombre { get; set; }
        public int? DestinoParadaId { get; set; }
        public string? DestinoParadaNombre { get; set; }

        // Datos del viaje (útiles para tracking)
        public DateTime? FechaViaje { get; set; }
        public string? HoraSalida { get; set; }
        public int IdRuta { get; set; }
    }
}
