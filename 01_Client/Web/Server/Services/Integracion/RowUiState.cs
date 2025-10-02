using Infraestructura.Models.Integracion;
using System.Collections.Generic;

using Web.Infraestructura.Models.Integracion;

namespace Server.Services.Integracion
{
    public class RowUiState
    {
        public bool Loading { get; set; }
        public ViajePlanillaDto Viaje { get; set; }   

        // Tramo
        public int OrigenParadaId { get; set; }
        public int DestinoParadaId { get; set; }
        public List<ParadaDto> Paradas { get; set; } = new();

        // Seatmap
        public List<SeatmapSeatDto> Seatmap { get; set; } = new();

        // Cliente
        public ClienteDto Cliente { get; set; } = new();

        // Selección
        public List<int> Pendientes { get; set; } = new();
        public Dictionary<int, SeatmapSeatDto> Seleccionados { get; set; } = new();

        // Pago
        public decimal PrecioUnitario { get; set; }
        public string MetodoPago { get; set; } = "EFECTIVO";
        public string? ReferenciaPago { get; set; }
    }
}
