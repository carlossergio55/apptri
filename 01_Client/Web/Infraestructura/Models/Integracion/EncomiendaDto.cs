// Infraestructura/Models/Integracion/EncomiendaDto.cs
using System.ComponentModel.DataAnnotations;

namespace Infraestructura.Models.Integracion
{
    public class EncomiendaDto
    {
        public int IdEncomienda { get; set; }

        [Required(ErrorMessage = "El remitente es obligatorio")]
        public string Remitente { get; set; } = string.Empty;

        [Required(ErrorMessage = "El destinatario es obligatorio")]
        public string Destinatario { get; set; } = string.Empty;
        public string? Guiacarga { get; set; }
        public string? Descripcion { get; set; }

        [Range(0.01, 1000, ErrorMessage = "Peso inválido")]
        public decimal Peso { get; set; }

        [Range(0.01, 10000, ErrorMessage = "Precio inválido")]
        public decimal Precio { get; set; }

        [Required]
        public int IdViaje { get; set; }

        // ----- Guía de carga (nuevo esquema) -----
        public int IdGuiaCarga { get; set; }                 // FK (lo llena el back si mandas CodigoGuia)
        public string CodigoGuia { get; set; } = string.Empty; // Visible en UI

        public string Estado { get; set; } = "en camino";
        public bool Pagado { get; set; }

        // ----- Tramo (nuevos) -----
        public int? OrigenParadaId { get; set; }
        public int? DestinoParadaId { get; set; }
    }
}
