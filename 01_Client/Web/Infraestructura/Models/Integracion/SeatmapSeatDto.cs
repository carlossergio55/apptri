namespace Web.Infraestructura.Models.Integracion
{
    public class SeatmapSeatDto
    {
        public int IdAsiento { get; set; }
        public int Numero { get; set; }
        public string EstadoSeat { get; set; } = "LIBRE";
        public int? IdBoleto { get; set; }
        public string? ClienteNombre { get; set; }
        public string? ClienteCI { get; set; }
        public decimal? Precio { get; set; }
        public int? OrigenParadaId { get; set; }
        public int? DestinoParadaId { get; set; }
    }
}
