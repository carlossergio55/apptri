using System.ComponentModel.DataAnnotations;

namespace Infraestructura.Models.Integracion
{
    public class ParadaDto
    {
        public int IdParada { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;
    }
}
