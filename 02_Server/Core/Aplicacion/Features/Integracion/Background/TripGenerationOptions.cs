namespace Aplicacion.Features.Integracion.Background
{

    public class TripGenerationOptions
    {
        
        public int DaysAhead { get; set; } = 14;

       
        public string RunAtLocalTime { get; set; } = "02:00";
    }
}
