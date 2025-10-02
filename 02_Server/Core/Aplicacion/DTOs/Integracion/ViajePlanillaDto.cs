using System;

public class ViajePlanillaDto
{
    public int IdViaje { get; set; }
    public DateTime Fecha { get; set; }
    public string HoraSalida { get; set; } = "";
    public string Estado { get; set; } = "PROGRAMADO";

    // Relacionados
    public string RutaNombre { get; set; } = "";
    public string Placa { get; set; } = "";
    public string ChoferNombre { get; set; } = "";

    // Capacidad y ocupación
    public int Capacidad { get; set; }
    public int Ocupados { get; set; }
    public int OrigenParadaId { get; set; }
    public int DestinoParadaId { get; set; }
}
